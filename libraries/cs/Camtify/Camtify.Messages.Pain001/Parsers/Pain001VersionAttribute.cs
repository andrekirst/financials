namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Metadata attribute for pain.001 parser versions.
/// </summary>
/// <remarks>
/// Used for automatic parser discovery and registration in Pain001ParserFactory.
/// Decorate each parser implementation with this attribute to specify its version and namespace.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class Pain001VersionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the Pain001VersionAttribute class.
    /// </summary>
    /// <param name="version">The pain.001 version identifier (e.g., "003", "009", "011").</param>
    /// <param name="namespace">The XML namespace URI for this version.</param>
    public Pain001VersionAttribute(string version, string @namespace)
    {
        Version = version;
        Namespace = @namespace;
    }

    /// <summary>
    /// Gets the pain.001 version identifier.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the XML namespace URI for this parser version.
    /// </summary>
    public string Namespace { get; }
}
