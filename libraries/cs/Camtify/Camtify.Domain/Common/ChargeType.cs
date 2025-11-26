namespace Camtify.Domain.Common;

/// <summary>
/// Charge type (XML: Tp in ChargesInformation).
/// </summary>
/// <remarks>
/// Specifies the type of charge using either an ISO code or a proprietary value.
/// Either Code or Proprietary should be provided, not both.
/// </remarks>
public sealed record ChargeType
{
    /// <summary>
    /// Gets the ISO code for the charge type (XML: Cd).
    /// </summary>
    /// <remarks>
    /// Common codes:
    /// - BRKF = Brokerage Fee
    /// - COMM = Commission
    /// - FEES = Fees
    /// - TAXE = Tax
    /// </remarks>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the proprietary charge type (XML: Prtry).
    /// </summary>
    public string? Proprietary { get; init; }
}
