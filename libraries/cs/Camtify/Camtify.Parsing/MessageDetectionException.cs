namespace Camtify.Parsing;

/// <summary>
/// Exception thrown when message type detection fails.
/// </summary>
/// <remarks>
/// This exception is thrown when the message detector cannot determine the message type
/// from the XML stream, usually due to missing or invalid namespace information.
/// </remarks>
public class MessageDetectionException : Iso20022Exception
{
    /// <summary>
    /// Gets the namespace that was found (if any).
    /// </summary>
    public string? FoundNamespace { get; }

    /// <summary>
    /// Gets the root element that was found (if any).
    /// </summary>
    public string? FoundRootElement { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetectionException"/> class.
    /// </summary>
    public MessageDetectionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetectionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MessageDetectionException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetectionException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MessageDetectionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetectionException"/> class with detailed information
    /// about what was found during detection.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="foundNamespace">The namespace that was found (if any).</param>
    /// <param name="foundRootElement">The root element that was found (if any).</param>
    public MessageDetectionException(string message, string? foundNamespace, string? foundRootElement)
        : base(message)
    {
        FoundNamespace = foundNamespace;
        FoundRootElement = foundRootElement;
    }
}
