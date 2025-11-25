namespace Camtify.Domain.Common;

/// <summary>
/// Currency code according to ISO 4217 (XML: Ccy).
/// </summary>
/// <remarks>
/// Three-letter alphabetic code identifying the currency.
/// Examples: EUR, USD, GBP, CHF, JPY.
/// </remarks>
public sealed record CurrencyCode
{
    /// <summary>
    /// Gets the three-letter currency code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the Euro currency code.
    /// </summary>
    public static CurrencyCode EUR => new() { Code = "EUR" };

    /// <summary>
    /// Gets the US Dollar currency code.
    /// </summary>
    public static CurrencyCode USD => new() { Code = "USD" };

    /// <summary>
    /// Gets the British Pound currency code.
    /// </summary>
    public static CurrencyCode GBP => new() { Code = "GBP" };

    /// <summary>
    /// Gets the Swiss Franc currency code.
    /// </summary>
    public static CurrencyCode CHF => new() { Code = "CHF" };

    /// <summary>
    /// Gets the Japanese Yen currency code.
    /// </summary>
    public static CurrencyCode JPY => new() { Code = "JPY" };

    /// <inheritdoc />
    public override string ToString() => Code;

    /// <summary>
    /// Implicitly converts a CurrencyCode to a string.
    /// </summary>
    public static implicit operator string(CurrencyCode currency) => currency.Code;
}
