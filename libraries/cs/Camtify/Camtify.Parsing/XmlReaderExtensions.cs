using System.Text;
using System.Xml;
using Camtify.Core.Parsing;

namespace Camtify.Parsing;

/// <summary>
/// Extension methods for XmlReader creation.
/// </summary>
public static class XmlReaderExtensions
{
    /// <summary>
    /// Creates a secure XmlReader for a file.
    /// </summary>
    /// <param name="factory">The XmlReader factory</param>
    /// <param name="filePath">Path to the XML file</param>
    /// <returns>Configured XmlReader</returns>
    public static XmlReader CreateSecureReader(this IXmlReaderFactory factory, string filePath)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 8192,
            useAsync: true);

        return factory.Create(stream);
    }

    /// <summary>
    /// Creates a secure XmlReader for a string.
    /// </summary>
    /// <param name="factory">The XmlReader factory</param>
    /// <param name="xml">XML content as string</param>
    /// <returns>Configured XmlReader</returns>
    public static XmlReader CreateFromString(this IXmlReaderFactory factory, string xml)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return factory.Create(stream);
    }
}
