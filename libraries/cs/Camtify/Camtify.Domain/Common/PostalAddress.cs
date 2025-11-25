namespace Camtify.Domain.Common;

/// <summary>
/// Postal address (XML: PstlAdr).
/// </summary>
/// <remarks>
/// Represents a physical postal address as used in ISO 20022 messages.
/// Supports both structured and unstructured address formats:
/// 1. Structured: Individual fields (StreetName, BuildingNumber, etc.)
/// 2. Unstructured: AddressLine array (max 7 lines of 70 characters each)
/// Both formats can be combined.
/// </remarks>
public sealed record PostalAddress
{
    /// <summary>
    /// Gets the address type (XML: AdrTp).
    /// </summary>
    public AddressType? AddressType { get; init; }

    /// <summary>
    /// Gets the department name (XML: Dept).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? Department { get; init; }

    /// <summary>
    /// Gets the sub-department name (XML: SubDept).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? SubDepartment { get; init; }

    /// <summary>
    /// Gets the street name (XML: StrtNm).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? StreetName { get; init; }

    /// <summary>
    /// Gets the building number (XML: BldgNb).
    /// </summary>
    /// <remarks>
    /// Maximum 16 characters.
    /// </remarks>
    public string? BuildingNumber { get; init; }

    /// <summary>
    /// Gets the building name (XML: BldgNm).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? BuildingName { get; init; }

    /// <summary>
    /// Gets the floor number (XML: Flr).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? Floor { get; init; }

    /// <summary>
    /// Gets the post box number (XML: PstBx).
    /// </summary>
    /// <remarks>
    /// Maximum 16 characters.
    /// </remarks>
    public string? PostBox { get; init; }

    /// <summary>
    /// Gets the room number (XML: Room).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? Room { get; init; }

    /// <summary>
    /// Gets the post code (XML: PstCd).
    /// </summary>
    /// <remarks>
    /// Maximum 16 characters.
    /// </remarks>
    public string? PostCode { get; init; }

    /// <summary>
    /// Gets the town/city name (XML: TwnNm).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? TownName { get; init; }

    /// <summary>
    /// Gets the town location name (XML: TwnLctnNm).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? TownLocationName { get; init; }

    /// <summary>
    /// Gets the district name (XML: DstrctNm).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? DistrictName { get; init; }

    /// <summary>
    /// Gets the country sub-division such as state or province (XML: CtrySubDvsn).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? CountrySubDivision { get; init; }

    /// <summary>
    /// Gets the country code as ISO 3166-1 alpha-2 (XML: Ctry).
    /// </summary>
    /// <remarks>
    /// Two-letter country code (e.g., "DE", "US", "GB").
    /// </remarks>
    public string? Country { get; init; }

    /// <summary>
    /// Gets the unstructured address lines (XML: AdrLine).
    /// </summary>
    /// <remarks>
    /// Used when the address cannot be structured into the specific fields above.
    /// Maximum 7 lines with up to 70 characters each.
    /// </remarks>
    public IReadOnlyList<string>? AddressLines { get; init; }
}
