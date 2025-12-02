namespace Camtify.Parsing;

/// <summary>
/// Exception thrown when parsing ISO 20022 messages fails.
/// </summary>
public sealed class Iso20022ParsingException : Iso20022Exception
{
    /// <summary>
    /// Gets the collection of parsing errors.
    /// </summary>
    public IReadOnlyCollection<ParseError> Errors { get; }

    /// <summary>
    /// Gets the collection of parsing warnings.
    /// </summary>
    public IReadOnlyCollection<ParseWarning> Warnings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022ParsingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public Iso20022ParsingException(string message)
        : base(message)
    {
        Errors = Array.Empty<ParseError>();
        Warnings = Array.Empty<ParseWarning>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022ParsingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public Iso20022ParsingException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = Array.Empty<ParseError>();
        Warnings = Array.Empty<ParseWarning>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022ParsingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of parsing errors.</param>
    /// <param name="warnings">The collection of parsing warnings.</param>
    public Iso20022ParsingException(
        string message,
        IReadOnlyCollection<ParseError> errors,
        IReadOnlyCollection<ParseWarning> warnings)
        : base(message)
    {
        Errors = errors;
        Warnings = warnings;
    }
}
