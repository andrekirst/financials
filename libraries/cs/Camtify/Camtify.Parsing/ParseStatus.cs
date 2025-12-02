namespace Camtify.Parsing;

/// <summary>
/// Status of the parsing operation.
/// </summary>
public enum ParseStatus
{
    /// <summary>
    /// Parsing is starting.
    /// </summary>
    Starting,

    /// <summary>
    /// Parsing the header section.
    /// </summary>
    ParsingHeader,

    /// <summary>
    /// Parsing the body content.
    /// </summary>
    ParsingBody,

    /// <summary>
    /// Parsing entries (for streaming parsers).
    /// </summary>
    ParsingEntries,

    /// <summary>
    /// Parsing completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Parsing failed with errors.
    /// </summary>
    Failed
}
