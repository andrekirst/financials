using System.Reflection;
using System.Xml;

namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Factory for creating version-specific pain.001 parsers with automatic version detection.
/// </summary>
/// <remarks>
/// Automatically discovers all pain.001 parser implementations via reflection and
/// instantiates the appropriate parser based on XML namespace detection.
/// New parser versions are automatically registered when decorated with Pain001VersionAttribute.
/// </remarks>
public static class Pain001ParserFactory
{
    private static readonly Dictionary<string, ParserMetadata> ParsersByNamespace;
    private static readonly Dictionary<string, ParserMetadata> ParsersByVersion;

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
