using System.Xml;
using System.Xml.Schema;

namespace Camtify.Core.Parsing;

/// <summary>
/// Factory for securely configured XmlReader instances.
/// </summary>
public interface IXmlReaderFactory
{
    /// <summary>
    /// Creates an XmlReader with default settings.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <returns>Configured XmlReader</returns>
    XmlReader Create(Stream stream);

    /// <summary>
    /// Creates an XmlReader with custom settings.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="settings">Custom settings</param>
    /// <returns>Configured XmlReader</returns>
    XmlReader Create(Stream stream, XmlReaderSettings settings);

    /// <summary>
    /// Creates an XmlReader with validation.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="schemaSet">Schema set for validation</param>
    /// <param name="validationHandler">Handler for validation errors</param>
    /// <returns>Validating XmlReader</returns>
    XmlReader CreateValidating(
        Stream stream,
        XmlSchemaSet schemaSet,
        ValidationEventHandler? validationHandler = null);

    /// <summary>
    /// Default settings for all readers.
    /// </summary>
    XmlReaderSettings DefaultSettings { get; }

    /// <summary>
    /// Settings for validation.
    /// </summary>
    XmlReaderSettings ValidatingSettings { get; }

    /// <summary>
    /// Settings for large files.
    /// </summary>
    XmlReaderSettings LargeFileSettings { get; }
}
