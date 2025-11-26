using System.ComponentModel;

namespace Camtify.Domain.Common;

/// <summary>
/// Credit or debit indicator according to ISO 20022.
/// </summary>
/// <remarks>
/// Indicates whether an entry is a credit or debit on an account.
/// </remarks>
public enum CreditDebitIndicator
{
    /// <summary>
    /// Credit entry - amount is added to the account.
    /// </summary>
    [Description("CRDT")]
    Credit,

    /// <summary>
    /// Debit entry - amount is subtracted from the account.
    /// </summary>
    [Description("DBIT")]
    Debit
}
