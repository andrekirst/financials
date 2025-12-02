using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Camtify.Core;
using Camtify.Core.Parsing;
using Microsoft.Extensions.Logging;

namespace Camtify.Parsing;

/// <summary>
/// Abstract base class for all ISO 20022 parsers.
/// Implements Template-Method-Pattern.
/// </summary>
/// <typeparam name="TDocument">The document type.</typeparam>
/// <remarks>
/// Concrete parsers inherit from this class and implement:
/// - <see cref="ParseDocumentCoreAsync"/> for the actual parsing
/// - <see cref="SupportedMessages"/> for supported versions
///
/// The base class handles:
/// - Stream-Handling
/// - XmlReader-Creation
/// - BAH-Parsing
/// - Error-Handling
/// - Progress-Reporting
/// </remarks>
/// <example>
/// <code>
/// public class Pain001Parser : Iso20022ParserBase&lt;Pain001Document&gt;
/// {
///     public override IReadOnlyCollection&lt;MessageIdentifier&gt; SupportedMessages { get; } =
///         new[] { MessageIdentifier.Pain.V001_09, MessageIdentifier.Pain.V001_10 };
///
///     protected override async Task&lt;Pain001Document&gt; ParseDocumentCoreAsync(
///         XmlReader reader,
///         MessageIdentifier messageId,
///         BusinessApplicationHeader? applicationHeader,
///         ParseOptions options,
///         List&lt;ParseError&gt; errors,
///         List&lt;ParseWarning&gt; warnings,
///         CancellationToken cancellationToken)
///     {
///         // Specific parsing logic
///     }
/// }
/// </code>
/// </example>
public abstract class Iso20022ParserBase<TDocument> : IIso20022Parser<TDocument>
    where TDocument : class
{
    private readonly IXmlReaderFactory _xmlReaderFactory;
    private readonly IMessageDetector _messageDetector;
    private readonly ILogger? _logger;

    /// <summary>
    /// Gets the list of supported message identifiers.
    /// </summary>
    public abstract IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; }

    /// <summary>
    /// Gets the primary message identifier (first in the list of supported messages).
    /// </summary>
    /// <remarks>
    /// This property is used to satisfy the <see cref="IIso20022Parser{TDocument}.MessageIdentifier"/> requirement.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the parser does not support any message types.</exception>
    public MessageIdentifier MessageIdentifier
    {
        get
        {
            if (!SupportedMessages.Any())
            {
                throw new InvalidOperationException(
                    $"Parser {GetType().Name} must support at least one message type.");
            }

            return SupportedMessages.First();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022ParserBase{TDocument}"/> class.
    /// </summary>
    /// <param name="xmlReaderFactory">The XML reader factory.</param>
    /// <param name="messageDetector">The message detector.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    protected Iso20022ParserBase(
        IXmlReaderFactory xmlReaderFactory,
        IMessageDetector messageDetector,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(xmlReaderFactory);
        ArgumentNullException.ThrowIfNull(messageDetector);

        _xmlReaderFactory = xmlReaderFactory;
        _messageDetector = messageDetector;
        _logger = logger;
    }

    #region Public Interface

    /// <inheritdoc />
    public async Task<TDocument> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return await ParseAsync(stream, null, cancellationToken);
    }

    /// <summary>
    /// Parses an ISO 20022 message from a stream with options.
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="options">Parse options (null uses defaults).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed document.</returns>
    public async Task<TDocument> ParseAsync(
        Stream stream,
        ParseOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        options ??= ParseOptions.Default;

        cancellationToken.ThrowIfCancellationRequested();

        var stopwatch = Stopwatch.StartNew();
        var errors = new List<ParseError>();
        var warnings = new List<ParseWarning>();

        try
        {
            // 1. Message type detection
            var detectionResult = await DetectMessageTypeAsync(stream, cancellationToken);

            // 2. Check if supported
            ValidateSupportedMessage(detectionResult.MessageId);

            // 3. Reset stream
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            else
            {
                throw new InvalidOperationException(
                    "Stream must be seekable or message detection must be skipped.");
            }

            // 4. Create XmlReader
            using var reader = CreateReader(stream, options);

            // 5. Initialize progress
            ReportProgress(options.Progress, new ParseProgress
            {
                Status = ParseStatus.Starting,
                TotalBytes = stream.CanSeek ? stream.Length : null
            });

            // 6. Optional: Parse BAH
            BusinessApplicationHeader? bah = null;
            if (options.ParseApplicationHeader && detectionResult.HasApplicationHeader)
            {
                bah = await ParseApplicationHeaderAsync(reader, cancellationToken);
            }

            // 7. Navigate to Document element
            await NavigateToDocumentAsync(reader, cancellationToken);

            // 8. Progress: Header done, Body starting
            ReportProgress(options.Progress, new ParseProgress
            {
                Status = ParseStatus.ParsingBody,
                BytesRead = stream.CanSeek ? stream.Position : 0
            });

            // 9. Actual parsing (abstract method)
            var document = await ParseDocumentCoreAsync(
                reader,
                detectionResult.MessageId,
                bah,
                options,
                errors,
                warnings,
                cancellationToken);

            // 10. Error handling
            if (errors.Count > 0 && options.StopOnFirstError)
            {
                throw new Iso20022ParsingException(
                    "Parse errors occurred",
                    errors,
                    warnings);
            }

            // 11. Progress: Complete
            stopwatch.Stop();
            ReportProgress(options.Progress, new ParseProgress
            {
                Status = ParseStatus.Completed,
                BytesRead = stream.CanSeek ? stream.Position : 0,
                TotalBytes = stream.CanSeek ? stream.Length : null
            });

            _logger?.LogInformation(
                "Parsing completed: {MessageId} in {Duration}ms, {Warnings} warnings",
                detectionResult.MessageId,
                stopwatch.ElapsedMilliseconds,
                warnings.Count);

            return document;
        }
        catch (Exception ex) when (ex is not Iso20022Exception)
        {
            _logger?.LogError(ex, "Error during parsing");
            throw new Iso20022ParsingException("Unexpected error during parsing", ex);
        }
    }

    /// <summary>
    /// Parses an ISO 20022 message from a file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="options">Parse options (null uses defaults).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed document.</returns>
    public async Task<TDocument> ParseAsync(
        string filePath,
        ParseOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 8192,
            useAsync: true);

        return await ParseAsync(stream, options, cancellationToken);
    }

    /// <summary>
    /// Parses an ISO 20022 message from a string.
    /// </summary>
    /// <param name="xml">The XML string.</param>
    /// <param name="options">Parse options (null uses defaults).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed document.</returns>
    public async Task<TDocument> ParseFromStringAsync(
        string xml,
        ParseOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(xml);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return await ParseAsync(stream, options, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> CanParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        try
        {
            var (success, messageId, _) = await _messageDetector.TryDetectAsync(stream, cancellationToken);

            if (!success || messageId is null)
            {
                return false;
            }

            return SupportedMessages.Contains(messageId.Value);
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Parses the actual document.
    /// Must be implemented by concrete parsers.
    /// </summary>
    /// <param name="reader">Positioned XmlReader (after Document element).</param>
    /// <param name="messageId">Detected message identifier.</param>
    /// <param name="applicationHeader">Parsed BAH (if present).</param>
    /// <param name="options">Parse options.</param>
    /// <param name="errors">List for errors (can be populated).</param>
    /// <param name="warnings">List for warnings (can be populated).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed document.</returns>
    protected abstract Task<TDocument> ParseDocumentCoreAsync(
        XmlReader reader,
        MessageIdentifier messageId,
        BusinessApplicationHeader? applicationHeader,
        ParseOptions options,
        List<ParseError> errors,
        List<ParseWarning> warnings,
        CancellationToken cancellationToken);

    #endregion

    #region Protected Helper Methods

    /// <summary>
    /// Creates an XmlReader with the appropriate settings.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Configured XmlReader.</returns>
    protected virtual XmlReader CreateReader(Stream stream, ParseOptions options)
    {
        if (options.ValidateSchema && !string.IsNullOrEmpty(options.SchemaPath))
        {
            var schemaSet = LoadSchemaSet(options.SchemaPath);
            return _xmlReaderFactory.CreateValidating(stream, schemaSet);
        }

        return _xmlReaderFactory.Create(stream);
    }

    /// <summary>
    /// Reads an element as string.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <returns>The element content as string, or null if empty.</returns>
    protected static async Task<string?> ReadElementContentAsync(XmlReader reader)
    {
        if (reader.IsEmptyElement)
        {
            return null;
        }

        return await reader.ReadElementContentAsStringAsync();
    }

    /// <summary>
    /// Reads an element as decimal.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <returns>The element content as decimal, or null if empty.</returns>
    protected static async Task<decimal?> ReadElementAsDecimalAsync(XmlReader reader)
    {
        var content = await ReadElementContentAsync(reader);
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        if (!decimal.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads an element as DateTime.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <returns>The element content as DateTime, or null if empty.</returns>
    protected static async Task<DateTime?> ReadElementAsDateTimeAsync(XmlReader reader)
    {
        var content = await ReadElementContentAsync(reader);
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        if (!DateTime.TryParse(content, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Navigates to the next element with the specified name.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="elementName">The element name to find.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the element was found; otherwise false.</returns>
    protected static async Task<bool> MoveToElementAsync(
        XmlReader reader,
        string elementName,
        CancellationToken cancellationToken)
    {
        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element &&
                reader.LocalName == elementName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Reads a subelement as XElement (for complex structures).
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The subelement as XElement, or null if not an element.</returns>
    protected static async Task<XElement?> ReadSubtreeAsXElementAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return null;

        using var subtreeReader = reader.ReadSubtree();
        return await XElement.LoadAsync(subtreeReader, LoadOptions.None, cancellationToken);
    }

    /// <summary>
    /// Adds a parse error.
    /// </summary>
    /// <param name="errors">The error list.</param>
    /// <param name="message">The error message.</param>
    /// <param name="reader">The XML reader.</param>
    /// <param name="innerException">Optional inner exception.</param>
    protected void AddError(
        List<ParseError> errors,
        string message,
        XmlReader reader,
        Exception? innerException = null)
    {
        var lineInfo = reader as IXmlLineInfo;
        errors.Add(new ParseError(
            message,
            GetCurrentPath(reader),
            lineInfo?.LineNumber,
            lineInfo?.LinePosition,
            innerException));
    }

    /// <summary>
    /// Adds a parse warning.
    /// </summary>
    /// <param name="warnings">The warning list.</param>
    /// <param name="message">The warning message.</param>
    /// <param name="reader">The XML reader.</param>
    protected void AddWarning(
        List<ParseWarning> warnings,
        string message,
        XmlReader reader)
    {
        var lineInfo = reader as IXmlLineInfo;
        warnings.Add(new ParseWarning(
            message,
            GetCurrentPath(reader),
            lineInfo?.LineNumber,
            lineInfo?.LinePosition));
    }

    #endregion

    #region Private Methods

    private async Task<MessageDetectionResult> DetectMessageTypeAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        return await _messageDetector.DetectWithDetailsAsync(stream, cancellationToken);
    }

    private void ValidateSupportedMessage(MessageIdentifier messageId)
    {
        if (!SupportedMessages.Contains(messageId))
        {
            throw new ParserNotFoundException(
                messageId,
                SupportedMessages);
        }
    }

    private async Task<BusinessApplicationHeader?> ParseApplicationHeaderAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        // Navigate to AppHdr
        if (!await MoveToElementAsync(reader, "AppHdr", cancellationToken))
            return null;

        // Parse BAH (simplified - full implementation in separate parser)
        var bahElement = await ReadSubtreeAsXElementAsync(reader, cancellationToken);
        if (bahElement is null)
            return null;

        // Extract basic fields
        var ns = bahElement.Name.Namespace;
        var msgDefIdrValue = bahElement.Element(ns + "MsgDefIdr")?.Value ?? string.Empty;

        MessageIdentifier msgDefIdr;
        if (!MessageIdentifier.TryParse(msgDefIdrValue, out var parsedMsgDefIdr, out _))
        {
            // Use a default if parsing fails
            msgDefIdr = new MessageIdentifier("head.001.001.01");
        }
        else
        {
            msgDefIdr = parsedMsgDefIdr!.Value;
        }

        return new BusinessApplicationHeader
        {
            BusinessMessageIdentifier = bahElement.Element(ns + "BizMsgIdr")?.Value ?? string.Empty,
            MessageDefinitionIdentifier = msgDefIdr,
            CreationDate = DateTime.TryParse(
                bahElement.Element(ns + "CreDt")?.Value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var dt) ? dt : DateTime.MinValue,
            From = new Party(),
            To = new Party()
        };
    }

    private static async Task NavigateToDocumentAsync(XmlReader reader, CancellationToken cancellationToken)
    {
        // Navigate to Document element or Message element
        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.LocalName == "Document")
                {
                    // Enter Document element
                    await reader.ReadAsync();
                    return;
                }

                // Already at Message element (e.g. when re-reading)
                return;
            }
        }
    }

    private static string GetCurrentPath(XmlReader reader)
    {
        // Simplified path determination
        return reader.LocalName;
    }

    private static XmlSchemaSet LoadSchemaSet(string schemaPath)
    {
        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, schemaPath);
        schemaSet.Compile();
        return schemaSet;
    }

    private static void ReportProgress(IProgress<ParseProgress>? progress, ParseProgress value)
    {
        progress?.Report(value);
    }

    #endregion
}
