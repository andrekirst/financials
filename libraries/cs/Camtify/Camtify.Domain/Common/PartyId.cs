namespace Camtify.Domain.Common;

/// <summary>
/// Abstract base type for party identification (XML: Id).
/// </summary>
/// <remarks>
/// A party can be identified either as an organisation or as a private person.
/// Use <see cref="OrganisationPartyId"/> for organisations and
/// <see cref="PersonPartyId"/> for natural persons.
/// </remarks>
public abstract record PartyId;
