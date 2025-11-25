namespace Camtify.Domain.Common;

/// <summary>
/// Proxy/alias identification for an account (XML: Prxy).
/// </summary>
/// <remarks>
/// Used for instant payments (TIPS, SCT Inst) where the account
/// can be identified by an alias such as a phone number, email address,
/// or other proxy identifier.
/// </remarks>
public sealed record ProxyAccountIdentification
{
    /// <summary>
    /// Gets the proxy type (XML: Tp).
    /// </summary>
    public ProxyAccountType? Type { get; init; }

    /// <summary>
    /// Gets the proxy value/identifier (XML: Id).
    /// </summary>
    /// <remarks>
    /// The actual proxy value, such as a phone number, email address, or alias.
    /// Maximum 2048 characters.
    /// </remarks>
    public required string Identification { get; init; }
}
