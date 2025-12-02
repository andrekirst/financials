namespace Camtify.Parsing;

/// <summary>
/// Represents a parsing warning.
/// </summary>
/// <param name="Message">The warning message.</param>
/// <param name="Path">The XPath to the element where the warning occurred.</param>
/// <param name="LineNumber">The line number where the warning occurred.</param>
/// <param name="LinePosition">The line position where the warning occurred.</param>
public sealed record ParseWarning(
    string Message,
    string? Path = null,
    int? LineNumber = null,
    int? LinePosition = null);
