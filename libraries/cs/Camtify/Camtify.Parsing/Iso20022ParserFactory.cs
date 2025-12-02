using System.Collections.Concurrent;
using Camtify.Core;
using Microsoft.Extensions.Logging;

namespace Camtify.Parsing;

/// <summary>
/// Standard implementation of the parser factory.
/// </summary>
/// <remarks>
/// This implementation uses a thread-safe registry to manage parser registrations.
/// It supports both regular and streaming parsers, and can automatically detect message types.
/// </remarks>
public class Iso20022ParserFactory : IParserFactory, IParserRegistry
{
    private readonly ConcurrentDictionary<MessageIdentifier, ParserRegistrationInternal> _parsers = new();
    private readonly IServiceProvider? _serviceProvider;
    private readonly IMessageDetector _messageDetector;
    private readonly ILogger<Iso20022ParserFactory>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022ParserFactory"/> class.
    /// </summary>
    /// <param name="messageDetector">The message detector for automatic message type detection.</param>
    /// <param name="serviceProvider">Optional service provider for DI-based parser creation.</param>
    /// <param name="logger">Optional logger for diagnostic information.</param>
    public Iso20022ParserFactory(
        IMessageDetector messageDetector,
        IServiceProvider? serviceProvider = null,
        ILogger<Iso20022ParserFactory>? logger = null)
    {
        _messageDetector = messageDetector ?? throw new ArgumentNullException(nameof(messageDetector));
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    #region IParserFactory Implementation

    /// <inheritdoc/>
    public IIso20022Parser<TDocument> CreateParser<TDocument>(MessageIdentifier messageId)
        where TDocument : class
    {
        if (!_parsers.TryGetValue(messageId, out var registration))
        {
            _logger?.LogError("No parser found for message type '{MessageId}'", messageId);
            throw new ParserNotFoundException(messageId, SupportedMessages);
        }

        var parser = registration.Factory();

        if (parser is not IIso20022Parser<TDocument> typedParser)
        {
            _logger?.LogError(
                "Parser type mismatch for '{MessageId}': expected {ExpectedType}, got {ActualType}",
                messageId,
                typeof(TDocument).Name,
                registration.DocumentType.Name);

            throw new ParserTypeMismatchException(typeof(TDocument), registration.DocumentType);
        }

        _logger?.LogDebug("Created parser for message type '{MessageId}'", messageId);
        return typedParser;
    }

    /// <inheritdoc/>
    public IStreamingParser<TEntry> CreateStreamingParser<TEntry>(MessageIdentifier messageId)
        where TEntry : class
    {
        if (!_parsers.TryGetValue(messageId, out var registration))
        {
            _logger?.LogError("No streaming parser found for message type '{MessageId}'", messageId);
            throw new ParserNotFoundException(messageId, SupportedMessages);
        }

        if (!registration.SupportsStreaming || registration.Factory() is not IStreamingParser<TEntry> streamingParser)
        {
            _logger?.LogError(
                "Parser for '{MessageId}' does not support streaming or type mismatch",
                messageId);

            throw new InvalidOperationException(
                $"Parser for '{messageId}' does not support streaming with entry type '{typeof(TEntry).Name}'.");
        }

        _logger?.LogDebug("Created streaming parser for message type '{MessageId}'", messageId);
        return streamingParser;
    }

    /// <inheritdoc/>
    public object CreateParser(MessageIdentifier messageId)
    {
        if (!_parsers.TryGetValue(messageId, out var registration))
        {
            _logger?.LogError("No parser found for message type '{MessageId}'", messageId);
            throw new ParserNotFoundException(messageId, SupportedMessages);
        }

        var parser = registration.Factory();
        _logger?.LogDebug("Created untyped parser for message type '{MessageId}'", messageId);
        return parser;
    }

    /// <inheritdoc/>
    public bool SupportsMessage(MessageIdentifier messageId)
    {
        return _parsers.ContainsKey(messageId);
    }

    /// <inheritdoc/>
    public bool SupportsBusinessArea(string businessArea)
    {
        return _parsers.Keys.Any(m => m.IsBusinessArea(businessArea));
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<MessageIdentifier> SupportedMessages =>
        _parsers.Keys.ToList().AsReadOnly();

    /// <inheritdoc/>
    public IReadOnlyCollection<MessageIdentifier> GetSupportedVersions(
        string businessArea,
        string messageType)
    {
        return _parsers.Keys
            .Where(m => m.IsBusinessArea(businessArea) && m.MessageNumber == messageType)
            .OrderByDescending(m => m.VersionNumber)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<(object Parser, MessageIdentifier MessageId)> DetectAndCreateParserAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var originalPosition = stream.CanSeek ? stream.Position : -1;

        try
        {
            _logger?.LogDebug("Detecting message type from stream");
            var messageId = await _messageDetector.DetectAsync(stream, cancellationToken);

            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }

            _logger?.LogInformation("Detected message type: {MessageId}", messageId);
            var parser = CreateParser(messageId);
            return (parser, messageId);
        }
        catch (Exception ex) when (ex is not Iso20022Exception)
        {
            _logger?.LogError(ex, "Error during message detection");
            throw new MessageDetectionException("Failed to detect message type from stream.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<(IIso20022Parser<TDocument> Parser, MessageIdentifier MessageId)>
        DetectAndCreateParserAsync<TDocument>(
            Stream stream,
            CancellationToken cancellationToken = default)
        where TDocument : class
    {
        var (parser, messageId) = await DetectAndCreateParserAsync(stream, cancellationToken);

        if (parser is not IIso20022Parser<TDocument> typedParser)
        {
            var actualType = parser.GetType()
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIso20022Parser<>))
                ?.GetGenericArguments()
                .FirstOrDefault() ?? typeof(object);

            throw new ParserTypeMismatchException(typeof(TDocument), actualType);
        }

        return (typedParser, messageId);
    }

    #endregion

    #region IParserRegistry Implementation

    /// <inheritdoc/>
    public void Register<TDocument>(
        MessageIdentifier messageId,
        Func<IIso20022Parser<TDocument>> parserFactory)
        where TDocument : class
    {
        ArgumentNullException.ThrowIfNull(parserFactory);

        var registration = new ParserRegistrationInternal
        {
            MessageId = messageId,
            DocumentType = typeof(TDocument),
            ParserType = typeof(IIso20022Parser<TDocument>),
            Factory = parserFactory,
            SupportsStreaming = false,
            RegisteredAt = DateTime.UtcNow
        };

        if (!_parsers.TryAdd(messageId, registration))
        {
            _logger?.LogWarning("Parser for '{MessageId}' is already registered", messageId);
            throw new ParserAlreadyRegisteredException(messageId);
        }

        _logger?.LogInformation(
            "Registered parser for '{MessageId}' with document type '{DocumentType}'",
            messageId,
            typeof(TDocument).Name);
    }

    /// <inheritdoc/>
    public void RegisterOrReplace<TDocument>(
        MessageIdentifier messageId,
        Func<IIso20022Parser<TDocument>> parserFactory)
        where TDocument : class
    {
        ArgumentNullException.ThrowIfNull(parserFactory);

        var registration = new ParserRegistrationInternal
        {
            MessageId = messageId,
            DocumentType = typeof(TDocument),
            ParserType = typeof(IIso20022Parser<TDocument>),
            Factory = parserFactory,
            SupportsStreaming = false,
            RegisteredAt = DateTime.UtcNow
        };

        _parsers[messageId] = registration;

        _logger?.LogInformation(
            "Registered/replaced parser for '{MessageId}' with document type '{DocumentType}'",
            messageId,
            typeof(TDocument).Name);
    }

    /// <inheritdoc/>
    public void RegisterStreaming<TEntry>(
        MessageIdentifier messageId,
        Func<IStreamingParser<TEntry>> parserFactory)
        where TEntry : class
    {
        ArgumentNullException.ThrowIfNull(parserFactory);

        var registration = new ParserRegistrationInternal
        {
            MessageId = messageId,
            DocumentType = typeof(TEntry),
            ParserType = typeof(IStreamingParser<TEntry>),
            Factory = parserFactory,
            SupportsStreaming = true,
            RegisteredAt = DateTime.UtcNow
        };

        _parsers[messageId] = registration;

        _logger?.LogInformation(
            "Registered streaming parser for '{MessageId}' with entry type '{EntryType}'",
            messageId,
            typeof(TEntry).Name);
    }

    /// <inheritdoc/>
    public bool Unregister(MessageIdentifier messageId)
    {
        var removed = _parsers.TryRemove(messageId, out _);

        if (removed)
        {
            _logger?.LogInformation("Unregistered parser for '{MessageId}'", messageId);
        }
        else
        {
            _logger?.LogWarning("Attempted to unregister non-existent parser for '{MessageId}'", messageId);
        }

        return removed;
    }

    /// <inheritdoc/>
    public bool IsRegistered(MessageIdentifier messageId)
    {
        return _parsers.ContainsKey(messageId);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<MessageIdentifier> RegisteredMessages =>
        _parsers.Keys.ToList().AsReadOnly();

    /// <inheritdoc/>
    public ParserRegistration? GetRegistration(MessageIdentifier messageId)
    {
        if (!_parsers.TryGetValue(messageId, out var internalReg))
        {
            return null;
        }

        return new ParserRegistration
        {
            MessageId = internalReg.MessageId,
            DocumentType = internalReg.DocumentType,
            ParserType = internalReg.ParserType,
            SupportsStreaming = internalReg.SupportsStreaming,
            RegisteredAt = internalReg.RegisteredAt
        };
    }

    #endregion

    /// <summary>
    /// Internal representation of a parser registration.
    /// </summary>
    private sealed class ParserRegistrationInternal
    {
        public required MessageIdentifier MessageId { get; init; }
        public required Type DocumentType { get; init; }
        public required Type ParserType { get; init; }
        public required Func<object> Factory { get; init; }
        public required bool SupportsStreaming { get; init; }
        public required DateTime RegisteredAt { get; init; }
    }
}
