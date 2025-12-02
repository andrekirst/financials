using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Detailed result of message detection.
/// </summary>
public sealed record MessageDetectionResult
{
    /// <summary>
    /// Gets the detected message identifier.
    /// </summary>
    public required MessageIdentifier MessageId { get; init; }

    /// <summary>
    /// Gets the full namespace URI.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// Gets a value indicating whether the document has a Business Application Header.
    /// </summary>
    public bool HasApplicationHeader { get; init; }

    /// <summary>
    /// Gets the Business Application Header message identifier (if present).
    /// </summary>
    public MessageIdentifier? ApplicationHeaderMessageId { get; init; }

    /// <summary>
    /// Gets the MsgDefIdr from the Business Application Header (if present).
    /// </summary>
    public string? MessageDefinitionIdentifier { get; init; }

    /// <summary>
    /// Gets the root element name (e.g., "Document", "BizMsgEnvlp").
    /// </summary>
    public required string RootElementName { get; init; }

    /// <summary>
    /// Gets the message element name (e.g., "CstmrCdtTrfInitn").
    /// </summary>
    public string? MessageElementName { get; init; }

    /// <summary>
    /// Gets the detected variant.
    /// </summary>
    public MessageVariant Variant { get; init; }
}
