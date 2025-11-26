namespace Camtify.Domain.Common;

/// <summary>
/// Discount amount with optional type specification.
/// </summary>
/// <remarks>
/// XML Element: DscntApldAmt within RfrdDocAmt.
/// </remarks>
public readonly record struct DiscountAmountAndType
{
    /// <summary>
    /// Gets the discount type.
    /// </summary>
    public CodeOrProprietary? Type { get; }

    /// <summary>
    /// Gets the discount amount.
    /// </summary>
    public Money Amount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscountAmountAndType"/> struct.
    /// </summary>
    /// <param name="amount">The discount amount.</param>
    /// <param name="type">The discount type.</param>
    public DiscountAmountAndType(Money amount, CodeOrProprietary? type = null)
    {
        Amount = amount;
        Type = type;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Type.HasValue ? $"{Amount} ({Type.Value})" : Amount.ToString();
    }
}
