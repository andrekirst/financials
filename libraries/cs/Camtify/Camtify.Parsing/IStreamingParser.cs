using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Interface for streaming parsers that process entries incrementally.
/// </summary>
/// <typeparam name="TEntry">The entry type (e.g., transaction, payment instruction).</typeparam>
/// <remarks>
/// Streaming parsers are useful for processing large files without loading the entire document into memory.
/// They yield entries one at a time, enabling memory-efficient processing.
/// </remarks>
public interface IStreamingParser<TEntry>
    where TEntry : class
{
    /// <summary>
    /// Gets the supported message identifiers.
    /// </summary>
    IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; }

    /// <summary>
    /// Streams entries from an ISO 20022 message.
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="Iso20022ParsingException">Thrown when parsing fails.</exception>
    IAsyncEnumerable<TEntry> ParseEntriesAsync(
        Stream stream,
        ParseOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses the message header and returns a streaming result with context.
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A streaming parse result with header and entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="Iso20022ParsingException">Thrown when parsing fails.</exception>
    Task<StreamingParseResult<TEntry>> ParseWithContextAsync(
        Stream stream,
        ParseOptions? options = null,
        CancellationToken cancellationToken = default);
}
