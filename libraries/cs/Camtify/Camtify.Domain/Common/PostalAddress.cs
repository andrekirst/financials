namespace Camtify.Domain.Common;

/// <summary>
/// Postal address (XML: PstlAdr).
/// </summary>
/// <remarks>
/// Represents a physical postal address as used in ISO 20022 messages.
/// Supports both structured and unstructured address formats.
/// </remarks>
public sealed record PostalAddress
{
    /// <summary>
    /// Address type (e.g., residential, business) (XML: AdrTp).
    /// </summary>
    public string? AddressType { get; init; }

    /// <summary>
    /// Department name (XML: Dept).
    /// </summary>
    public string? Department { get; init; }

    /// <summary>
    /// Sub-department name (XML: SubDept).
    /// </summary>
    public string? SubDepartment { get; init; }

    /// <summary>
    /// Street name (XML: StrtNm).
    /// </summary>
    public string? StreetName { get; init; }

    /// <summary>
    /// Building number (XML: BldgNb).
    /// </summary>
    public string? BuildingNumber { get; init; }

    /// <summary>
    /// Building name (XML: BldgNm).
    /// </summary>
    public string? BuildingName { get; init; }

    /// <summary>
    /// Floor number (XML: Flr).
    /// </summary>
    public string? Floor { get; init; }

    /// <summary>
    /// Post box number (XML: PstBx).
    /// </summary>
    public string? PostBox { get; init; }

    /// <summary>
    /// Room number (XML: Room).
    /// </summary>
    public string? Room { get; init; }

    /// <summary>
    /// Post code (XML: PstCd).
    /// </summary>
    public string? PostCode { get; init; }

    /// <summary>
    /// Town/city name (XML: TwnNm).
    /// </summary>
    public string? TownName { get; init; }

    /// <summary>
    /// Town location name (XML: TwnLctnNm).
    /// </summary>
    public string? TownLocationName { get; init; }

    /// <summary>
    /// District name (XML: DstrctNm).
    /// </summary>
    public string? DistrictName { get; init; }

    /// <summary>
    /// Country sub-division (state/province) (XML: CtrySubDvsn).
    /// </summary>
    public string? CountrySubDivision { get; init; }

    /// <summary>
    /// Country code as ISO 3166-1 alpha-2 (XML: Ctry).
    /// </summary>
    /// <remarks>
    /// Two-letter country code according to ISO 3166-1.
    /// Examples: "DE" for Germany, "US" for United States, "GB" for United Kingdom.
    /// </remarks>
    public string? Country { get; init; }

    /// <summary>
    /// Unstructured address lines (XML: AdrLine).
    /// </summary>
    /// <remarks>
    /// Used when the address cannot be structured into the specific fields above.
    /// Maximum 7 lines with up to 70 characters each.
    /// </remarks>
    public IReadOnlyList<string>? AddressLines { get; init; }
}
