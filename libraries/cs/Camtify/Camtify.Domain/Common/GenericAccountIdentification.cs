namespace Camtify.Domain.Common;

/// <summary>
/// Generic account identification (XML: Othr).
/// </summary>
/// <remarks>
/// Used for non-IBAN account identifiers such as BBAN,
/// customer identification numbers, or proprietary schemes.
/// </remarks>
public sealed record GenericAccountIdentification
{
    /// <summary>
    /// Gets the identification value (XML: Id).
    /// </summary>
    /// <remarks>
    /// Maximum 34 characters.
    /// </remarks>
    public required string Identification { get; init; }

    /// <summary>
    /// Gets the identification scheme name (XML: SchmeNm).
    /// </summary>
    public AccountSchemeName? SchemeName { get; init; }

    /// <summary>
    /// Gets the issuer of the identification (XML: Issr).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? Issuer { get; init; }
}
