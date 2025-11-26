namespace Camtify.Domain.Common;

/// <summary>
/// Status reason with code or proprietary identification according to ISO 20022.
/// </summary>
/// <remarks>
/// Represents either a standardized reason code or a proprietary/narrative reason.
/// </remarks>
public readonly record struct StatusReason
{
    /// <summary>
    /// Gets the standardized reason code (e.g., AC01, AM04).
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Gets the proprietary or narrative reason text.
    /// </summary>
    public string? Proprietary { get; }

    /// <summary>
    /// Gets a value indicating whether this is a coded reason.
    /// </summary>
    public bool IsCode => !string.IsNullOrWhiteSpace(Code);

    /// <summary>
    /// Gets a value indicating whether this is a proprietary/narrative reason.
    /// </summary>
    public bool IsProprietary => !string.IsNullOrWhiteSpace(Proprietary);

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusReason"/> struct with a code.
    /// </summary>
    /// <param name="code">The reason code.</param>
    private StatusReason(string? code, string? proprietary)
    {
        Code = code;
        Proprietary = proprietary;
    }

    /// <summary>
    /// Creates a status reason from a standardized code.
    /// </summary>
    /// <param name="code">The reason code (e.g., AC01, AM04).</param>
    /// <returns>A new StatusReason with the specified code.</returns>
    public static StatusReason FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Reason code cannot be null or empty.", nameof(code));
        }

        return new StatusReason(code, null);
    }

    /// <summary>
    /// Creates a status reason from a proprietary or narrative text.
    /// </summary>
    /// <param name="proprietary">The proprietary or narrative reason text.</param>
    /// <returns>A new StatusReason with the specified proprietary text.</returns>
    public static StatusReason FromProprietary(string proprietary)
    {
        if (string.IsNullOrWhiteSpace(proprietary))
        {
            throw new ArgumentException("Proprietary reason cannot be null or empty.", nameof(proprietary));
        }

        return new StatusReason(null, proprietary);
    }

    /// <summary>
    /// Gets the German description for this reason if available.
    /// </summary>
    /// <returns>The German description, or the original code/proprietary text.</returns>
    public string GetDescription()
    {
        if (IsCode)
        {
            return ReasonCodeDescriptions.GetDescription(Code!);
        }

        return Proprietary ?? string.Empty;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsCode)
        {
            return $"Code: {Code}";
        }

        if (IsProprietary)
        {
            return $"Proprietary: {Proprietary}";
        }

        return "Empty";
    }
}
