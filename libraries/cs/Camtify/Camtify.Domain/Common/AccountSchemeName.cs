namespace Camtify.Domain.Common;

/// <summary>
/// Schema name for account identification (XML: SchmeNm).
/// </summary>
/// <remarks>
/// Identifies the scheme used for the account identification.
/// Either Code or Proprietary should be provided, not both.
/// </remarks>
public sealed record AccountSchemeName
{
    /// <summary>
    /// Gets the ISO code for the account scheme (XML: Cd).
    /// </summary>
    /// <remarks>
    /// Common codes:
    /// - BBAN = Basic Bank Account Number
    /// - CUID = Customer Identification Number
    /// - UPIC = Universal Payment Identification Code
    /// </remarks>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the proprietary scheme name (XML: Prtry).
    /// </summary>
    /// <remarks>
    /// Used when the scheme is not covered by the ISO code list.
    /// Maximum 35 characters.
    /// </remarks>
    public string? Proprietary { get; init; }
}
