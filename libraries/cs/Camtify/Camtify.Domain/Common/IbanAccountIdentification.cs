namespace Camtify.Domain.Common;

/// <summary>
/// IBAN-based account identification (XML: IBAN).
/// </summary>
/// <remarks>
/// Used when the account is identified by an International Bank Account Number.
/// </remarks>
public sealed record IbanAccountIdentification : AccountIdentification
{
    /// <summary>
    /// Gets the IBAN (XML: IBAN).
    /// </summary>
    public required Iban Iban { get; init; }
}
