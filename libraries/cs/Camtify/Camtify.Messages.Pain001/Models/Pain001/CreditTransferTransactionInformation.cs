using Camtify.Domain.Common;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents Credit Transfer Transaction Information (CdtTrfTxInf).
/// </summary>
/// <remarks>
/// Contains details about a single credit transfer transaction.
/// </remarks>
public sealed record CreditTransferTransactionInformation
{
    /// <summary>
    /// Gets the payment identification.
    /// </summary>
    /// <remarks>
    /// XML Element: PmtId
    /// </remarks>
    public PaymentIdentification PaymentIdentification { get; init; } = null!;

    /// <summary>
    /// Gets the payment type information (optional, can override payment information level).
    /// </summary>
    /// <remarks>
    /// XML Element: PmtTpInf
    /// </remarks>
    public PaymentTypeInformation? PaymentTypeInformation { get; init; }

    /// <summary>
    /// Gets the instructed amount.
    /// </summary>
    /// <remarks>
    /// XML Element: Amt/InstdAmt
    /// The amount to be transferred.
    /// </remarks>
    public Money InstructedAmount { get; init; }

    /// <summary>
    /// Gets the charge bearer (optional, can override payment information level).
    /// </summary>
    /// <remarks>
    /// XML Element: ChrgBr
    /// </remarks>
    public ChargeBearer? ChargeBearer { get; init; }

    /// <summary>
    /// Gets the ultimate debtor (optional).
    /// </summary>
    /// <remarks>
    /// XML Element: UltmtDbtr
    /// </remarks>
    public PartyIdentification? UltimateDebtor { get; init; }

    /// <summary>
    /// Gets the intermediary agent (optional).
    /// </summary>
    /// <remarks>
    /// XML Element: IntrmyAgt1
    /// </remarks>
    public BranchAndFinancialInstitutionIdentification? IntermediaryAgent { get; init; }

    /// <summary>
    /// Gets the creditor agent (creditor's bank).
    /// </summary>
    /// <remarks>
    /// XML Element: CdtrAgt
    /// </remarks>
    public BranchAndFinancialInstitutionIdentification CreditorAgent { get; init; } = null!;

    /// <summary>
    /// Gets the creditor (payee).
    /// </summary>
    /// <remarks>
    /// XML Element: Cdtr
    /// </remarks>
    public PartyIdentification Creditor { get; init; } = null!;

    /// <summary>
    /// Gets the creditor account.
    /// </summary>
    /// <remarks>
    /// XML Element: CdtrAcct
    /// </remarks>
    public CashAccount CreditorAccount { get; init; } = null!;

    /// <summary>
    /// Gets the ultimate creditor (optional).
    /// </summary>
    /// <remarks>
    /// XML Element: UltmtCdtr
    /// The party that ultimately receives the amount.
    /// </remarks>
    public PartyIdentification? UltimateCreditor { get; init; }

    /// <summary>
    /// Gets the purpose code.
    /// </summary>
    /// <remarks>
    /// XML Element: Purp/Cd
    /// SALA, PENS, SUPP, etc.
    /// </remarks>
    public string? PurposeCode { get; init; }

    /// <summary>
    /// Gets the remittance information.
    /// </summary>
    /// <remarks>
    /// XML Element: RmtInf
    /// Payment reference or purpose.
    /// </remarks>
    public RemittanceInformation? RemittanceInformation { get; init; }
}
