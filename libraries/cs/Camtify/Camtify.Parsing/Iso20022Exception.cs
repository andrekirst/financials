namespace Camtify.Parsing;

/// <summary>
/// Base exception for all ISO 20022 parsing related errors.
/// </summary>
/// <remarks>
/// This exception serves as the base class for all parsing-specific exceptions in the Camtify library.
/// Catching this exception allows handling all parsing errors in a unified way.
/// </remarks>
public class Iso20022Exception : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022Exception"/> class.
    /// </summary>
    public Iso20022Exception()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022Exception"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public Iso20022Exception(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022Exception"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public Iso20022Exception(string message, Exception innerException) : base(message, innerException)
    {
    }
}
