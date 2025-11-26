namespace Camtify.Domain.Common;

/// <summary>
/// Represents the type of a referred document in remittance information.
/// </summary>
/// <remarks>
/// XML Element: Tp (Type) within RfrdDocInf.
/// </remarks>
public readonly record struct ReferredDocumentType
{
    /// <summary>
    /// Gets the code or proprietary type identification.
    /// </summary>
    public CodeOrProprietary? CodeOrProprietary { get; }

    /// <summary>
    /// Gets the issuer of the document type (e.g., bank or organization).
    /// </summary>
    public string? Issuer { get; }

    /// <summary>
    /// Gets the document type code if available.
    /// </summary>
    public string? TypeCode => CodeOrProprietary?.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferredDocumentType"/> struct.
    /// </summary>
    /// <param name="codeOrProprietary">The code or proprietary type.</param>
    /// <param name="issuer">The issuer.</param>
    public ReferredDocumentType(CodeOrProprietary? codeOrProprietary, string? issuer = null)
    {
        CodeOrProprietary = codeOrProprietary;
        Issuer = issuer;
    }

    /// <summary>
    /// Creates a ReferredDocumentType from a standard document type code.
    /// </summary>
    /// <param name="code">The document type code (e.g., CINV, CREN).</param>
    /// <param name="issuer">Optional issuer.</param>
    /// <returns>A new ReferredDocumentType.</returns>
    public static ReferredDocumentType FromCode(string code, string? issuer = null)
    {
        return new ReferredDocumentType(Common.CodeOrProprietary.FromCode(code), issuer);
    }

    /// <summary>
    /// Creates a ReferredDocumentType from a proprietary type.
    /// </summary>
    /// <param name="proprietary">The proprietary type value.</param>
    /// <param name="issuer">Optional issuer.</param>
    /// <returns>A new ReferredDocumentType.</returns>
    public static ReferredDocumentType FromProprietary(string proprietary, string? issuer = null)
    {
        return new ReferredDocumentType(Common.CodeOrProprietary.FromProprietary(proprietary), issuer);
    }

    /// <summary>
    /// Gets the description for this document type.
    /// </summary>
    /// <returns>The description.</returns>
    public string GetDescription()
    {
        if (CodeOrProprietary?.IsCode == true)
        {
            return DocumentTypeCodes.GetDescription(CodeOrProprietary.Value.Code);
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
