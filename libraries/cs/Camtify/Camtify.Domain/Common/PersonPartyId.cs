namespace Camtify.Domain.Common;

/// <summary>
/// Person identification wrapper (XML: PrvtId).
/// </summary>
/// <remarks>
/// Used when the party is a natural person rather than an organisation.
/// </remarks>
public sealed record PersonPartyId : PartyId
{
    /// <summary>
    /// Gets the person identification details.
    /// </summary>
    public required PersonIdentification Person { get; init; }
}
