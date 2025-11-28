namespace Camtify.Parsing;

/// <summary>
/// Exception thrown when the parser type does not match the expected document type.
/// </summary>
public class ParserTypeMismatchException : Iso20022Exception
{
    /// <summary>
    /// Gets the expected document type.
    /// </summary>
    public Type ExpectedType { get; }

    /// <summary>
    /// Gets the actual document type produced by the parser.
    /// </summary>
    public Type ActualType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserTypeMismatchException"/> class.
    /// </summary>
    /// <param name="expectedType">The expected document type.</param>
    /// <param name="actualType">The actual document type.</param>
    public ParserTypeMismatchException(Type expectedType, Type actualType)
        : base($"Parser produces '{actualType.Name}', expected '{expectedType.Name}'.")
    {
        ExpectedType = expectedType;
        ActualType = actualType;
    }
}
