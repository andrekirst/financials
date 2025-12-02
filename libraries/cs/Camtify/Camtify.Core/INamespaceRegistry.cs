namespace Camtify.Core;

/// <summary>
/// Registry for ISO 20022 namespaces.
/// </summary>
public interface INamespaceRegistry
{
    /// <summary>
    /// Searches for the MessageIdentifier for a namespace.
    /// </summary>
    /// <param name="namespaceUri">The XML namespace.</param>
    /// <returns>The MessageIdentifier or null if not found.</returns>
    MessageIdentifier? GetMessageIdentifier(string namespaceUri);

    /// <summary>
    /// Searches for the default namespace for a MessageIdentifier.
    /// </summary>
    /// <param name="messageId">The MessageIdentifier.</param>
    /// <returns>The namespace or null if not found.</returns>
    string? GetNamespace(MessageIdentifier messageId);

    /// <summary>
    /// Returns all registered namespaces for a MessageIdentifier.
    /// </summary>
    /// <param name="messageId">The MessageIdentifier.</param>
    /// <returns>All known namespaces (ISO, SWIFT, etc.).</returns>
    IReadOnlyCollection<string> GetAllNamespaces(MessageIdentifier messageId);

    /// <summary>
    /// Checks if a namespace is known.
    /// </summary>
    /// <param name="namespaceUri">The XML namespace to check.</param>
    /// <returns>True if the namespace is known, false otherwise.</returns>
    bool IsKnownNamespace(string namespaceUri);

    /// <summary>
    /// Checks if a MessageIdentifier is known.
    /// </summary>
    /// <param name="messageId">The MessageIdentifier to check.</param>
    /// <returns>True if the MessageIdentifier is known, false otherwise.</returns>
    bool IsKnownMessage(MessageIdentifier messageId);

    /// <summary>
    /// Registers a new namespace.
    /// </summary>
    /// <param name="namespaceUri">The XML namespace to register.</param>
    /// <param name="messageId">The associated MessageIdentifier.</param>
    void Register(string namespaceUri, MessageIdentifier messageId);

    /// <summary>
    /// Gets all known MessageIdentifiers.
    /// </summary>
    IReadOnlyCollection<MessageIdentifier> KnownMessages { get; }

    /// <summary>
    /// Gets all known namespaces.
    /// </summary>
    IReadOnlyCollection<string> KnownNamespaces { get; }
}
