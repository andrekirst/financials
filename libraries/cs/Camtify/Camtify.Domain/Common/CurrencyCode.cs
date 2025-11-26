namespace Camtify.Domain.Common;

/// <summary>
/// Currency code according to ISO 4217 (XML: Ccy).
/// </summary>
/// <remarks>
/// Three-letter alphabetic code identifying the currency.
/// Examples: EUR, USD, GBP, CHF, JPY.
/// </remarks>
public readonly struct CurrencyCode : IEquatable<CurrencyCode>
{
    private readonly string _code;

    private CurrencyCode(string code) => _code = code;

    /// <summary>
    /// Gets the three-letter currency code.
    /// </summary>
    public string Code => _code ?? string.Empty;

    /// <summary>
    /// Gets the Euro currency code.
    /// </summary>
    public static readonly CurrencyCode EUR = new("EUR");

    /// <summary>
    /// Gets the US Dollar currency code.
    /// </summary>
    public static readonly CurrencyCode USD = new("USD");

    /// <summary>
    /// Gets the British Pound currency code.
    /// </summary>
    public static readonly CurrencyCode GBP = new("GBP");

    /// <summary>
    /// Gets the Swiss Franc currency code.
    /// </summary>
    public static readonly CurrencyCode CHF = new("CHF");

    /// <summary>
    /// Gets the Japanese Yen currency code.
    /// </summary>
    public static readonly CurrencyCode JPY = new("JPY");

    /// <summary>
    /// Gets the Chinese Yuan currency code.
    /// </summary>
    public static readonly CurrencyCode CNY = new("CNY");

    /// <summary>
    /// Gets the Australian Dollar currency code.
    /// </summary>
    public static readonly CurrencyCode AUD = new("AUD");

    /// <summary>
    /// Gets the Canadian Dollar currency code.
    /// </summary>
    public static readonly CurrencyCode CAD = new("CAD");

    /// <summary>
    /// Gets the Swedish Krona currency code.
    /// </summary>
    public static readonly CurrencyCode SEK = new("SEK");

    /// <summary>
    /// Gets the Norwegian Krone currency code.
    /// </summary>
    public static readonly CurrencyCode NOK = new("NOK");

    /// <summary>
    /// Gets the Danish Krone currency code.
    /// </summary>
    public static readonly CurrencyCode DKK = new("DKK");

    /// <summary>
    /// Gets the Polish Zloty currency code.
    /// </summary>
    public static readonly CurrencyCode PLN = new("PLN");

    /// <summary>
    /// Gets the Czech Koruna currency code.
    /// </summary>
    public static readonly CurrencyCode CZK = new("CZK");

    /// <summary>
    /// Gets the Hungarian Forint currency code.
    /// </summary>
    public static readonly CurrencyCode HUF = new("HUF");

    /// <summary>
    /// Gets the number of decimal places for this currency.
    /// </summary>
    /// <remarks>
    /// Most currencies use 2 decimal places. Some exceptions:
    /// - JPY, KRW, VND: 0 decimal places
    /// - BHD, KWD, OMR: 3 decimal places
    /// </remarks>
    public int DecimalPlaces => _code switch
    {
        "JPY" or "KRW" or "VND" => 0,
        "BHD" or "KWD" or "OMR" => 3,
        _ => 2
    };

    /// <summary>
    /// Parses and validates a currency code.
    /// </summary>
    /// <param name="code">The currency code string.</param>
    /// <returns>A validated CurrencyCode.</returns>
    /// <exception cref="ArgumentException">If the currency code is invalid.</exception>
    public static CurrencyCode Parse(string code)
    {
        if (!TryParse(code, out var currency, out var error))
        {
            throw new ArgumentException(error, nameof(code));
        }

        return currency;
    }

    /// <summary>
    /// Attempts to parse a currency code.
    /// </summary>
    /// <param name="code">The currency code string to parse.</param>
    /// <param name="currency">The parsed currency code if successful.</param>
    /// <param name="error">The error message if parsing failed.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? code, out CurrencyCode currency, out string? error)
    {
        currency = default;
        error = null;

        if (string.IsNullOrWhiteSpace(code))
        {
            error = "Currency code must not be empty.";
            return false;
        }

        var normalized = code.Trim().ToUpperInvariant();

        if (normalized.Length != 3 || !normalized.All(char.IsLetter))
        {
            error = "Currency code must consist of 3 letters.";
            return false;
        }

        currency = new CurrencyCode(normalized);
        return true;
    }

    /// <summary>
    /// Attempts to parse a currency code.
    /// </summary>
    /// <param name="code">The currency code string to parse.</param>
    /// <param name="currency">The parsed currency code if successful.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? code, out CurrencyCode currency)
    {
        return TryParse(code, out currency, out _);
    }

    /// <inheritdoc />
    public override string ToString() => _code ?? string.Empty;

    /// <inheritdoc />
    public bool Equals(CurrencyCode other) => _code == other._code;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CurrencyCode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _code?.GetHashCode(StringComparison.Ordinal) ?? 0;

    /// <summary>
    /// Determines whether two CurrencyCode instances are equal.
    /// </summary>
    public static bool operator ==(CurrencyCode left, CurrencyCode right) => left.Equals(right);

    /// <summary>
    /// Determines whether two CurrencyCode instances are not equal.
    /// </summary>
    public static bool operator !=(CurrencyCode left, CurrencyCode right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a CurrencyCode to a string.
    /// </summary>
    public static implicit operator string(CurrencyCode currency) => currency._code ?? string.Empty;
}
