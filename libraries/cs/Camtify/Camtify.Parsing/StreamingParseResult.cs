using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Result of a streaming parse operation with header context.
/// </summary>
/// <typeparam name="TEntry">The entry type that will be streamed.</typeparam>
public sealed record StreamingParseResult<TEntry>
    where TEntry : class
{
    /// <summary>
    /// Gets the detected message identifier.
    /// </summary>
    public required MessageIdentifier MessageId { get; init; }

    /// <summary>
    /// Gets the parsed header (e.g., GroupHeader or StatementHeader).
    /// </summary>
    public required object Header { get; init; }

    /// <summary>
    /// Gets the Business Application Header (if present).
    /// </summary>
    public BusinessApplicationHeader? ApplicationHeader { get; init; }

    /// <summary>
    /// Gets the expected number of entries (if specified in header).
    /// </summary>
    public int? ExpectedEntryCount { get; init; }

    /// <summary>
    /// Gets the async enumerable of entries to stream.
    /// </summary>
    public required IAsyncEnumerable<TEntry> Entries { get; init; }
}
