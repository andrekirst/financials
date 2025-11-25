namespace Camtify.Core;

/// <summary>
/// Marker interface for messages that support entry-level streaming.
/// Enables processing of millions of entries with constant memory usage.
/// </summary>
/// <typeparam name="TEntry">The type of individual entries (e.g., Entry, Transaction)</typeparam>
/// <remarks>
/// This interface enables streaming processing of large ISO 20022 messages
/// without loading the entire message into memory.
///
/// Use cases:
/// - Processing large bank statements (camt.053) with millions of entries
/// - Handling bulk payment files (pain.001) with thousands of transactions
/// - Real-time processing of payment confirmations (pacs.002)
///
/// Implementation notes:
/// - The implementation should use yield return to provide entries lazily
/// - Memory usage should remain constant regardless of message size
/// - The underlying XML reader should be configured for streaming
/// - Cancellation tokens should be respected for graceful shutdown
/// </remarks>
/// <example>
/// <code>
/// public class BankToCustomerStatement : IIso20022Message, IStreamable&lt;Entry&gt;
/// {
///     private readonly Stream _xmlStream;
///
///     public async IAsyncEnumerable&lt;Entry&gt; GetEntriesAsync(
///         [EnumeratorCancellation] CancellationToken cancellationToken = default)
///     {
///         await using var reader = XmlReader.Create(_xmlStream, new XmlReaderSettings
///         {
///             Async = true,
///             IgnoreWhitespace = true
///         });
///
///         while (await reader.ReadAsync() &amp;&amp; !cancellationToken.IsCancellationRequested)
///         {
///             if (reader.NodeType == XmlNodeType.Element &amp;&amp; reader.LocalName == "Ntry")
///             {
///                 yield return ParseEntry(reader);
///             }
///         }
///     }
/// }
///
/// // Usage:
/// await foreach (var entry in statement.GetEntriesAsync(cancellationToken))
/// {
///     await ProcessEntryAsync(entry);
/// }
/// </code>
/// </example>
public interface IStreamable<out TEntry>
{
    /// <summary>
    /// Returns the entries as an asynchronous stream.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop the streaming operation.</param>
    /// <returns>
    /// An asynchronous enumerable that yields entries one at a time.
    /// The enumeration can be cancelled using the provided cancellation token.
    /// </returns>
    /// <remarks>
    /// This method provides lazy evaluation of entries, meaning entries are only
    /// parsed and created as they are consumed by the caller.
    ///
    /// Memory characteristics:
    /// - Memory usage is constant (O(1)) relative to message size
    /// - Only one entry is in memory at a time
    /// - Large messages (GB+) can be processed on machines with limited RAM
    ///
    /// Performance characteristics:
    /// - First entry available quickly (low latency)
    /// - Total throughput depends on entry complexity and processing logic
    /// - Suitable for real-time processing pipelines
    ///
    /// Error handling:
    /// - XML parsing errors will throw exceptions during enumeration
    /// - The caller should handle exceptions appropriately
    /// - Partial results may have been processed before an error occurs
    /// </remarks>
    IAsyncEnumerable<TEntry> GetEntriesAsync(CancellationToken cancellationToken = default);
}
