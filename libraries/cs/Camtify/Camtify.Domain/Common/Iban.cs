using System.Text;
using System.Text.RegularExpressions;

namespace Camtify.Domain.Common;

/// <summary>
/// International Bank Account Number (IBAN).
/// ISO 13616 Standard.
/// </summary>
/// <remarks>
/// IBAN structure: [Country Code 2][Check Digits 2][BBAN variable]
/// Example: DE89 3704 0044 0532 0130 00
/// - DE = Germany
/// - 89 = Check digits
/// - 37040044 = Bank code (Bankleitzahl)
/// - 0532013000 = Account number
/// </remarks>
public sealed partial class Iban : IEquatable<Iban>
{
    private readonly string _value;

    private Iban(string value) => _value = value;

    /// <summary>
    /// Gets the country code (first 2 characters).
    /// ISO 3166-1 alpha-2.
    /// </summary>
    public string CountryCode => _value[..2];

    /// <summary>
    /// Gets the check digits (characters 3-4).
    /// </summary>
    public string CheckDigits => _value[2..4];

    /// <summary>
    /// Gets the BBAN (from character 5 onwards).
    /// Basic Bank Account Number, country-specific format.
    /// </summary>
    public string Bban => _value[4..];

    /// <summary>
    /// Gets the total length of the IBAN.
    /// </summary>
    public int Length => _value.Length;

    /// <summary>
    /// Gets the formatted IBAN (groups of four with spaces).
    /// </summary>
    /// <example>DE89 3704 0044 0532 0130 00</example>
    public string Formatted => string.Join(
        " ",
        Enumerable.Range(0, (int)Math.Ceiling(_value.Length / 4.0))
            .Select(i => _value.Substring(i * 4, Math.Min(4, _value.Length - (i * 4)))));

    /// <summary>
    /// Gets the electronic format (without spaces).
    /// </summary>
    public string Electronic => _value;

    /// <summary>
    /// Parses and validates an IBAN.
    /// </summary>
    /// <param name="value">IBAN string (with or without spaces).</param>
    /// <returns>Validated IBAN object.</returns>
    /// <exception cref="ArgumentException">If IBAN is invalid.</exception>
    public static Iban Parse(string value)
    {
        if (!TryParse(value, out var iban, out var error))
        {
            throw new ArgumentException(error, nameof(value));
        }

        return iban!;
    }

    /// <summary>
    /// Attempts to parse an IBAN.
    /// </summary>
    /// <param name="value">The IBAN string to parse.</param>
    /// <param name="iban">The parsed IBAN if successful.</param>
    /// <param name="error">The error message if parsing failed.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? value, out Iban? iban, out string? error)
    {
        iban = null;
        error = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "IBAN must not be empty.";
            return false;
        }

        // Normalize: remove spaces, convert to uppercase
        var normalized = value.Replace(" ", "", StringComparison.Ordinal)
            .Replace("-", "", StringComparison.Ordinal)
            .ToUpperInvariant();

        // Length check (15-34 characters depending on country)
        if (normalized.Length < 15 || normalized.Length > 34)
        {
            error = $"Invalid IBAN length: {normalized.Length} characters (expected: 15-34).";
            return false;
        }

        // Format: 2 letters + 2 digits + alphanumeric
        if (!IbanFormatRegex().IsMatch(normalized))
        {
            error = "Invalid IBAN format. Expected: 2 letters + 2 digits + alphanumeric.";
            return false;
        }

        // Checksum validation (ISO 7064 Mod 97-10)
        if (!ValidateChecksum(normalized))
        {
            error = "Invalid IBAN checksum.";
            return false;
        }

        iban = new Iban(normalized);
        return true;
    }

    /// <summary>
    /// Attempts to parse an IBAN.
    /// </summary>
    /// <param name="value">The IBAN string to parse.</param>
    /// <param name="iban">The parsed IBAN if successful.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? value, out Iban? iban)
    {
        return TryParse(value, out iban, out _);
    }

    private static bool ValidateChecksum(string iban)
    {
        // Rearrange IBAN: BBAN + Country Code + Check Digits
        var rearranged = iban[4..] + iban[..4];

        // Convert letters to numbers (A=10, B=11, ..., Z=35)
        var numericString = new StringBuilder();
        foreach (var c in rearranged)
        {
            if (char.IsDigit(c))
            {
                numericString.Append(c);
            }
            else
            {
                numericString.Append(c - 'A' + 10);
            }
        }

        // Calculate modulo 97 (large number, so calculate piecewise)
        var remainder = 0;
        foreach (var c in numericString.ToString())
        {
            remainder = ((remainder * 10) + (c - '0')) % 97;
        }

        return remainder == 1;
    }

    /// <inheritdoc />
    public override string ToString() => _value;

    /// <inheritdoc />
    public bool Equals(Iban? other) => other is not null && _value == other._value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Iban other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two IBAN instances are equal.
    /// </summary>
    public static bool operator ==(Iban? left, Iban? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>
    /// Determines whether two IBAN instances are not equal.
    /// </summary>
    public static bool operator !=(Iban? left, Iban? right) => !(left == right);

    /// <summary>
    /// Implicitly converts an IBAN to a string.
    /// </summary>
    public static implicit operator string(Iban iban) => iban._value;

    [GeneratedRegex(@"^[A-Z]{2}\d{2}[A-Z0-9]+$")]
    private static partial Regex IbanFormatRegex();
}
