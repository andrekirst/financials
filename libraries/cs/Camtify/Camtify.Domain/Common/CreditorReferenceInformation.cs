using System.Text;

namespace Camtify.Domain.Common;

/// <summary>
/// Creditor reference information for automatic payment matching.
/// </summary>
/// <remarks>
/// XML Element: CdtrRefInf within Strd.
/// The RF reference (ISO 11649) is particularly important for automatic matching.
/// Format: RF[CheckDigits][Reference]
/// Example: RF18539007547034
/// </remarks>
public readonly record struct CreditorReferenceInformation
{
    /// <summary>
    /// Gets the reference type.
    /// </summary>
    public CreditorReferenceType? Type { get; }

    /// <summary>
    /// Gets the reference value.
    /// </summary>
    /// <remarks>Max 35 characters.</remarks>
    public string? Reference { get; }

    /// <summary>
    /// Gets a value indicating whether this is an RF reference (ISO 11649).
    /// </summary>
    public bool IsStructuredCreditorReference =>
        Reference?.StartsWith("RF", StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreditorReferenceInformation"/> struct.
    /// </summary>
    /// <param name="type">The reference type.</param>
    /// <param name="reference">The reference value.</param>
    public CreditorReferenceInformation(CreditorReferenceType? type = null, string? reference = null)
    {
        Type = type;
        Reference = reference;
    }

    /// <summary>
    /// Creates a CreditorReferenceInformation for an RF reference.
    /// </summary>
    /// <param name="rfReference">The RF reference (e.g., RF18539007547034).</param>
    /// <returns>A new CreditorReferenceInformation.</returns>
    public static CreditorReferenceInformation ForRfReference(string rfReference)
    {
        return new CreditorReferenceInformation(
            type: CreditorReferenceType.ForStructuredReference(),
            reference: rfReference);
    }

    /// <summary>
    /// Creates a CreditorReferenceInformation for a generic reference.
    /// </summary>
    /// <param name="reference">The reference value.</param>
    /// <param name="type">Optional reference type.</param>
    /// <returns>A new CreditorReferenceInformation.</returns>
    public static CreditorReferenceInformation ForReference(string reference, CreditorReferenceType? type = null)
    {
        return new CreditorReferenceInformation(type: type, reference: reference);
    }

    /// <summary>
    /// Generates an RF reference from a base reference string.
    /// </summary>
    /// <param name="baseReference">The base reference (alphanumeric).</param>
    /// <returns>The complete RF reference with check digits.</returns>
    public static string GenerateRfReference(string baseReference)
    {
        if (string.IsNullOrWhiteSpace(baseReference))
        {
            throw new ArgumentException("Base reference cannot be null or empty.", nameof(baseReference));
        }

        // Remove any non-alphanumeric characters
        var cleanReference = new string(baseReference.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

        if (cleanReference.Length == 0 || cleanReference.Length > 21)
        {
            throw new ArgumentException("Base reference must be 1-21 alphanumeric characters.", nameof(baseReference));
        }

        // Calculate check digits: reference + RF00, then 98 - (mod 97)
        var numericString = ConvertToNumericString(cleanReference + "RF00");
        var checkDigits = 98 - Mod97(numericString);

        return $"RF{checkDigits:D2}{cleanReference}";
    }

    /// <summary>
    /// Validates the RF reference according to ISO 11649.
    /// </summary>
    /// <returns>True if the RF reference is valid, false otherwise.</returns>
    public bool ValidateRfReference()
    {
        if (!IsStructuredCreditorReference || Reference is null || Reference.Length < 5)
        {
            return false;
        }

        // RF + 2 check digits + reference (min 1 char)
        var checkDigits = Reference[2..4];
        var baseReference = Reference[4..];

        // Validate format: check digits must be numeric
        if (!int.TryParse(checkDigits, out var expectedCheckDigits))
        {
            return false;
        }

        // Validate characters: only alphanumeric allowed in reference
        if (!baseReference.All(c => char.IsLetterOrDigit(c)))
        {
            return false;
        }

        // Calculate checksum: rearrange to reference + RF + check digits
        var numericString = ConvertToNumericString(baseReference + "RF" + checkDigits);
        var remainder = Mod97(numericString);

        // Valid if remainder is 1
        return remainder == 1;
    }

    /// <summary>
    /// Gets a validation result with detailed error information.
    /// </summary>
    /// <returns>A tuple indicating validity and any error message.</returns>
    public (bool IsValid, string? ErrorMessage) GetValidationResult()
    {
        if (Reference is null)
        {
            return (false, "Reference is null");
        }

        if (!Reference.StartsWith("RF", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Reference must start with 'RF'");
        }

        if (Reference.Length < 5)
        {
            return (false, "Reference too short (minimum 5 characters: RF + 2 check digits + reference)");
        }

        if (Reference.Length > 25)
        {
            return (false, "Reference too long (maximum 25 characters)");
        }

        var checkDigits = Reference[2..4];
        if (!int.TryParse(checkDigits, out _))
        {
            return (false, "Check digits must be numeric");
        }

        var baseReference = Reference[4..];
        if (!baseReference.All(c => char.IsLetterOrDigit(c)))
        {
            return (false, "Reference part must be alphanumeric only");
        }

        if (!ValidateRfReference())
        {
            return (false, "Invalid check digits (checksum validation failed)");
        }

        return (true, null);
    }

    /// <summary>
    /// Converts an alphanumeric string to its numeric representation for MOD 97 calculation.
    /// </summary>
    private static string ConvertToNumericString(string input)
    {
        var result = new StringBuilder();
        foreach (var c in input.ToUpperInvariant())
        {
            if (char.IsDigit(c))
            {
                result.Append(c);
            }
            else if (char.IsLetter(c))
            {
                // A=10, B=11, ..., Z=35
                result.Append(c - 'A' + 10);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Calculates MOD 97 for large numbers using piece-wise calculation.
    /// </summary>
    private static long Mod97(string numericString)
    {
        long remainder = 0;
        foreach (var c in numericString)
        {
            remainder = ((remainder * 10) + (c - '0')) % 97;
        }

        return remainder;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (Reference is null)
        {
            return "Empty";
        }

        var validity = IsStructuredCreditorReference && ValidateRfReference() ? "Valid" : "Standard";
        return $"{Reference} ({validity})";
    }
}
