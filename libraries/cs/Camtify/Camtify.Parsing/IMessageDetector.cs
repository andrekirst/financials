using Camtify.Core;

namespace Camtify.Parsing;

/// <summary>
/// Interface for detecting ISO 20022 message types from XML streams.
/// </summary>
/// <remarks>
/// Message detectors analyze XML namespaces or document elements to determine
/// the message type without fully parsing the document.
/// </remarks>
public interface IMessageDetector
{
    /// <summary>
    /// Detects the message identifier from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the XML message. Position will be reset after detection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detected message identifier.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the stream is not seekable.</exception>
    /// <exception cref="MessageDetectionException">Thrown when message type cannot be detected.</exception>
    Task<MessageIdentifier> DetectAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to detect the message identifier from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the XML message. Position will be reset after detection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detected message identifier, or null if detection failed.</returns>
    Task<MessageIdentifier?> TryDetectAsync(Stream stream, CancellationToken cancellationToken = default);
}
