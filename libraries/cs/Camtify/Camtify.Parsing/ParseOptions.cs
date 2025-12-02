namespace Camtify.Parsing;

/// <summary>
/// Options for parsing ISO 20022 messages.
/// </summary>
public sealed record ParseOptions
{
    /// <summary>
    /// Gets the default parsing options.
    /// </summary>
    public static ParseOptions Default { get; } = new();

    /// <summary>
    /// Gets a value indicating whether to validate against XML schema.
    /// </summary>
    public bool ValidateSchema { get; init; }

    /// <summary>
    /// Gets the path to the XSD schema file for validation.
    /// </summary>
    public string? SchemaPath { get; init; }

    /// <summary>
    /// Gets a value indicating whether to stop on the first error.
    /// </summary>
    public bool StopOnFirstError { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to collect warnings.
    /// </summary>
    public bool CollectWarnings { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to parse the Business Application Header.
    /// </summary>
    public bool ParseApplicationHeader { get; init; } = true;

    /// <summary>
    /// Gets the progress reporter (if any).
    /// </summary>
    public IProgress<ParseProgress>? Progress { get; init; }

    /// <summary>
    /// Gets a value indicating whether to preserve whitespace.
    /// </summary>
    public bool PreserveWhitespace { get; init; }
}
