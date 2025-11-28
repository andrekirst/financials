using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Base interface for all ISO 20022 parsers.
/// </summary>
/// <typeparam name="TDocument">The document type that this parser produces.</typeparam>
/// <remarks>
/// This interface provides the foundation for all ISO 20022 parsers in the Camtify library.
/// Implementations should handle XML parsing and validation for specific message types.
/// </remarks>
public interface IIso20022Parser<TDocument> where TDocument : class
{
    /// <summary>
    /// Gets the message identifier that this parser supports.
    /// </summary>
    MessageIdentifier MessageIdentifier { get; }

    /// <summary>
    /// Gets the version identifier with leading zeros (e.g., "003", "009", "011").
    /// </summary>
    /// <remarks>
    /// Default implementation uses MessageIdentifier.VersionPadded.
    /// Implementations can override if custom format is needed.
    /// </remarks>
    string Version => MessageIdentifier.VersionPadded;

    /// <summary>
    /// Gets the XML namespace URI for this parser version.
    /// </summary>
    /// <remarks>
    /// Default implementation uses MessageIdentifier.ToNamespace().
    /// Implementations can override if custom namespace format is needed.
    /// </remarks>
    string Namespace => MessageIdentifier.ToNamespace();

    /// <summary>
    /// Parses an ISO 20022 message from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed document.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when parsing fails.</exception>
    Task<TDocument> ParseAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates whether the parser can handle the given stream.
    /// </summary>
    /// <param name="stream">The stream to validate. Position will be reset after validation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the parser can handle this stream; otherwise false.</returns>
    Task<bool> CanParseAsync(Stream stream, CancellationToken cancellationToken = default);
}
