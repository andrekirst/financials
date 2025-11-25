namespace Camtify.Domain.Common;

/// <summary>
/// Party in the Business Application Header (sender or receiver) (XML: Fr, To).
/// </summary>
/// <remarks>
/// Represents either the sender (From) or receiver (To) of an ISO 20022 message.
/// A party can be identified either as a financial institution or as an organisation.
/// At least one of the identification types should be provided.
/// </remarks>
public sealed record Party
{
    /// <summary>
    /// Identification as a financial institution (XML: FIId).
    /// </summary>
    /// <remarks>
    /// Used when the party is a bank or other financial institution.
    /// Contains BIC, LEI, clearing system ID, or other financial institution identifiers.
    /// </remarks>
    public FinancialInstitutionIdentification? FinancialInstitutionId { get; init; }

    /// <summary>
    /// Identification as an organisation (XML: OrgId).
    /// </summary>
    /// <remarks>
    /// Used when the party is a non-financial institution organisation.
    /// Contains organisation-specific identifiers like LEI or proprietary IDs.
    /// </remarks>
    public OrganisationIdentification? OrganisationId { get; init; }
}
