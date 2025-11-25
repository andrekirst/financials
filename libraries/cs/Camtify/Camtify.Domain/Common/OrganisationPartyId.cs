namespace Camtify.Domain.Common;

/// <summary>
/// Organisation identification wrapper (XML: OrgId).
/// </summary>
/// <remarks>
/// Used when the party is an organisation (company, institution, etc.)
/// rather than a natural person.
/// </remarks>
public sealed record OrganisationPartyId : PartyId
{
    /// <summary>
    /// Gets the organisation identification details.
    /// </summary>
    public required OrganisationIdentification Organisation { get; init; }
}
