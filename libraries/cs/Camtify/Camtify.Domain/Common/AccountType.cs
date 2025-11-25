namespace Camtify.Domain.Common;

/// <summary>
/// Account type (XML: Tp).
/// </summary>
/// <remarks>
/// Specifies the type of account using either an ISO code or a proprietary value.
/// Either Code or Proprietary should be provided, not both.
/// </remarks>
public sealed record AccountType
{
    /// <summary>
    /// Gets the ISO code for the account type (XML: Cd).
    /// </summary>
    /// <remarks>
    /// Common codes:
    /// - CACC = Current Account (checking account)
    /// - SVGS = Savings Account
    /// - LOAN = Loan Account
    /// - CASH = Cash Payment
    /// - CHAR = Charges Account
    /// - CISH = Cash Income
    /// - COMM = Commission
    /// - CPAC = Clearing Participant Account
    /// - LLSV = Limited Liquidity Savings
    /// - MGLD = Marginal Lending
    /// - MOMA = Money Market
    /// - NREX = Non-Resident External
    /// - ODFT = Overdraft
    /// - ONDP = Overnight Deposit
    /// - OTHR = Other
    /// - SACC = Settlement Account
    /// - SLRY = Salary
    /// - TAXE = Tax
    /// - TRAN = Transacting Account
    /// - TRAS = Cash Trading
    /// </remarks>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the proprietary account type (XML: Prtry).
    /// </summary>
    /// <remarks>
    /// Used when the account type is not covered by the ISO code list.
    /// Maximum 35 characters.
    /// </remarks>
    public string? Proprietary { get; init; }
}
