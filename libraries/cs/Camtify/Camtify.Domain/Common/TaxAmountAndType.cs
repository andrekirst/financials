namespace Camtify.Domain.Common;

/// <summary>
/// Tax amount with optional type specification.
/// </summary>
/// <remarks>
/// XML Element: TaxAmt within RfrdDocAmt.
/// </remarks>
public readonly record struct TaxAmountAndType
{
    /// <summary>
    /// Gets the tax type (e.g., VAT, GST).
    /// </summary>
    public CodeOrProprietary? Type { get; }

    /// <summary>
    /// Gets the tax amount.
    /// </summary>
    public Money Amount { get; }

    /// <summary>
    /// Gets the tax rate as percentage.
    /// </summary>
    public decimal? Rate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaxAmountAndType"/> struct.
    /// </summary>
    /// <param name="amount">The tax amount.</param>
    /// <param name="type">The tax type.</param>
    /// <param name="rate">The tax rate.</param>
    public TaxAmountAndType(Money amount, CodeOrProprietary? type = null, decimal? rate = null)
    {
        Amount = amount;
        Type = type;
        Rate = rate;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string> { Amount.ToString() };

        if (Type.HasValue)
        {
            parts.Add(Type.Value.Value ?? string.Empty);
        }

        if (Rate.HasValue)
        {
            parts.Add($"{Rate.Value:F2}%");
        }

        return string.Join(" ", parts);
    }
}
