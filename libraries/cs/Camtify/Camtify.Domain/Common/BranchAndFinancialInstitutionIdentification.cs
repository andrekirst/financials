namespace Camtify.Domain.Common;

/// <summary>
/// Branch and financial institution identification (XML: BrnchAndFinInstnId).
/// </summary>
/// <remarks>
/// Combines a financial institution identification with optional branch information.
/// Used to identify the servicer of an account or the agent in a payment.
/// </remarks>
public sealed record BranchAndFinancialInstitutionIdentification
{
    /// <summary>
    /// Gets the financial institution identification (XML: FinInstnId).
    /// </summary>
    public required FinancialInstitutionIdentification FinancialInstitutionIdentification { get; init; }

    /// <summary>
    /// Gets the branch identification (XML: BrnchId).
    /// </summary>
    public BranchData? BranchIdentification { get; init; }
}
