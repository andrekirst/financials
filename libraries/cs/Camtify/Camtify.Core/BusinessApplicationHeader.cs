namespace Camtify.Core;

/// <summary>
/// Represents an optional Business Application Header (BAH) for ISO 20022 messages.
/// </summary>
/// <remarks>
/// The Business Application Header (head.001.001.xx) provides additional metadata
/// for message routing, identification, and processing instructions.
/// It is optional but recommended for end-to-end message tracking.
/// </remarks>
public sealed record BusinessApplicationHeader
{
    /// <summary>
    /// Gets the unique business message identifier.
    /// </summary>
    /// <remarks>
    /// Used for end-to-end tracking across multiple financial institutions.
    /// </remarks>
    public required string BusinessMessageIdentifier { get; init; }

    /// <summary>
    /// Gets the message definition identifier (e.g., "pain.001.001.09").
    /// </summary>
    public required MessageIdentifier MessageDefinitionIdentifier { get; init; }

    /// <summary>
    /// Gets the creation date and time of the header.
    /// </summary>
    public required DateTime CreationDate { get; init; }

    /// <summary>
    /// Gets information about the sender of the message.
    /// </summary>
    public Party? From { get; init; }

    /// <summary>
    /// Gets information about the receiver of the message.
    /// </summary>
    public Party? To { get; init; }

    /// <summary>
    /// Gets a value indicating whether the message is a copy, duplicate, or original.
    /// </summary>
    public bool? CopyDuplicate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the message is intended for production or testing.
    /// </summary>
    public bool? PossibleDuplicate { get; init; }
}
