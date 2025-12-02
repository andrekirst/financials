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
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detected message identifier.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="MessageDetectionException">Thrown when message type cannot be detected.</exception>
    /// <remarks>
    /// The stream position will NOT be reset after detection.
    /// The caller is responsible for resetting the stream position if needed.
    /// </remarks>
    Task<MessageIdentifier> DetectAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects the message identifier and extracts detailed information (including BAH if present).
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detection result with all extracted information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="MessageDetectionException">Thrown when message type cannot be detected.</exception>
    /// <remarks>
    /// The stream position will NOT be reset after detection.
    /// The caller is responsible for resetting the stream position if needed.
    /// </remarks>
    Task<MessageDetectionResult> DetectWithDetailsAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to detect the message identifier from a stream (without throwing exceptions).
    /// </summary>
    /// <param name="stream">The stream containing the XML message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple indicating success, the detected message identifier (if successful), and error message (if failed).</returns>
    /// <remarks>
    /// The stream position will NOT be reset after detection.
    /// The caller is responsible for resetting the stream position if needed.
    /// </remarks>
    Task<(bool Success, MessageIdentifier? MessageId, string? Error)> TryDetectAsync(Stream stream, CancellationToken cancellationToken = default);
}
