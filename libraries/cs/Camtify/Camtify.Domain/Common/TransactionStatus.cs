using System.ComponentModel;

namespace Camtify.Domain.Common;

/// <summary>
/// Transaction status codes according to ISO 20022.
/// </summary>
/// <remarks>
/// Indicates the status of a payment transaction in its lifecycle.
/// </remarks>
public enum TransactionStatus
{
    /// <summary>
    /// AcceptedCustomerProfile - Authentication and syntactical validation successful.
    /// </summary>
    [Description("ACCP")]
    AcceptedCustomerProfile,

    /// <summary>
    /// AcceptedSettlementCompleted - Settlement on the creditor's account completed.
    /// </summary>
    [Description("ACSC")]
    AcceptedSettlementCompleted,

    /// <summary>
    /// AcceptedSettlementInProcess - All previous checks successful, initiation settlement processing.
    /// </summary>
    [Description("ACSP")]
    AcceptedSettlementInProcess,

    /// <summary>
    /// AcceptedTechnicalValidation - Authentication and syntactical validation successful.
    /// </summary>
    [Description("ACTC")]
    AcceptedTechnicalValidation,

    /// <summary>
    /// AcceptedWithChange - Instruction accepted but a change will be made.
    /// </summary>
    [Description("ACWC")]
    AcceptedWithChange,

    /// <summary>
    /// AcceptedWithoutPosting - Instruction accepted but not yet executed.
    /// </summary>
    [Description("ACWP")]
    AcceptedWithoutPosting,

    /// <summary>
    /// Received - Payment has been received by the receiving agent.
    /// </summary>
    [Description("RCVD")]
    Received,

    /// <summary>
    /// Pending - Payment is pending processing.
    /// </summary>
    [Description("PDNG")]
    Pending,

    /// <summary>
    /// Rejected - Payment instruction has been rejected.
    /// </summary>
    [Description("RJCT")]
    Rejected,

    /// <summary>
    /// Cancelled - Payment instruction has been cancelled.
    /// </summary>
    [Description("CANC")]
    Cancelled,

    /// <summary>
    /// AcceptedFundsChecked - Preceding checks successful and funds available.
    /// </summary>
    [Description("ACFC")]
    AcceptedFundsChecked,

    /// <summary>
    /// PartiallyAccepted - Only part of the payment instruction has been accepted.
    /// </summary>
    [Description("PART")]
    PartiallyAccepted
}
