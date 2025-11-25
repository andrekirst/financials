namespace Camtify.Domain.Common;

/// <summary>
/// Identification of a financial institution (XML: FinInstnId).
/// </summary>
/// <remarks>
/// Used to identify banks and other financial institutions in ISO 20022 messages.
/// The primary identifier is the BIC (Bank Identifier Code), but alternatives
/// such as clearing system member IDs are also supported.
/// </remarks>
public sealed record FinancialInstitutionIdentification
{
    /// <summary>
    /// BIC (Bank Identifier Code) as defined by ISO 9362 (XML: BICFI).
    /// </summary>
    /// <remarks>
    /// The BIC is an 8 or 11 character code that uniquely identifies a financial institution.
    /// Format: BBBBCCLL[XXX]
    /// - BBBB: Institution code (4 letters)
    /// - CC: Country code (2 letters, ISO 3166-1)
    /// - LL: Location code (2 alphanumeric)
    /// - XXX: Branch code (optional, 3 alphanumeric)
    /// Examples: "DEUTDEFF", "COBADEFFXXX"
    /// </remarks>
    public string? Bic { get; init; }

    /// <summary>
    /// Clearing system member identification (XML: ClrSysMmbId).
    /// </summary>
    /// <remarks>
    /// Alternative identification through a clearing system.
    /// Used when BIC is not available or for domestic clearing systems.
    /// </remarks>
    public ClearingSystemMemberIdentification? ClearingSystemMemberId { get; init; }

    /// <summary>
    /// Legal Entity Identifier (LEI) as defined by ISO 17442 (XML: LEI).
    /// </summary>
    /// <remarks>
    /// A 20-character alphanumeric code that uniquely identifies
    /// legally distinct entities engaged in financial transactions.
    /// </remarks>
    public string? Lei { get; init; }

    /// <summary>
    /// Name of the financial institution (XML: Nm).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Postal address of the financial institution (XML: PstlAdr).
    /// </summary>
    public PostalAddress? PostalAddress { get; init; }

    /// <summary>
    /// Other proprietary identification (XML: Othr).
    /// </summary>
    public GenericIdentification? Other { get; init; }
}
