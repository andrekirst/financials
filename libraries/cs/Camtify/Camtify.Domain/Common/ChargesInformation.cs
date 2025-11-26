namespace Camtify.Domain.Common;

/// <summary>
/// Charges information (XML: ChrgsInf in pacs.008, camt.053).
/// </summary>
/// <remarks>
/// Provides information about charges applied to a transaction,
/// including the amount, the agent that applied the charge, and the type.
/// </remarks>
public sealed record ChargesInformation
{
    /// <summary>
    /// Gets the charge amount (XML: Amt).
    /// </summary>
    public required Money Amount { get; init; }

    /// <summary>
    /// Gets the agent that applied the charge (XML: Agt).
    /// </summary>
    public BranchAndFinancialInstitutionIdentification? Agent { get; init; }

    /// <summary>
    /// Gets the charge type (XML: Tp).
    /// </summary>
    public ChargeType? Type { get; init; }
}
