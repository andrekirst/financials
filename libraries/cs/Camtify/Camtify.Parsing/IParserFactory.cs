using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Factory for ISO 20022 parsers.
/// Central entry point for creating parsers.
/// </summary>
/// <remarks>
/// The factory automatically selects the appropriate parser based on the message identifier.
/// Parsers can be registered via Dependency Injection or manually.
/// </remarks>
/// <example>
/// <code>
/// // Via DI
/// public class PaymentService
/// {
///     private readonly IParserFactory _parserFactory;
///
///     public PaymentService(IParserFactory parserFactory)
///     {
///         _parserFactory = parserFactory;
///     }
///
///     public async Task ProcessPaymentFileAsync(string filePath, CancellationToken cancellationToken = default)
///     {
///         await using var stream = File.OpenRead(filePath);
///
///         // Auto-detect message type
///         var (parser, messageId) = await _parserFactory.DetectAndCreateParserAsync(stream, cancellationToken);
///         var document = await parser.ParseAsync(stream, cancellationToken);
///
///         // Or with known type
///         var pain001Parser = _parserFactory.CreateParser&lt;Pain001Document&gt;(
///             MessageIdentifier.Pain.V001_09);
///     }
/// }
/// </code>
/// </example>
public interface IParserFactory
{
    /// <summary>
    /// Creates a typed parser for a specific message type.
    /// </summary>
    /// <typeparam name="TDocument">The expected document type.</typeparam>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>The parser for this message type.</returns>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no parser is registered for this message type.
    /// </exception>
    /// <exception cref="ParserTypeMismatchException">
    /// Thrown when the registered parser does not produce the expected document type.
    /// </exception>
    IIso20022Parser<TDocument> CreateParser<TDocument>(MessageIdentifier messageId)
        where TDocument : class;

    /// <summary>
    /// Creates a streaming parser for entry-level streaming.
    /// </summary>
    /// <typeparam name="TEntry">The entry type (e.g., CamtEntry, PainTransaction).</typeparam>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>The streaming parser.</returns>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no streaming parser is registered for this message type.
    /// </exception>
    IStreamingParser<TEntry> CreateStreamingParser<TEntry>(MessageIdentifier messageId);

    /// <summary>
    /// Creates an untyped parser (for dynamic scenarios).
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>The parser as object.</returns>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no parser is registered for this message type.
    /// </exception>
    object CreateParser(MessageIdentifier messageId);

    /// <summary>
    /// Checks whether a parser is available for the message type.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>True if supported.</returns>
    bool SupportsMessage(MessageIdentifier messageId);

    /// <summary>
    /// Checks whether a parser is available for the business area.
    /// </summary>
    /// <param name="businessArea">The business area (pain, camt, pacs, etc.).</param>
    /// <returns>True if at least one version is supported.</returns>
    bool SupportsBusinessArea(string businessArea);

    /// <summary>
    /// Gets all supported message identifiers.
    /// </summary>
    IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; }

    /// <summary>
    /// Gets all supported versions for a message type.
    /// </summary>
    /// <param name="businessArea">Business area (e.g., "pain").</param>
    /// <param name="messageType">Message type (e.g., "001").</param>
    /// <returns>List of supported versions.</returns>
    IReadOnlyCollection<MessageIdentifier> GetSupportedVersions(
        string businessArea,
        string messageType);

    /// <summary>
    /// Detects the message type from a stream and creates the appropriate parser.
    /// </summary>
    /// <param name="stream">The XML stream (position will be reset).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of parser and detected message identifier.</returns>
    /// <exception cref="MessageDetectionException">
    /// Thrown when the message type cannot be detected.
    /// </exception>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no parser is registered for the detected message type.
    /// </exception>
    Task<(object Parser, MessageIdentifier MessageId)> DetectAndCreateParserAsync(
        Stream stream,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects the message type and creates a typed parser.
    /// </summary>
    /// <typeparam name="TDocument">The expected document type.</typeparam>
    /// <param name="stream">The XML stream (position will be reset).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of parser and detected message identifier.</returns>
    /// <exception cref="MessageDetectionException">
    /// Thrown when the message type cannot be detected.
    /// </exception>
    /// <exception cref="ParserNotFoundException">
    /// Thrown when no parser is registered for the detected message type.
    /// </exception>
    /// <exception cref="ParserTypeMismatchException">
    /// Thrown when the registered parser does not produce the expected document type.
    /// </exception>
    Task<(IIso20022Parser<TDocument> Parser, MessageIdentifier MessageId)>
        DetectAndCreateParserAsync<TDocument>(
            Stream stream,
            CancellationToken cancellationToken = default)
        where TDocument : class;
}
