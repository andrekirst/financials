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
{
    /// <summary>
    /// Gets the message identifier that this parser supports.
    /// </summary>
    MessageIdentifier MessageIdentifier { get; }

    /// <summary>
    /// Streams entries from an ISO 20022 message.
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when parsing fails.</exception>
    IAsyncEnumerable<TEntry> StreamEntriesAsync(Stream stream, CancellationToken cancellationToken = default);
}
