namespace Camtify.Domain.Common;

/// <summary>
/// Account identification (IBAN or other) (XML: Id).
/// </summary>
/// <remarks>
/// Abstract base type for account identification.
/// Use <see cref="IbanAccountIdentification"/> for IBAN-based accounts
/// or <see cref="OtherAccountIdentification"/> for other identification schemes.
/// </remarks>
public abstract record AccountIdentification;
