using Camtify.Domain.Common;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents Payment Information (PmtInf) in a pain.001 message.
/// </summary>
/// <remarks>
/// Contains information about a set of credit transfers with common characteristics.
/// </remarks>
public sealed record PaymentInformation
{
    /// <summary>
    /// Gets the payment information identification.
    /// </summary>
    /// <remarks>
    /// XML Element: PmtInfId
    /// Must be unique per message.
    /// </remarks>
    public string PaymentInformationIdentification { get; init; } = null!;

    /// <summary>
    /// Gets the payment method.
    /// </summary>
    /// <remarks>
    /// XML Element: PmtMtd
    /// For credit transfers, typically "TRF" (Transfer).
    /// </remarks>
    public string PaymentMethod { get; init; } = null!;

    /// <summary>
    /// Gets a value indicating whether this is a batch booking.
    /// </summary>
    /// <remarks>
    /// XML Element: BtchBookg
    /// True = book as single entry, False = book individually.
    /// </remarks>
    public bool? BatchBooking { get; init; }

    /// <summary>
    /// Gets the number of transactions in this payment information block.
    /// </summary>
    /// <remarks>
    /// XML Element: NbOfTxs
    /// Optional. Number of credit transfer transaction information elements.
    /// </remarks>
    public int? NumberOfTransactions { get; init; }

    /// <summary>
    /// Gets the control sum for this payment information block.
    /// </summary>
    /// <remarks>
    /// XML Element: CtrlSum
    /// Optional. Sum of all instructed amounts in this block.
    /// </remarks>
    public decimal? ControlSum { get; init; }

    /// <summary>
    /// Gets the payment type information.
    /// </summary>
    /// <remarks>
    /// XML Element: PmtTpInf
    /// Contains service level, local instrument, category purpose.
    /// </remarks>
    public PaymentTypeInformation? PaymentTypeInformation { get; init; }

    /// <summary>
    /// Gets the requested execution date.
    /// </summary>
    /// <remarks>
    /// XML Element: ReqdExctnDt
    /// Date when the payment should be executed.
    /// </remarks>
    public DateOnly RequestedExecutionDate { get; init; }

    /// <summary>
    /// Gets the debtor (payer).
    /// </summary>
    /// <remarks>
    /// XML Element: Dbtr
    /// </remarks>
    public PartyIdentification Debtor { get; init; } = null!;

    /// <summary>
    /// Gets the debtor account.
    /// </summary>
    /// <remarks>
    /// XML Element: DbtrAcct
    /// </remarks>
    public CashAccount DebtorAccount { get; init; } = null!;

    /// <summary>
    /// Gets the debtor agent (debtor's bank).
    /// </summary>
    /// <remarks>
    /// XML Element: DbtrAgt
    /// </remarks>
    public BranchAndFinancialInstitutionIdentification DebtorAgent { get; init; } = null!;

    /// <summary>
    /// Gets the ultimate debtor (optional).
    /// </summary>
    /// <remarks>
    /// XML Element: UltmtDbtr
    /// The party that ultimately owes the amount.
    /// </remarks>
    public PartyIdentification? UltimateDebtor { get; init; }

    /// <summary>
    /// Gets the charge bearer.
    /// </summary>
    /// <remarks>
    /// XML Element: ChrgBr
    /// Indicates who bears the charges (SLEV for SEPA, SHAR, DEBT, CRED).
    /// </remarks>
    public ChargeBearer? ChargeBearer { get; init; }

    /// <summary>
    /// Gets the list of credit transfer transaction information.
    /// </summary>
    /// <remarks>
    /// XML Element: CdtTrfTxInf
    /// At least one transaction is required.
    /// </remarks>
    public IReadOnlyList<CreditTransferTransactionInformation> CreditTransferTransactionInformation { get; init; } = null!;
}
