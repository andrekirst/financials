namespace Camtify.Domain.Common;

/// <summary>
/// Generic identification scheme (XML: Othr).
/// </summary>
/// <remarks>
/// Used for proprietary or non-standard identification schemes
/// that are not covered by the standard ISO 20022 identification types.
/// </remarks>
public sealed record GenericIdentification
{
    /// <summary>
    /// Identification value (XML: Id).
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Name of the identification scheme (XML: SchmeNm).
    /// </summary>
    /// <remarks>
    /// Can be a code from an external code list or a proprietary scheme name.
    /// </remarks>
    public string? SchemeName { get; init; }

    /// <summary>
    /// Issuer of the identification (XML: Issr).
    /// </summary>
    /// <remarks>
    /// Entity that assigns and maintains the identification.
    /// </remarks>
    public string? Issuer { get; init; }
}
