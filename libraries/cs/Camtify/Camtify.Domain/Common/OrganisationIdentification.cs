namespace Camtify.Domain.Common;

/// <summary>
/// Organisation identification (XML: OrgId).
/// </summary>
/// <remarks>
/// Identifies a non-financial institution organisation.
/// Used in the Business Application Header when the sender or receiver
/// is not a financial institution.
/// </remarks>
public sealed record OrganisationIdentification
{
    /// <summary>
    /// Any Business Identifier Code (BIC) (XML: AnyBIC).
    /// </summary>
    /// <remarks>
    /// BIC assigned to a non-financial institution.
    /// </remarks>
    public string? AnyBic { get; init; }

    /// <summary>
    /// Legal Entity Identifier (LEI) as defined by ISO 17442 (XML: LEI).
    /// </summary>
    /// <remarks>
    /// A 20-character alphanumeric code that uniquely identifies
    /// legally distinct entities engaged in financial transactions.
    /// </remarks>
    public string? Lei { get; init; }

    /// <summary>
    /// Other identification schemes (XML: Othr).
    /// </summary>
    /// <remarks>
    /// Proprietary identification schemes not covered by standard types.
    /// Multiple identifications can be provided.
    /// </remarks>
    public IReadOnlyList<GenericIdentification>? Other { get; init; }
}
