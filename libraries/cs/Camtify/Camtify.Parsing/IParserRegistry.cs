using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Registry for parser registration.
/// Enables dynamic addition and removal of parsers.
/// </summary>
/// <remarks>
/// Primarily configured at application startup via DI.
/// Can also be extended at runtime (e.g., plugin systems).
/// </remarks>
public interface IParserRegistry
{
    /// <summary>
    /// Registers a parser for a message type.
    /// </summary>
    /// <typeparam name="TDocument">The document type.</typeparam>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="parserFactory">Factory function to create the parser.</param>
    /// <exception cref="ParserAlreadyRegisteredException">
    /// Thrown when a parser is already registered for this message type.
    /// </exception>
    void Register<TDocument>(
        MessageIdentifier messageId,
        Func<IIso20022Parser<TDocument>> parserFactory)
        where TDocument : class;

    /// <summary>
    /// Registers a parser (overwrites existing registration).
    /// </summary>
    /// <typeparam name="TDocument">The document type.</typeparam>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="parserFactory">Factory function to create the parser.</param>
    void RegisterOrReplace<TDocument>(
        MessageIdentifier messageId,
        Func<IIso20022Parser<TDocument>> parserFactory)
        where TDocument : class;

    /// <summary>
    /// Registers a streaming parser.
    /// </summary>
    /// <typeparam name="TEntry">The entry type.</typeparam>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="parserFactory">Factory function to create the streaming parser.</param>
    void RegisterStreaming<TEntry>(
        MessageIdentifier messageId,
        Func<IStreamingParser<TEntry>> parserFactory)
        where TEntry : class;

    /// <summary>
    /// Removes the registration for a message type.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>True if successfully removed.</returns>
    bool Unregister(MessageIdentifier messageId);

    /// <summary>
    /// Checks whether a parser is registered.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>True if a parser is registered.</returns>
    bool IsRegistered(MessageIdentifier messageId);

    /// <summary>
    /// All registered message identifiers.
    /// </summary>
    IReadOnlyCollection<MessageIdentifier> RegisteredMessages { get; }

    /// <summary>
    /// Gets information about a registered parser.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <returns>Parser registration information, or null if not registered.</returns>
    ParserRegistration? GetRegistration(MessageIdentifier messageId);
}
