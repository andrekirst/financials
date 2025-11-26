namespace Camtify.Domain.Common;

/// <summary>
/// Amounts referenced in structured remittance information.
/// </summary>
/// <remarks>
/// XML Element: RfrdDocAmt within Strd.
/// Contains various amounts related to the referenced document.
/// </remarks>
public readonly record struct RemittanceAmount
{
    /// <summary>
    /// Gets the due payable amount.
    /// </summary>
    public Money? DuePayableAmount { get; }

    /// <summary>
    /// Gets the discount applied amount.
    /// </summary>
    public DiscountAmountAndType? DiscountAppliedAmount { get; }

    /// <summary>
    /// Gets the credit note amount.
    /// </summary>
    public Money? CreditNoteAmount { get; }

    /// <summary>
    /// Gets the tax amount.
    /// </summary>
    public TaxAmountAndType? TaxAmount { get; }

    /// <summary>
    /// Gets the adjustment amounts and reasons.
    /// </summary>
    public IReadOnlyList<DocumentAdjustment>? AdjustmentAmountAndReason { get; }

    /// <summary>
    /// Gets the remitted amount (actually transferred).
    /// </summary>
    public Money? RemittedAmount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemittanceAmount"/> struct.
    /// </summary>
    public RemittanceAmount(
        Money? duePayableAmount = null,
        DiscountAmountAndType? discountAppliedAmount = null,
        Money? creditNoteAmount = null,
        TaxAmountAndType? taxAmount = null,
        IReadOnlyList<DocumentAdjustment>? adjustmentAmountAndReason = null,
        Money? remittedAmount = null)
    {
        DuePayableAmount = duePayableAmount;
        DiscountAppliedAmount = discountAppliedAmount;
        CreditNoteAmount = creditNoteAmount;
        TaxAmount = taxAmount;
        AdjustmentAmountAndReason = adjustmentAmountAndReason;
        RemittedAmount = remittedAmount;
    }

    /// <summary>
    /// Creates a RemittanceAmount with just the due payable amount.
    /// </summary>
    /// <param name="duePayableAmount">The due payable amount.</param>
    /// <returns>A new RemittanceAmount.</returns>
    public static RemittanceAmount ForDueAmount(Money duePayableAmount)
    {
        return new RemittanceAmount(duePayableAmount: duePayableAmount);
    }

    /// <summary>
    /// Creates a RemittanceAmount with due and remitted amounts.
    /// </summary>
    /// <param name="duePayableAmount">The due payable amount.</param>
    /// <param name="remittedAmount">The remitted amount.</param>
    /// <returns>A new RemittanceAmount.</returns>
    public static RemittanceAmount ForPayment(Money duePayableAmount, Money remittedAmount)
    {
        return new RemittanceAmount(duePayableAmount: duePayableAmount, remittedAmount: remittedAmount);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string>();

        if (DuePayableAmount.HasValue)
        {
            parts.Add($"Due: {DuePayableAmount.Value}");
        }

        if (RemittedAmount.HasValue)
        {
            parts.Add($"Remitted: {RemittedAmount.Value}");
        }

        if (DiscountAppliedAmount.HasValue)
        {
            parts.Add($"Discount: {DiscountAppliedAmount.Value.Amount}");
        }

        if (TaxAmount.HasValue)
        {
            parts.Add($"Tax: {TaxAmount.Value.Amount}");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "Empty";
    }
}
