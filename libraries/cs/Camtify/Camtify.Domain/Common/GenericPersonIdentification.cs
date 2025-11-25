namespace Camtify.Domain.Common;

/// <summary>
/// Generic person identification with scheme (XML: Othr within PrvtId).
/// </summary>
/// <remarks>
/// Used for person identification schemes like passport number,
/// driver's license, national ID, tax ID, etc.
/// </remarks>
public sealed record GenericPersonIdentification
{
    /// <summary>
    /// Gets the identification value (XML: Id).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public required string Identification { get; init; }

    /// <summary>
    /// Gets the scheme name of the identification (XML: SchmeNm).
    /// </summary>
    public PersonIdentificationSchemeName? SchemeName { get; init; }

    /// <summary>
    /// Gets the issuer of the identification (XML: Issr).
    /// </summary>
    /// <remarks>
    /// Entity that issued the identification document.
    /// Maximum 35 characters.
    /// </remarks>
    public string? Issuer { get; init; }
}
