using System.Globalization;

namespace Camtify.Domain.Common;

/// <summary>
/// Money amount with currency (XML: ActiveCurrencyAndAmount).
/// </summary>
/// <remarks>
/// Uses decimal for exact calculation (no rounding errors).
/// Supports amounts up to 18 digits with up to 5 decimal places.
/// </remarks>
/// <example>
/// var amount = new Money(1234.56m, CurrencyCode.EUR);
/// </example>
public readonly record struct Money : IComparable<Money>
{
    /// <summary>
    /// Gets the amount value.
    /// </summary>
    /// <remarks>
    /// Positive for credit, negative for debit (depending on context).
    /// </remarks>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency according to ISO 4217.
    /// </summary>
    public CurrencyCode Currency { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> struct.
    /// </summary>
    /// <param name="amount">The amount value.</param>
    /// <param name="currency">The currency code.</param>
    public Money(decimal amount, CurrencyCode currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Parses Money from string values (for XML parsing).
    /// </summary>
    /// <param name="amount">The amount as string.</param>
    /// <param name="currencyCode">The currency code as string.</param>
    /// <returns>A new Money instance.</returns>
    public static Money Parse(string amount, string currencyCode)
    {
        return new Money(
            decimal.Parse(amount, CultureInfo.InvariantCulture),
            CurrencyCode.Parse(currencyCode));
    }

    /// <summary>
    /// Gets the absolute value of the amount.
    /// </summary>
    /// <returns>A new Money with the absolute amount.</returns>
    public Money Abs() => new(Math.Abs(Amount), Currency);

    /// <summary>
    /// Negates the amount.
    /// </summary>
    /// <returns>A new Money with the negated amount.</returns>
    public Money Negate() => new(-Amount, Currency);

    /// <summary>
    /// Gets a value indicating whether the amount is positive.
    /// </summary>
    public bool IsPositive => Amount > 0;

    /// <summary>
    /// Gets a value indicating whether the amount is negative.
    /// </summary>
    public bool IsNegative => Amount < 0;

    /// <summary>
    /// Gets a value indicating whether the amount is zero.
    /// </summary>
    public bool IsZero => Amount == 0;

    /// <summary>
    /// Adds two Money values (same currency only).
    /// </summary>
    /// <param name="left">The first Money value.</param>
    /// <param name="right">The second Money value.</param>
    /// <returns>The sum of both values.</returns>
    /// <exception cref="InvalidOperationException">If currencies do not match.</exception>
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot add {left.Currency} and {right.Currency}.");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two Money values (same currency only).
    /// </summary>
    /// <param name="left">The first Money value.</param>
    /// <param name="right">The second Money value.</param>
    /// <returns>The difference of both values.</returns>
    /// <exception cref="InvalidOperationException">If currencies do not match.</exception>
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot subtract {left.Currency} and {right.Currency}.");
        }

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies Money by a factor.
    /// </summary>
    /// <param name="money">The Money value.</param>
    /// <param name="factor">The multiplication factor.</param>
    /// <returns>The product.</returns>
    public static Money operator *(Money money, decimal factor)
        => new(money.Amount * factor, money.Currency);

    /// <summary>
    /// Multiplies Money by a factor.
    /// </summary>
    /// <param name="factor">The multiplication factor.</param>
    /// <param name="money">The Money value.</param>
    /// <returns>The product.</returns>
    public static Money operator *(decimal factor, Money money)
        => new(money.Amount * factor, money.Currency);

    /// <summary>
    /// Divides Money by a divisor.
    /// </summary>
    /// <param name="money">The Money value.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The quotient.</returns>
    /// <exception cref="DivideByZeroException">If divisor is zero.</exception>
    public static Money operator /(Money money, decimal divisor)
        => new(money.Amount / divisor, money.Currency);

    /// <summary>
    /// Compares this Money to another (same currency only).
    /// </summary>
    /// <param name="other">The other Money value.</param>
    /// <returns>A value indicating the relative order.</returns>
    /// <exception cref="InvalidOperationException">If currencies do not match.</exception>
    public int CompareTo(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot compare {Currency} and {other.Currency}.");
        }

        return Amount.CompareTo(other.Amount);
    }

    /// <summary>
    /// Formats the Money as a string with the specified format provider.
    /// </summary>
    /// <param name="provider">The format provider.</param>
    /// <returns>The formatted string.</returns>
    public string ToString(IFormatProvider? provider)
    {
        return $"{Amount.ToString("N2", provider)} {Currency}";
    }

    /// <inheritdoc />
    public override string ToString() => $"{Amount.ToString("N2", CultureInfo.InvariantCulture)} {Currency}";

    /// <summary>
    /// Creates a zero amount for the specified currency.
    /// </summary>
    /// <param name="currency">The currency.</param>
    /// <returns>A Money with zero amount.</returns>
    public static Money Zero(CurrencyCode currency) => new(0m, currency);

    /// <summary>
    /// Determines whether the left Money is greater than the right Money.
    /// </summary>
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether the left Money is less than the right Money.
    /// </summary>
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether the left Money is greater than or equal to the right Money.
    /// </summary>
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Determines whether the left Money is less than or equal to the right Money.
    /// </summary>
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
}
