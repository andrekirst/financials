namespace Camtify.Domain.Common;

/// <summary>
/// Other contact channel information (XML: Othr).
/// </summary>
/// <remarks>
/// Used to specify additional contact methods not covered by standard fields
/// like phone, email, or fax.
/// </remarks>
public sealed record OtherContact
{
    /// <summary>
    /// Gets the type of contact channel (XML: ChanlTp).
    /// </summary>
    /// <remarks>
    /// Examples: "SKYPE", "WHATSAPP", "TELEGRAM", "SLACK".
    /// Maximum 4 characters.
    /// </remarks>
    public required string ChannelType { get; init; }

    /// <summary>
    /// Gets the identification/address on the channel (XML: Id).
    /// </summary>
    /// <remarks>
    /// The user identifier or address on the specified channel.
    /// Maximum 128 characters.
    /// </remarks>
    public required string Identification { get; init; }
}
