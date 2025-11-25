namespace Camtify.Domain.Common;

/// <summary>
/// Proxy account type (XML: Tp).
/// </summary>
/// <remarks>
/// Specifies the type of proxy identifier used for an account.
/// Either Code or Proprietary should be provided, not both.
/// </remarks>
public sealed record ProxyAccountType
{
    /// <summary>
    /// Gets the code for the proxy type (XML: Cd).
    /// </summary>
    /// <remarks>
    /// Common codes:
    /// - TELE = Telephone number
    /// - EMAL = Email address
    /// - DNAM = Display name
    /// - BANK = Bank ID
    /// - BIID = Buyer ID
    /// - PRVD = Provider ID
    /// - CINC = Corporate Identification Number Code
    /// </remarks>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the proprietary proxy type (XML: Prtry).
    /// </summary>
    /// <remarks>
    /// Used when the proxy type is not covered by the standard code list.
    /// Maximum 35 characters.
    /// </remarks>
    public string? Proprietary { get; init; }
}
