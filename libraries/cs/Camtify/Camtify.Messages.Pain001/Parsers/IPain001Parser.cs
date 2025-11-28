namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Common interface for all pain.001 parsers.
/// </summary>
/// <remarks>
/// This interface provides version-independent access to pain.001 parsers.
/// Use Pain001ParserFactory.CreateAsync() for automatic version detection.
/// </remarks>
public interface IPain001Parser
{
    /// <summary>
    /// Gets the pain.001 version identifier (e.g., "003", "009", "011").
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the XML namespace URI for this parser version.
    /// </summary>
    string Namespace { get; }
}
