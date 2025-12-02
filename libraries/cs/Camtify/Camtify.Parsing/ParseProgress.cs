namespace Camtify.Parsing;

/// <summary>
/// Represents progress information for parsing operations.
/// </summary>
public sealed record ParseProgress
{
    /// <summary>
    /// Gets the current status of the parsing operation.
    /// </summary>
    public required ParseStatus Status { get; init; }

    /// <summary>
    /// Gets the number of bytes read so far.
    /// </summary>
    public long? BytesRead { get; init; }

    /// <summary>
    /// Gets the total number of bytes to read (if known).
    /// </summary>
    public long? TotalBytes { get; init; }

    /// <summary>
    /// Gets the percentage completed (0-100).
    /// </summary>
    public double? PercentComplete =>
        TotalBytes.HasValue && BytesRead.HasValue && TotalBytes.Value > 0
            ? (double)BytesRead.Value / TotalBytes.Value * 100
            : null;

    /// <summary>
    /// Gets an optional message describing the current operation.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the number of entries parsed so far (for streaming parsers).
    /// </summary>
    public int? EntriesParsed { get; init; }
}
