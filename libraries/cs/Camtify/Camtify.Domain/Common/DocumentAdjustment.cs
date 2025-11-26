namespace Camtify.Domain.Common;

/// <summary>
/// Document adjustment with amount and reason.
/// </summary>
/// <remarks>
/// XML Element: AdjstmntAmtAndRsn within RfrdDocAmt.
/// </remarks>
public readonly record struct DocumentAdjustment
{
    /// <summary>
    /// Gets the adjustment amount.
    /// </summary>
    public Money Amount { get; }

    /// <summary>
    /// Gets the credit/debit indicator for the adjustment.
    /// </summary>
    public CreditDebitIndicator? CreditDebitIndicator { get; }

    /// <summary>
    /// Gets the reason for the adjustment.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Gets additional information about the adjustment.
    /// </summary>
    public string? AdditionalInformation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentAdjustment"/> struct.
    /// </summary>
    /// <param name="amount">The adjustment amount.</param>
    /// <param name="creditDebitIndicator">The credit/debit indicator.</param>
    /// <param name="reason">The reason.</param>
    /// <param name="additionalInformation">Additional information.</param>
    public DocumentAdjustment(
        Money amount,
        CreditDebitIndicator? creditDebitIndicator = null,
        string? reason = null,
        string? additionalInformation = null)
    {
        Amount = amount;
        CreditDebitIndicator = creditDebitIndicator;
        Reason = reason;
        AdditionalInformation = additionalInformation;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string> { Amount.ToString() };

        if (CreditDebitIndicator.HasValue)
        {
            parts.Add(CreditDebitIndicator.Value == Common.CreditDebitIndicator.Credit ? "CR" : "DR");
        }

        if (!string.IsNullOrWhiteSpace(Reason))
        {
            parts.Add(Reason);
        }

        return string.Join(" ", parts);
    }
}
