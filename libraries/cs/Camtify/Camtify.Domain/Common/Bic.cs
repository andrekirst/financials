using System.Text.RegularExpressions;

namespace Camtify.Domain.Common;

/// <summary>
/// Business Identifier Code (BIC).
/// ISO 9362 Standard (also known as SWIFT code).
/// </summary>
/// <remarks>
/// BIC structure (8 or 11 characters):
/// - Institution Code (4 characters): Bank identifier
/// - Country Code (2 characters): ISO 3166-1 alpha-2
/// - Location Code (2 characters): City/Region
/// - Branch Code (3 characters, optional): Branch (XXX = head office)
///
/// Example: DEUTDEFF or DEUTDEFFXXX
/// - DEUT = Deutsche Bank
/// - DE = Germany
/// - FF = Frankfurt
/// - XXX = Head office (optional)
/// </remarks>
public sealed partial class Bic : IEquatable<Bic>
{
    private readonly string _value;

    private Bic(string value) => _value = value;

    /// <summary>
    /// Gets the institution code (first 4 characters).
    /// Identifies the bank/institution.
    /// </summary>
    public string InstitutionCode => _value[..4];

    /// <summary>
    /// Gets the country code (characters 5-6).
    /// ISO 3166-1 alpha-2.
    /// </summary>
    public string CountryCode => _value[4..6];

    /// <summary>
    /// Gets the location code (characters 7-8).
    /// City or region.
    /// </summary>
    public string LocationCode => _value[6..8];

    /// <summary>
    /// Gets the branch code (characters 9-11, optional).
    /// XXX = head office, otherwise branch.
    /// </summary>
    public string? BranchCode => _value.Length == 11 ? _value[8..] : null;

    /// <summary>
    /// Gets a value indicating whether this is a BIC8 (without branch code).
    /// </summary>
    public bool IsBic8 => _value.Length == 8;

    /// <summary>
    /// Gets a value indicating whether this is a BIC11 (with branch code).
    /// </summary>
    public bool IsBic11 => _value.Length == 11;

    /// <summary>
    /// Gets the BIC as BIC11 (appends XXX if BIC8).
    /// </summary>
    public string AsBic11 => IsBic8 ? _value + "XXX" : _value;

    /// <summary>
    /// Gets the BIC as BIC8 (removes branch code if XXX).
    /// </summary>
    public string AsBic8 => IsBic11 && BranchCode == "XXX" ? _value[..8] : _value;

    /// <summary>
    /// Parses and validates a BIC.
    /// </summary>
    /// <param name="value">BIC string.</param>
    /// <returns>Validated BIC object.</returns>
    /// <exception cref="ArgumentException">If BIC is invalid.</exception>
    public static Bic Parse(string value)
    {
        if (!TryParse(value, out var bic, out var error))
        {
            throw new ArgumentException(error, nameof(value));
        }

        return bic!;
    }

    /// <summary>
    /// Attempts to parse a BIC.
    /// </summary>
    /// <param name="value">The BIC string to parse.</param>
    /// <param name="bic">The parsed BIC if successful.</param>
    /// <param name="error">The error message if parsing failed.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? value, out Bic? bic, out string? error)
    {
        bic = null;
        error = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "BIC must not be empty.";
            return false;
        }

        var normalized = value.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant();

        // Length: 8 or 11 characters
        if (normalized.Length != 8 && normalized.Length != 11)
        {
            error = $"Invalid BIC length: {normalized.Length} characters (expected: 8 or 11).";
            return false;
        }

        // Format: 4 letters + 2 letters + 2 alphanumeric + optional 3 alphanumeric
        var pattern = normalized.Length == 8
            ? Bic8FormatRegex()
            : Bic11FormatRegex();

        if (!pattern.IsMatch(normalized))
        {
            error = "Invalid BIC format.";
            return false;
        }

        bic = new Bic(normalized);
        return true;
    }

    /// <summary>
    /// Attempts to parse a BIC.
    /// </summary>
    /// <param name="value">The BIC string to parse.</param>
    /// <param name="bic">The parsed BIC if successful.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? value, out Bic? bic)
    {
        return TryParse(value, out bic, out _);
    }

    /// <inheritdoc />
    public override string ToString() => _value;

    /// <inheritdoc />
    public bool Equals(Bic? other) => other is not null && AsBic11 == other.AsBic11;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Bic other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => AsBic11.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two BIC instances are equal.
    /// </summary>
    public static bool operator ==(Bic? left, Bic? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>
    /// Determines whether two BIC instances are not equal.
    /// </summary>
    public static bool operator !=(Bic? left, Bic? right) => !(left == right);

    /// <summary>
    /// Implicitly converts a BIC to a string.
    /// </summary>
    public static implicit operator string(Bic bic) => bic._value;

    [GeneratedRegex(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}$")]
    private static partial Regex Bic8FormatRegex();

    [GeneratedRegex(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}[A-Z0-9]{3}$")]
    private static partial Regex Bic11FormatRegex();
}
