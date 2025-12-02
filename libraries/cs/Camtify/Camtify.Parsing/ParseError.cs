namespace Camtify.Parsing;

/// <summary>
/// Represents a parsing error.
/// </summary>
/// <param name="Message">The error message.</param>
/// <param name="Path">The XPath to the element where the error occurred.</param>
/// <param name="LineNumber">The line number where the error occurred.</param>
/// <param name="LinePosition">The line position where the error occurred.</param>
/// <param name="InnerException">The inner exception (if any).</param>
public sealed record ParseError(
    string Message,
    string? Path = null,
    int? LineNumber = null,
    int? LinePosition = null,
    Exception? InnerException = null);
