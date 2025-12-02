namespace Camtify.Core;

/// <summary>
/// Represents a mapping between a namespace and a message identifier.
/// </summary>
internal sealed class NamespaceMapping
{
    /// <summary>
    /// Gets or sets the XML namespace.
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    public string MessageId { get; set; } = string.Empty;
}
