using System.Text;
using System.Xml;
using System.Xml.Schema;
using Camtify.Core.Parsing;
using Microsoft.Extensions.Logging;

namespace Camtify.Parsing;

/// <summary>
/// Standard implementation of the XmlReader factory.
/// Configures XmlReader with security best practices.
/// </summary>
public sealed class Iso20022XmlReaderFactory : IXmlReaderFactory
{
    private readonly ILogger<Iso20022XmlReaderFactory>? _logger;

    /// <summary>
    /// Default settings for all readers.
    /// </summary>
    /// <remarks>
    /// Security measures:
    /// - XXE Prevention: XmlResolver = null
    /// - DTD Prevention: DtdProcessing = Prohibit
    /// - Entity Limit: MaxCharactersFromEntities = 1024
    ///
    /// Performance:
    /// - Async = true for async/await support
    /// - IgnoreWhitespace = true for fewer nodes
    /// - IgnoreComments = true for fewer nodes.
    /// </remarks>
    public XmlReaderSettings DefaultSettings { get; } = new()
    {
        // Security
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null,
        MaxCharactersFromEntities = 1024,

        // Performance
        Async = true,
        IgnoreWhitespace = true,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,

        // No limit for normal files
        MaxCharactersInDocument = 0,

        // Conformance
        CheckCharacters = true,
        ConformanceLevel = ConformanceLevel.Document
    };

    /// <summary>
    /// Settings for validation.
    /// </summary>
    public XmlReaderSettings ValidatingSettings
    {
        get
        {
            var settings = DefaultSettings.Clone();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags =
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.ReportValidationWarnings;
            return settings;
        }
    }

    /// <summary>
    /// Settings for large files.
    /// </summary>
    /// <remarks>
    /// Increased limits for GB-sized files.
    /// Entity limits remain for security reasons.
    /// </remarks>
    public XmlReaderSettings LargeFileSettings
    {
        get
        {
            var settings = DefaultSettings.Clone();
            settings.MaxCharactersInDocument = 0; // Unlimited
            return settings;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022XmlReaderFactory"/> class.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public Iso20022XmlReaderFactory(ILogger<Iso20022XmlReaderFactory>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates an XmlReader with default settings.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>Configured XmlReader.</returns>
    public XmlReader Create(Stream stream)
    {
        return Create(stream, DefaultSettings);
    }

    /// <summary>
    /// Creates an XmlReader with custom settings.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="settings">Custom settings.</param>
    /// <returns>Configured XmlReader.</returns>
    public XmlReader Create(Stream stream, XmlReaderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(settings);

        // Security validation
        ValidateSecuritySettings(settings);

        _logger?.LogDebug("XmlReader created with Async={Async}, DTD={DtdProcessing}", settings.Async, settings.DtdProcessing);

        return XmlReader.Create(stream, settings);
    }

    /// <summary>
    /// Creates an XmlReader with validation.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="schemaSet">Schema set for validation.</param>
    /// <param name="validationHandler">Handler for validation errors.</param>
    /// <returns>Validating XmlReader.</returns>
    public XmlReader CreateValidating(
        Stream stream,
        XmlSchemaSet schemaSet,
        ValidationEventHandler? validationHandler = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(schemaSet);

        var settings = ValidatingSettings.Clone();
        settings.Schemas = schemaSet;

        if (validationHandler is not null)
        {
            settings.ValidationEventHandler += validationHandler;
        }

        _logger?.LogDebug("Validating XmlReader created with {SchemaCount} schemas", schemaSet.Count);

        return XmlReader.Create(stream, settings);
    }

    /// <summary>
    /// Creates a reader for a TextReader (e.g. StringReader).
    /// </summary>
    /// <param name="textReader">The text reader to read from.</param>
    /// <returns>Configured XmlReader.</returns>
    public XmlReader CreateFromTextReader(TextReader textReader)
    {
        ArgumentNullException.ThrowIfNull(textReader);
        return XmlReader.Create(textReader, DefaultSettings);
    }

    /// <summary>
    /// Creates a reader with namespace manager.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="namespaceManager">Namespace manager for context.</param>
    /// <returns>Configured XmlReader.</returns>
    public XmlReader CreateWithNamespaceManager(
        Stream stream,
        XmlNamespaceManager namespaceManager)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(namespaceManager);

        var context = new XmlParserContext(null, namespaceManager, null, XmlSpace.Default);
        return XmlReader.Create(stream, DefaultSettings, context);
    }

    private void ValidateSecuritySettings(XmlReaderSettings settings)
    {
        // Warning if unsafe settings
        if (settings.DtdProcessing == DtdProcessing.Parse)
        {
            _logger?.LogWarning("DTD Processing is enabled. This can be a security risk.");
        }

        // Note: XmlResolver is write-only in .NET 9.0, so we can't check it here.
        // However, our DefaultSettings already set it to null for security.
    }
}
