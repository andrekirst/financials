namespace Camtify.Core;

/// <summary>
/// Configuration container for namespace mappings.
/// </summary>
internal sealed class NamespaceConfig
{
    /// <summary>
    /// Gets or sets the list of namespace mappings.
    /// </summary>
    public List<NamespaceMapping>? Mappings { get; set; }
}
