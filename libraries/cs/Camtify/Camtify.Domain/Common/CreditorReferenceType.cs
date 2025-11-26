namespace Camtify.Domain.Common;

/// <summary>
/// Represents the type of a creditor reference.
/// </summary>
/// <remarks>
/// XML Element: Tp within CdtrRefInf.
/// </remarks>
public readonly record struct CreditorReferenceType
{
    /// <summary>
    /// Gets the code or proprietary type identification.
    /// </summary>
    public CodeOrProprietary? CodeOrProprietary { get; }

    /// <summary>
    /// Gets the issuer of the reference type.
    /// </summary>
    public string? Issuer { get; }

    /// <summary>
    /// Gets the reference type code if available.
    /// </summary>
    public string? TypeCode => CodeOrProprietary?.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreditorReferenceType"/> struct.
    /// </summary>
    /// <param name="codeOrProprietary">The code or proprietary type.</param>
    /// <param name="issuer">The issuer.</param>
    public CreditorReferenceType(CodeOrProprietary? codeOrProprietary, string? issuer = null)
    {
        CodeOrProprietary = codeOrProprietary;
        Issuer = issuer;
    }

    /// <summary>
    /// Creates a CreditorReferenceType from a standard reference type code.
    /// </summary>
    /// <param name="code">The reference type code (e.g., SCOR).</param>
    /// <param name="issuer">Optional issuer.</param>
    /// <returns>A new CreditorReferenceType.</returns>
    public static CreditorReferenceType FromCode(string code, string? issuer = null)
    {
        return new CreditorReferenceType(Common.CodeOrProprietary.FromCode(code), issuer);
    }

    /// <summary>
    /// Creates a CreditorReferenceType for structured communication reference (RF).
    /// </summary>
    /// <param name="issuer">Optional issuer.</param>
    /// <returns>A new CreditorReferenceType for SCOR.</returns>
    public static CreditorReferenceType ForStructuredReference(string? issuer = null)
    {
        return FromCode(ReferenceTypeCodes.SCOR, issuer);
    }

    /// <summary>
    /// Creates a CreditorReferenceType from a proprietary type.
    /// </summary>
    /// <param name="proprietary">The proprietary type value.</param>
    /// <param name="issuer">Optional issuer.</param>
    /// <returns>A new CreditorReferenceType.</returns>
    public static CreditorReferenceType FromProprietary(string proprietary, string? issuer = null)
    {
        return new CreditorReferenceType(Common.CodeOrProprietary.FromProprietary(proprietary), issuer);
    }

    /// <summary>
    /// Gets the description for this reference type.
    /// </summary>
    /// <returns>The description.</returns>
    public string GetDescription()
    {
        if (CodeOrProprietary?.IsCode == true)
        {
            return ReferenceTypeCodes.GetDescription(CodeOrProprietary.Value.Code);
        }

        return CodeOrProprietary?.Value ?? string.Empty;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var desc = GetDescription();
        return Issuer is not null ? $"{desc} (Issuer: {Issuer})" : desc;
    }
}
