namespace Camtify.Domain.Common;

/// <summary>
/// Other (non-IBAN) account identification (XML: Othr).
/// </summary>
/// <remarks>
/// Used when the account is not identified by an IBAN,
/// such as BBAN, US account numbers, or proprietary schemes.
/// </remarks>
public sealed record OtherAccountIdentification : AccountIdentification
{
    /// <summary>
    /// Gets the generic account identification (XML: Othr).
    /// </summary>
    public required GenericAccountIdentification Other { get; init; }
}
