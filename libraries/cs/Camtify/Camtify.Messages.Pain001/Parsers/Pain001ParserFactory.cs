using System.Reflection;
using System.Xml;
using Camtify.Core;
using Camtify.Messages.Pain001.Models.Pain001;
using Camtify.Parsing;

namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Factory for creating version-specific pain.001 parsers with automatic version detection.
/// </summary>
/// <remarks>
/// Automatically discovers all pain.001 parser implementations via reflection and
/// instantiates the appropriate parser based on XML namespace detection.
/// New parser versions are automatically registered when decorated with Pain001VersionAttribute.
/// </remarks>
public sealed class Pain001ParserFactory : IParserFactory
{
    // Static dictionaries for backward compatibility
    private static readonly Dictionary<string, ParserMetadata> ParsersByNamespace;
    private static readonly Dictionary<string, ParserMetadata> ParsersByVersion;

    // Instance-level dictionary for MessageIdentifier-based lookup
    private readonly Lazy<Dictionary<MessageIdentifier, ParserMetadata>> _parsersByMessageId;

    /// <summary>
    /// Static constructor that discovers all pain.001 parsers via reflection.
    /// </summary>
    static Pain001ParserFactory()
    {
        ParsersByNamespace = new Dictionary<string, ParserMetadata>();
        ParsersByVersion = new Dictionary<string, ParserMetadata>();

        DiscoverParsers();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Pain001ParserFactory"/> class.
    /// </summary>
    public Pain001ParserFactory()
    {
        _parsersByMessageId = new Lazy<Dictionary<MessageIdentifier, ParserMetadata>>(
            BuildMessageIdDictionary,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Builds the MessageIdentifier-based dictionary from static parsers.
    /// </summary>
    private static Dictionary<MessageIdentifier, ParserMetadata> BuildMessageIdDictionary()
    {
        var dictionary = new Dictionary<MessageIdentifier, ParserMetadata>();

        foreach (var (ns, metadata) in ParsersByNamespace)
        {
            var messageId = MessageIdentifier.FromNamespace(ns);
            dictionary[messageId] = metadata;
        }

        return dictionary;
    }

    #region IParserFactory Implementation

    /// <summary>
    /// Creates a typed parser for a specific message type.
    /// </summary>
    /// <remarks>
    /// Pain.001 parsers require a stream at construction time.
    /// Use <see cref="DetectAndCreateParserAsync{TDocument}"/> instead.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Always thrown because Pain.001 parsers require a stream at construction.
    /// </exception>
    public IIso20022Parser<TDocument> CreateParser<TDocument>(MessageIdentifier messageId)
        where TDocument : class
    {
        throw new NotSupportedException(
            "Pain.001 parsers require a stream at construction time. " +
            "Use DetectAndCreateParserAsync() or the static CreateAsync()/CreateForVersion() methods instead.");
    }

    /// <summary>
    /// Creates a streaming parser for entry-level streaming.
    /// </summary>
    /// <remarks>
    /// Pain.001 parsers require a stream at construction time.
    /// Use <see cref="DetectAndCreateParserAsync"/> instead.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Always thrown because Pain.001 parsers require a stream at construction.
    /// </exception>
    public IStreamingParser<TEntry> CreateStreamingParser<TEntry>(MessageIdentifier messageId)
        where TEntry : class
    {
        throw new NotSupportedException(
            "Pain.001 parsers require a stream at construction time. " +
            "Use DetectAndCreateParserAsync() or the static CreateAsync()/CreateForVersion() methods instead.");
    }

    /// <summary>
    /// Creates an untyped parser (for dynamic scenarios).
    /// </summary>
    /// <remarks>
    /// Pain.001 parsers require a stream at construction time.
    /// Use <see cref="DetectAndCreateParserAsync"/> instead.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Always thrown because Pain.001 parsers require a stream at construction.
    /// </exception>
    public object CreateParser(MessageIdentifier messageId)
    {
        throw new NotSupportedException(
            "Pain.001 parsers require a stream at construction time. " +
            "Use DetectAndCreateParserAsync() or the static CreateAsync()/CreateForVersion() methods instead.");
    }

    /// <summary>
    /// Checks whether a parser is available for the message type.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>True if supported.</returns>
    public bool SupportsMessage(MessageIdentifier messageId)
    {
        return _parsersByMessageId.Value.ContainsKey(messageId);
    }

    /// <summary>
    /// Checks whether a parser is available for the business area.
    /// </summary>
    /// <param name="businessArea">The business area (pain, camt, pacs, etc.).</param>
    /// <returns>True if at least one version is supported.</returns>
    public bool SupportsBusinessArea(string businessArea)
    {
        return _parsersByMessageId.Value.Keys
            .Any(msgId => msgId.BusinessArea.Equals(businessArea, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all supported message identifiers.
    /// </summary>
    public IReadOnlyCollection<MessageIdentifier> SupportedMessages =>
        _parsersByMessageId.Value.Keys.ToList().AsReadOnly();

    /// <summary>
    /// Gets all supported versions for a message type.
    /// </summary>
    /// <param name="businessArea">Business area (e.g., "pain").</param>
    /// <param name="messageType">Message type (e.g., "001").</param>
    /// <returns>List of supported versions.</returns>
    public IReadOnlyCollection<MessageIdentifier> GetSupportedVersions(
        string businessArea,
        string messageType)
    {
        return _parsersByMessageId.Value.Keys
            .Where(msgId =>
                msgId.BusinessArea.Equals(businessArea, StringComparison.OrdinalIgnoreCase) &&
                msgId.MessageNumber.Equals(messageType, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Detects the message type from a stream and creates the appropriate parser.
    /// </summary>
    /// <param name="stream">The XML stream (position will be reset).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of parser and detected message identifier.</returns>
    /// <exception cref="ArgumentNullException">Thrown if stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not readable or seekable.</exception>
    /// <exception cref="MessageDetectionException">
    /// Thrown when the message type cannot be detected.
    /// </exception>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no parser is registered for the detected message type.
    /// </exception>
    public async Task<(object Parser, MessageIdentifier MessageId)> DetectAndCreateParserAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(stream));
        }

        if (!stream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable for automatic version detection.", nameof(stream));
        }

        try
        {
            var detectedNamespace = await DetectNamespaceAsync(stream, cancellationToken);

            if (!ParsersByNamespace.TryGetValue(detectedNamespace, out var metadata))
            {
                var messageId = MessageIdentifier.FromNamespace(detectedNamespace);
                throw new ParserNotFoundException(messageId, SupportedMessages);
            }

            var parser = CreateParserInstance(metadata.ParserType, stream, leaveOpen: false, cacheGroupHeader: true);
            var messageIdResult = MessageIdentifier.FromNamespace(detectedNamespace);

            return (parser, messageIdResult);
        }
        catch (ArgumentException ex)
        {
            throw new MessageDetectionException("Failed to detect message type from stream.", ex);
        }
    }

    /// <summary>
    /// Detects the message type and creates a typed parser.
    /// </summary>
    /// <typeparam name="TDocument">The expected document type.</typeparam>
    /// <param name="stream">The XML stream (position will be reset).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of parser and detected message identifier.</returns>
    /// <exception cref="ArgumentNullException">Thrown if stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not readable or seekable.</exception>
    /// <exception cref="MessageDetectionException">
    /// Thrown when the message type cannot be detected.
    /// </exception>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no parser is registered for the detected message type.
    /// </exception>
    /// <exception cref="ParserTypeMismatchException">
    /// Thrown when the registered parser does not produce the expected document type.
    /// </exception>
    public async Task<(IIso20022Parser<TDocument> Parser, MessageIdentifier MessageId)>
        DetectAndCreateParserAsync<TDocument>(
            Stream stream,
            CancellationToken cancellationToken = default)
        where TDocument : class
    {
        var (parser, messageId) = await DetectAndCreateParserAsync(stream, cancellationToken);

        if (parser is not IIso20022Parser<TDocument> typedParser)
        {
            throw new ParserTypeMismatchException(typeof(TDocument), parser.GetType());
        }

        return (typedParser, messageId);
    }

    #endregion

    /// <summary>
    /// Discovers all types implementing IPain001Parser with Pain001VersionAttribute.
    /// </summary>
    private static void DiscoverParsers()
    {
        var assembly = typeof(Pain001ParserFactory).Assembly;
        var parserTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IPain001Parser).IsAssignableFrom(t))
            .ToList();

        foreach (var parserType in parserTypes)
        {
            var attribute = parserType.GetCustomAttribute<Pain001VersionAttribute>();
            if (attribute == null)
            {
                continue;
            }

            var metadata = new ParserMetadata
            {
                ParserType = parserType,
                Version = attribute.Version,
                Namespace = attribute.Namespace
            };

            ParsersByNamespace[attribute.Namespace] = metadata;
            ParsersByVersion[attribute.Version] = metadata;
        }
    }

    /// <summary>
    /// Creates a version-specific parser based on automatic namespace detection.
    /// </summary>
    /// <param name="xmlStream">The XML stream to parse. Must be readable.</param>
    /// <param name="leaveOpen">If true, the stream will not be disposed after parsing.</param>
    /// <param name="cacheGroupHeader">If true, the GroupHeader will be cached during streaming.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A version-specific Pain001 parser instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if xmlStream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not readable or seekable.</exception>
    /// <exception cref="NotSupportedException">Thrown if the detected version is not supported.</exception>
    /// <remarks>
    /// The stream must be seekable for version detection. After detection, the stream position
    /// is reset to the beginning for parsing. If you already know the version, consider
    /// instantiating the parser directly (e.g., new Pain001v09Parser(...)).
    /// </remarks>
    public static async Task<IPain001Parser> CreateAsync(
        Stream xmlStream,
        bool leaveOpen = false,
        bool cacheGroupHeader = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(xmlStream);

        if (!xmlStream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(xmlStream));
        }

        if (!xmlStream.CanSeek)
        {
            throw new ArgumentException(
                "Stream must be seekable for automatic version detection. " +
                "If you know the version, instantiate the parser directly (e.g., new Pain001v09Parser(...)).",
                nameof(xmlStream));
        }

        var detectedNamespace = await DetectNamespaceAsync(xmlStream, cancellationToken);

        if (!ParsersByNamespace.TryGetValue(detectedNamespace, out var metadata))
        {
            throw new NotSupportedException(
                $"Unsupported pain.001 namespace: {detectedNamespace}. " +
                $"Supported versions: {string.Join(", ", GetSupportedVersions())}");
        }

        return CreateParserInstance(metadata.ParserType, xmlStream, leaveOpen, cacheGroupHeader);
    }

    /// <summary>
    /// Creates a parser instance for a specific version.
    /// </summary>
    /// <param name="version">The version identifier (e.g., "003", "009", "011").</param>
    /// <param name="xmlStream">The XML stream to parse. Must be readable.</param>
    /// <param name="leaveOpen">If true, the stream will not be disposed after parsing.</param>
    /// <param name="cacheGroupHeader">If true, the GroupHeader will be cached during streaming.</param>
    /// <returns>A version-specific Pain001 parser instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if xmlStream or version is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not readable.</exception>
    /// <exception cref="NotSupportedException">Thrown if the version is not supported.</exception>
    public static IPain001Parser CreateForVersion(
        string version,
        Stream xmlStream,
        bool leaveOpen = false,
        bool cacheGroupHeader = true)
    {
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(xmlStream);

        if (!xmlStream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(xmlStream));
        }

        if (!ParsersByVersion.TryGetValue(version, out var metadata))
        {
            throw new NotSupportedException(
                $"Unsupported pain.001 version: {version}. " +
                $"Supported versions: {string.Join(", ", GetSupportedVersions())}");
        }

        return CreateParserInstance(metadata.ParserType, xmlStream, leaveOpen, cacheGroupHeader);
    }

    /// <summary>
    /// Creates a parser instance using reflection.
    /// </summary>
    private static IPain001Parser CreateParserInstance(
        Type parserType,
        Stream xmlStream,
        bool leaveOpen,
        bool cacheGroupHeader)
    {
        var instance = Activator.CreateInstance(parserType, xmlStream, leaveOpen, cacheGroupHeader);

        if (instance is not IPain001Parser parser)
        {
            throw new InvalidOperationException(
                $"Failed to create parser instance of type {parserType.Name}.");
        }

        return parser;
    }

    /// <summary>
    /// Detects the XML namespace from the Document element.
    /// </summary>
    /// <param name="xmlStream">The XML stream. Must be seekable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The detected namespace URI.</returns>
    /// <exception cref="ArgumentNullException">Thrown if xmlStream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not seekable or Document element not found.</exception>
    /// <remarks>
    /// This method reads only the Document element's namespace attribute and then resets
    /// the stream position to the beginning. It's optimized for minimal I/O operations.
    /// </remarks>
    private static async Task<string> DetectNamespaceAsync(
        Stream xmlStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(xmlStream);

        if (!xmlStream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable for namespace detection.", nameof(xmlStream));
        }

        var originalPosition = xmlStream.Position;

        try
        {
            // Reset to beginning
            xmlStream.Position = 0;

            var settings = new XmlReaderSettings
            {
                Async = true,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                CloseInput = false,
                DtdProcessing = DtdProcessing.Prohibit,
                ValidationType = ValidationType.None
            };

            using var reader = XmlReader.Create(xmlStream, settings);

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Document")
                {
                    var ns = reader.NamespaceURI;

                    if (string.IsNullOrWhiteSpace(ns))
                    {
                        throw new ArgumentException("Document element has no namespace.");
                    }

                    return ns;
                }
            }

            throw new ArgumentException("Document element not found in XML.");
        }
        finally
        {
            // Reset stream position to original
            xmlStream.Position = originalPosition;
        }
    }

    /// <summary>
    /// Detects the pain.001 version from the XML namespace without full parsing.
    /// </summary>
    /// <param name="xmlStream">The XML stream. Must be seekable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The detected version identifier (e.g., "003", "009", "011").</returns>
    /// <exception cref="ArgumentNullException">Thrown if xmlStream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not seekable or Document element not found.</exception>
    /// <exception cref="NotSupportedException">Thrown if the namespace is not recognized.</exception>
    /// <remarks>
    /// This method reads only the Document element's namespace attribute and then resets
    /// the stream position to the beginning. It's optimized for minimal I/O operations.
    /// </remarks>
    public static async Task<string> DetectVersionAsync(
        Stream xmlStream,
        CancellationToken cancellationToken = default)
    {
        var detectedNamespace = await DetectNamespaceAsync(xmlStream, cancellationToken);

        if (!ParsersByNamespace.TryGetValue(detectedNamespace, out var metadata))
        {
            throw new NotSupportedException(
                $"Unsupported pain.001 namespace: {detectedNamespace}. " +
                $"Supported versions: {string.Join(", ", GetSupportedVersions())}");
        }

        return metadata.Version;
    }

    /// <summary>
    /// Checks if a specific pain.001 version is supported by the factory.
    /// </summary>
    /// <param name="version">The version identifier to check (e.g., "003", "009", "011").</param>
    /// <returns>True if the version is supported, false otherwise.</returns>
    public static bool IsVersionSupported(string version)
    {
        return ParsersByVersion.ContainsKey(version);
    }

    /// <summary>
    /// Checks if a specific pain.001 namespace is supported by the factory.
    /// </summary>
    /// <param name="namespace">The namespace URI to check.</param>
    /// <returns>True if the namespace is supported, false otherwise.</returns>
    public static bool IsNamespaceSupported(string @namespace)
    {
        return ParsersByNamespace.ContainsKey(@namespace);
    }

    /// <summary>
    /// Gets the version identifier from a namespace URI.
    /// </summary>
    /// <param name="namespace">The namespace URI.</param>
    /// <returns>The version identifier if found; null otherwise.</returns>
    public static string? GetVersionFromNamespace(string @namespace)
    {
        return ParsersByNamespace.TryGetValue(@namespace, out var metadata) ? metadata.Version : null;
    }

    /// <summary>
    /// Gets the namespace URI from a version identifier.
    /// </summary>
    /// <param name="version">The version identifier (e.g., "003", "009").</param>
    /// <returns>The namespace URI if found; null otherwise.</returns>
    public static string? GetNamespaceFromVersion(string version)
    {
        return ParsersByVersion.TryGetValue(version, out var metadata) ? metadata.Namespace : null;
    }

    /// <summary>
    /// Gets all supported pain.001 versions.
    /// </summary>
    /// <returns>A read-only collection of supported version identifiers.</returns>
    public static IReadOnlyCollection<string> GetSupportedVersions()
    {
        return ParsersByVersion.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets all supported pain.001 namespaces.
    /// </summary>
    /// <returns>A read-only collection of supported namespace URIs.</returns>
    public static IReadOnlyCollection<string> GetSupportedNamespaces()
    {
        return ParsersByNamespace.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// Internal metadata for discovered parsers.
    /// </summary>
    private sealed class ParserMetadata
    {
        public required Type ParserType { get; init; }
        public required string Version { get; init; }
        public required string Namespace { get; init; }
    }
}
