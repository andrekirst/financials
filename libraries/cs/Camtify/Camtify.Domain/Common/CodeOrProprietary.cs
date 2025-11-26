namespace Camtify.Domain.Common;

/// <summary>
/// Represents a value that can be either a standardized code or a proprietary value.
/// </summary>
/// <remarks>
/// This pattern is used frequently in ISO 20022 to allow both standard codes
/// and bank-specific proprietary values. XML elements: Cd (Code) or Prtry (Proprietary).
/// </remarks>
public readonly record struct CodeOrProprietary
{
    /// <summary>
    /// Gets the standardized ISO code.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Gets the proprietary (bank-specific) value.
    /// </summary>
    public string? Proprietary { get; }

    /// <summary>
    /// Gets the effective value, preferring Code over Proprietary.
    /// </summary>
    public string? Value => Code ?? Proprietary;

    /// <summary>
    /// Gets a value indicating whether this is a standardized code.
    /// </summary>
    public bool IsCode => !string.IsNullOrWhiteSpace(Code);

    /// <summary>
    /// Gets a value indicating whether this is a proprietary value.
    /// </summary>
    public bool IsProprietary => !string.IsNullOrWhiteSpace(Proprietary);

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeOrProprietary"/> struct.
    /// </summary>
    /// <param name="code">The standardized code.</param>
    /// <param name="proprietary">The proprietary value.</param>
    private CodeOrProprietary(string? code, string? proprietary)
    {
        Code = code;
        Proprietary = proprietary;
    }

    /// <summary>
    /// Creates a CodeOrProprietary from a standardized code.
    /// </summary>
    /// <param name="code">The standardized code.</param>
    /// <returns>A new CodeOrProprietary with the specified code.</returns>
    public static CodeOrProprietary FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code cannot be null or empty.", nameof(code));
        }

        return new CodeOrProprietary(code, null);
    }

    /// <summary>
    /// Creates a CodeOrProprietary from a proprietary value.
    /// </summary>
    /// <param name="proprietary">The proprietary value.</param>
    /// <returns>A new CodeOrProprietary with the specified proprietary value.</returns>
    public static CodeOrProprietary FromProprietary(string proprietary)
    {
        if (string.IsNullOrWhiteSpace(proprietary))
        {
            throw new ArgumentException("Proprietary value cannot be null or empty.", nameof(proprietary));
        }

        return new CodeOrProprietary(null, proprietary);
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
