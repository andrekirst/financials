namespace Camtify.Domain.Common;

/// <summary>
/// Date and place of birth of a person (XML: DtAndPlcOfBirth).
/// </summary>
/// <remarks>
/// Used for person identification in ISO 20022 messages.
/// All fields except ProvinceOfBirth are required.
/// </remarks>
public sealed record DateAndPlaceOfBirth
{
    /// <summary>
    /// Gets the birth date (XML: BirthDt).
    /// </summary>
    public required DateOnly BirthDate { get; init; }

    /// <summary>
    /// Gets the province/state of birth (XML: PrvcOfBirth).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? ProvinceOfBirth { get; init; }

    /// <summary>
    /// Gets the city of birth (XML: CityOfBirth).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public required string CityOfBirth { get; init; }

    /// <summary>
    /// Gets the country of birth as ISO 3166-1 alpha-2 code (XML: CtryOfBirth).
    /// </summary>
    /// <remarks>
    /// Two-letter country code (e.g., "DE", "US", "GB").
    /// </remarks>
    public required string CountryOfBirth { get; init; }
}
