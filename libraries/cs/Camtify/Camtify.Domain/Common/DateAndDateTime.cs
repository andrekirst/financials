using System.Globalization;

namespace Camtify.Domain.Common;

/// <summary>
/// ISO 20022 date/time (can be date-only or date-time).
/// </summary>
/// <remarks>
/// ISO 20022 distinguishes between:
/// - Dt: Date only (e.g., ValueDate)
/// - DtTm: Date and time (e.g., CreationDateTime)
/// </remarks>
public readonly struct DateAndDateTime : IComparable<DateAndDateTime>, IEquatable<DateAndDateTime>
{
    private readonly DateTime _value;
    private readonly bool _hasTime;

    private DateAndDateTime(DateTime value, bool hasTime)
    {
        _value = value;
        _hasTime = hasTime;
    }

    /// <summary>
    /// Gets a value indicating whether this value has a time component.
    /// </summary>
    public bool HasTime => _hasTime;

    /// <summary>
    /// Gets the value as DateTime (time is 00:00:00 if date-only).
    /// </summary>
    public DateTime AsDateTime => _value;

    /// <summary>
    /// Gets the value as DateOnly (time is ignored).
    /// </summary>
    public DateOnly AsDateOnly => DateOnly.FromDateTime(_value);

    /// <summary>
    /// Creates a DateAndDateTime from a DateTime.
    /// </summary>
    /// <param name="dateTime">The DateTime value.</param>
    /// <returns>A new DateAndDateTime with time component.</returns>
    public static DateAndDateTime FromDateTime(DateTime dateTime)
        => new(dateTime, hasTime: true);

    /// <summary>
    /// Creates a DateAndDateTime from a DateOnly.
    /// </summary>
    /// <param name="date">The DateOnly value.</param>
    /// <returns>A new DateAndDateTime without time component.</returns>
    public static DateAndDateTime FromDate(DateOnly date)
        => new(date.ToDateTime(TimeOnly.MinValue), hasTime: false);

    /// <summary>
    /// Parses a DateAndDateTime from a string (ISO 8601 format).
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed DateAndDateTime.</returns>
    public static DateAndDateTime Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value must not be empty.", nameof(value));
        }

        // Check if time is included
        if (value.Contains('T', StringComparison.Ordinal) || value.Contains(' ', StringComparison.Ordinal))
        {
            return FromDateTime(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
        }

        return FromDate(DateOnly.Parse(value, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Attempts to parse a DateAndDateTime from a string.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">The parsed result if successful.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? value, out DateAndDateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            result = Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public int CompareTo(DateAndDateTime other) => _value.CompareTo(other._value);

    /// <inheritdoc />
    public bool Equals(DateAndDateTime other) => _value == other._value && _hasTime == other._hasTime;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DateAndDateTime other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(_value, _hasTime);

    /// <inheritdoc />
    public override string ToString() => _hasTime
        ? _value.ToString("o", CultureInfo.InvariantCulture)
        : AsDateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    /// <summary>
    /// Determines whether two DateAndDateTime instances are equal.
    /// </summary>
    public static bool operator ==(DateAndDateTime left, DateAndDateTime right) => left.Equals(right);

    /// <summary>
    /// Determines whether two DateAndDateTime instances are not equal.
    /// </summary>
    public static bool operator !=(DateAndDateTime left, DateAndDateTime right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the left DateAndDateTime is less than the right.
    /// </summary>
    public static bool operator <(DateAndDateTime left, DateAndDateTime right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether the left DateAndDateTime is greater than the right.
    /// </summary>
    public static bool operator >(DateAndDateTime left, DateAndDateTime right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether the left DateAndDateTime is less than or equal to the right.
    /// </summary>
    public static bool operator <=(DateAndDateTime left, DateAndDateTime right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether the left DateAndDateTime is greater than or equal to the right.
    /// </summary>
    public static bool operator >=(DateAndDateTime left, DateAndDateTime right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Implicitly converts a DateTime to a DateAndDateTime.
    /// </summary>
    public static implicit operator DateAndDateTime(DateTime dateTime) => FromDateTime(dateTime);

    /// <summary>
    /// Implicitly converts a DateOnly to a DateAndDateTime.
    /// </summary>
    public static implicit operator DateAndDateTime(DateOnly date) => FromDate(date);
}
