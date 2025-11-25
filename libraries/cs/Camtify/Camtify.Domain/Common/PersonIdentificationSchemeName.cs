namespace Camtify.Domain.Common;

/// <summary>
/// Scheme name for person identification (XML: SchmeNm).
/// </summary>
/// <remarks>
/// Identifies the scheme used for person identification.
/// Either Code or Proprietary should be provided, not both.
/// </remarks>
public sealed record PersonIdentificationSchemeName
{
    /// <summary>
    /// Gets the ISO code of the identification scheme (XML: Cd).
    /// </summary>
    /// <remarks>
    /// Standard codes include:
    /// - ARNU: Alien Registration Number
    /// - CCPT: Passport Number
    /// - CUST: Customer Identification Number
    /// - DRLC: Driver's License Number
    /// - EMPL: Employer Identification Number
    /// - NIDN: National Identity Number
    /// - SOSE: Social Security Number
    /// - TXID: Tax Identification Number
    /// </remarks>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the proprietary scheme name (XML: Prtry).
    /// </summary>
    /// <remarks>
    /// Used when the identification scheme is not covered by standard codes.
    /// Maximum 35 characters.
    /// </remarks>
    public string? Proprietary { get; init; }
}
