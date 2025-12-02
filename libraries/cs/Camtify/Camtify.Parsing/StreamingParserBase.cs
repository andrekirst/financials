using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml;
using Camtify.Core;
using Camtify.Core.Parsing;
using Microsoft.Extensions.Logging;

namespace Camtify.Parsing;

/// <summary>
/// Abstract base class for streaming parsers.
/// Enables processing of large files with constant memory usage.
/// </summary>
/// <typeparam name="TDocument">The document type (for header).</typeparam>
/// <typeparam name="TEntry">The entry type (what gets streamed).</typeparam>
/// <remarks>
/// Streaming parsers are ideal for:
/// - camt.053 with millions of entries
/// - pain.001 bulk files with 100K+ transactions
/// - ETL processes (file → database)
///
/// Memory usage remains constant, independent of file size.
/// </remarks>
/// <example>
/// <code>
/// public class Camt053StreamingParser : StreamingParserBase&lt;Camt053Document, CamtEntry&gt;
/// {
///     protected override string EntryElementName =&gt; "Ntry";
///
///     protected override async Task&lt;CamtEntry&gt; ParseEntryAsync(
///         XmlReader reader, CancellationToken cancellationToken)
///     {
///         // Entry parsing logic
///     }
/// }
/// </code>
/// </example>
public abstract class StreamingParserBase<TDocument, TEntry> : IStreamingParser<TEntry>
    where TDocument : class
    where TEntry : class
{
    private readonly IXmlReaderFactory _xmlReaderFactory;
    private readonly IMessageDetector _messageDetector;
    private readonly ILogger? _logger;

    /// <summary>
    /// Name of the entry element (e.g., "Ntry", "CdtTrfTxInf").
    /// </summary>
    protected abstract string EntryElementName { get; }

    /// <summary>
    /// Optional parent element names for nested entries.
    /// E.g., for pain.001: ["PmtInf", "CdtTrfTxInf"]
    /// </summary>
    protected virtual string[]? ParentElementPath => null;

    /// <summary>
    /// Supported message identifiers.
    /// </summary>
    public abstract IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamingParserBase{TDocument, TEntry}"/> class.
    /// </summary>
    /// <param name="xmlReaderFactory">Factory for creating XML readers.</param>
    /// <param name="messageDetector">Detector for identifying message types.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    protected StreamingParserBase(
        IXmlReaderFactory xmlReaderFactory,
        IMessageDetector messageDetector,
        ILogger? logger = null)
    {
        _xmlReaderFactory = xmlReaderFactory ?? throw new ArgumentNullException(nameof(xmlReaderFactory));
        _messageDetector = messageDetector ?? throw new ArgumentNullException(nameof(messageDetector));
        _logger = logger;
    }

    #region IStreamingParser Implementation

    /// <inheritdoc />
    public async IAsyncEnumerable<TEntry> ParseEntriesAsync(
        Stream stream,
        ParseOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        options ??= ParseOptions.Default;

        using var reader = _xmlReaderFactory.Create(stream);
        var entriesParsed = 0;
        var stopwatch = Stopwatch.StartNew();

        // Initialize progress
        ReportProgress(options.Progress, new ParseProgress
        {
            Status = ParseStatus.Starting,
            TotalBytes = stream.CanSeek ? stream.Length : null
        });

        _logger?.LogDebug("Starting streaming parse for {EntryElement}", EntryElementName);

        // Navigate to first entry
        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element &&
                reader.LocalName == EntryElementName)
            {
                TEntry? entry = null;

                try
                {
                    entry = await ParseEntryAsync(reader, cancellationToken);
                }
                catch (Exception ex) when (!options.StopOnFirstError)
                {
                    _logger?.LogWarning(ex, "Error parsing entry {Index}", entriesParsed);
                    continue;
                }

                if (entry is not null)
                {
                    entriesParsed++;

                    // Report progress every 1000 entries
                    if (entriesParsed % 1000 == 0)
                    {
                        ReportProgress(options.Progress, new ParseProgress
                        {
                            Status = ParseStatus.ParsingEntries,
                            EntriesParsed = entriesParsed,
                            BytesRead = stream.CanSeek ? stream.Position : 0,
                            TotalBytes = stream.CanSeek ? stream.Length : null
                        });
                    }

                    // Yield entry before checking max limit
                    yield return entry;

                    // Check max entries limit
                    if (options.MaxEntries > 0 && entriesParsed >= options.MaxEntries)
                    {
                        _logger?.LogInformation(
                            "MaxEntries reached ({MaxEntries}), streaming ended",
                            options.MaxEntries);
                        break;
                    }
                }
            }
        }

        stopwatch.Stop();

        ReportProgress(options.Progress, new ParseProgress
        {
            Status = ParseStatus.Completed,
            EntriesParsed = entriesParsed,
            BytesRead = stream.CanSeek ? stream.Position : 0,
            TotalBytes = stream.CanSeek ? stream.Length : null
        });

        _logger?.LogInformation(
            "Streaming completed: {Count} entries in {Duration}ms ({Rate}/s)",
            entriesParsed,
            stopwatch.ElapsedMilliseconds,
            entriesParsed * 1000 / Math.Max(1, stopwatch.ElapsedMilliseconds));
    }

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// <strong>IMPORTANT:</strong> This method requires a <strong>seekable stream</strong> (stream.CanSeek = true).
    /// The stream is read multiple times: once for message detection, once for header parsing,
    /// and once for entry streaming.
    /// </para>
    /// <para>
    /// Non-seekable streams (HTTP response streams, network pipes, stdin) are not supported.
    /// For non-seekable streams, use <see cref="ParseEntriesAsync"/> directly without context.
    /// </para>
    /// <para>
    /// For file-based parsing, use FileStream which is always seekable.
    /// For in-memory parsing, use MemoryStream which is also seekable.
    /// </para>
    /// </remarks>
    public async Task<StreamingParseResult<TEntry>> ParseWithContextAsync(
        Stream stream,
        ParseOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        options ??= ParseOptions.Default;

        // Remember stream position for later reset
        var startPosition = stream.CanSeek ? stream.Position : 0;

        // 1. Message detection
        var detection = await _messageDetector.DetectWithDetailsAsync(stream, cancellationToken);

        // 2. Reset stream
        if (stream.CanSeek)
        {
            stream.Position = startPosition;
        }

        // 3. Parse header
        var header = await ParseHeaderAsync(stream, detection.MessageId, cancellationToken);

        // 4. Reset stream again for entry streaming
        if (stream.CanSeek)
        {
            stream.Position = startPosition;
        }

        // 5. Create result with entry stream
        return new StreamingParseResult<TEntry>
        {
            MessageId = detection.MessageId,
            Header = header,
            ApplicationHeader = detection.HasApplicationHeader
                ? await ParseApplicationHeaderAsync(stream, cancellationToken)
                : null,
            ExpectedEntryCount = GetExpectedEntryCount(header),
            Entries = ParseEntriesAsync(stream, options, cancellationToken)
        };
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Parses a single entry from the XmlReader.
    /// </summary>
    /// <param name="reader">Positioned XmlReader at entry element.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed entry.</returns>
    /// <remarks>
    /// After calling, the reader should be positioned after the entry element.
    /// Use XmlReader extension methods from <see cref="XmlReaderExtensions"/> for parsing.
    /// For complex nested structures, use:
    /// - <see cref="XmlReaderExtensions.ReadDateElementAsync"/> for BookgDt/Dt patterns
    /// - <see cref="XmlReaderExtensions.ReadAmountWithCurrencyAsync"/> for Amt[@Ccy]
    /// - <see cref="XmlReaderExtensions.MoveToElementAsync"/> for navigation
    /// </remarks>
    protected abstract Task<TEntry> ParseEntryAsync(
        XmlReader reader,
        CancellationToken cancellationToken);

    /// <summary>
    /// Parses the header (GroupHeader or Statement-Header).
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="messageId">The detected message identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed header.</returns>
    protected abstract Task<object> ParseHeaderAsync(
        Stream stream,
        MessageIdentifier messageId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Extracts the expected entry count from the header (if available).
    /// </summary>
    /// <param name="header">The parsed header.</param>
    /// <returns>The expected entry count, or null if not available.</returns>
    protected virtual int? GetExpectedEntryCount(object header) => null;

    #endregion

    #region Protected Helper Methods

    /// <summary>
    /// Counts entries without parsing them fully (for pre-counting).
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entries in the document.</returns>
    public async Task<int> CountEntriesAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var reader = _xmlReaderFactory.Create(stream);
        var count = 0;

        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element &&
                reader.LocalName == EntryElementName)
            {
                count++;
                await reader.SkipAsync(); // Skip entry content
            }
        }

        return count;
    }

    #endregion

    #region Private Methods

    private async Task<BusinessApplicationHeader?> ParseApplicationHeaderAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        // Simplified implementation - full parsing in separate parser
        using var reader = _xmlReaderFactory.Create(stream);

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "AppHdr")
            {
                // TODO: Parse BAH...
                break;
            }
        }

        return null;
    }

    private static void ReportProgress(IProgress<ParseProgress>? progress, ParseProgress value)
    {
        progress?.Report(value);
    }

    #endregion
}
