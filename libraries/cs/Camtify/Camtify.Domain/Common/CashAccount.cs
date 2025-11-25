namespace Camtify.Domain.Common;

/// <summary>
/// Cash account (bank/checking account) (XML: DbtrAcct, CdtrAcct, Acct).
/// </summary>
/// <remarks>
/// Represents a cash account as used in ISO 20022 payment messages.
/// Used for debtor accounts, creditor accounts, and general account references.
/// </remarks>
public sealed record CashAccount
{
    /// <summary>
    /// Gets the account identification (XML: Id).
    /// </summary>
    /// <remarks>
    /// IBAN or other account identification.
    /// </remarks>
    public required AccountIdentification Identification { get; init; }

    /// <summary>
    /// Gets the account type (XML: Tp).
    /// </summary>
    public AccountType? Type { get; init; }

    /// <summary>
    /// Gets the account currency (XML: Ccy).
    /// </summary>
    /// <remarks>
    /// ISO 4217 three-letter currency code.
    /// </remarks>
    public CurrencyCode? Currency { get; init; }

    /// <summary>
    /// Gets the account name/description (XML: Nm).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the proxy/alias for the account (XML: Prxy).
    /// </summary>
    /// <remarks>
    /// Used for instant payments (TIPS, SCT Inst) where the account
    /// can be identified by phone number, email, or other alias.
    /// </remarks>
    public ProxyAccountIdentification? Proxy { get; init; }

    /// <summary>
    /// Gets the account owner (XML: Ownr).
    /// </summary>
    public PartyIdentification? Owner { get; init; }

    /// <summary>
    /// Gets the account servicer (XML: Svcr).
    /// </summary>
    /// <remarks>
    /// The financial institution that maintains the account.
    /// </remarks>
    public BranchAndFinancialInstitutionIdentification? Servicer { get; init; }
}
