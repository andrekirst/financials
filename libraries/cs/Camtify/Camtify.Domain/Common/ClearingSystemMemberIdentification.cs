namespace Camtify.Domain.Common;

/// <summary>
/// Clearing system member identification (XML: ClrSysMmbId).
/// </summary>
/// <remarks>
/// Identifies a member within a specific clearing system.
/// Used as an alternative to BIC for domestic clearing systems.
/// </remarks>
public sealed record ClearingSystemMemberIdentification
{
    /// <summary>
    /// Clearing system identification (XML: ClrSysId).
    /// </summary>
    /// <remarks>
    /// Identifies the clearing system, e.g.:
    /// - "USABA" for American Bankers Association routing number
    /// - "DEBLZ" for German Bankleitzahl
    /// - "GBDSC" for UK Sort Code
    /// - "ATBLZ" for Austrian Bankleitzahl
    /// - "CHBCC" for Swiss BC Number
    /// </remarks>
    public string? ClearingSystemId { get; init; }

    /// <summary>
    /// Member identification within the clearing system (XML: MmbId).
    /// </summary>
    public required string MemberId { get; init; }
}
