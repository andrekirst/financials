namespace Camtify.Domain.Common;

/// <summary>
/// Identification of a natural person (XML: PrvtId).
/// </summary>
/// <remarks>
/// Used to identify individuals in ISO 20022 messages.
/// Can contain date/place of birth and/or other identification schemes
/// like passport number, driver's license, tax ID, etc.
/// </remarks>
public sealed record PersonIdentification
{
    /// <summary>
    /// Gets the date and place of birth (XML: DtAndPlcOfBirth).
    /// </summary>
    public DateAndPlaceOfBirth? DateAndPlaceOfBirth { get; init; }

    /// <summary>
    /// Gets other identification schemes (XML: Othr).
    /// </summary>
    /// <remarks>
    /// Identification schemes like passport number, driver's license,
    /// national ID, social security number, tax ID, etc.
    /// </remarks>
    public IReadOnlyList<GenericPersonIdentification>? Other { get; init; }
}
