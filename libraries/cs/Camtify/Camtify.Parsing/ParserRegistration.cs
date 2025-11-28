using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Information about a parser registration.
/// </summary>
public sealed record ParserRegistration
{
    /// <summary>
    /// The message identifier.
    /// </summary>
    public required MessageIdentifier MessageId { get; init; }

    /// <summary>
    /// The document type that the parser produces.
    /// </summary>
    public required Type DocumentType { get; init; }

    /// <summary>
    /// The parser type.
    /// </summary>
    public required Type ParserType { get; init; }

    /// <summary>
    /// Supports streaming?
    /// </summary>
    public bool SupportsStreaming { get; init; }

    /// <summary>
    /// Registration timestamp.
    /// </summary>
    public DateTime RegisteredAt { get; init; }
}
