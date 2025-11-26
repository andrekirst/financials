namespace Camtify.Domain.Common;

/// <summary>
/// Charge bearer - specifies who bears the charges (XML: ChrgBr).
/// </summary>
public enum ChargeBearer
{
    /// <summary>
    /// Debtor bears all charges (DEBT).
    /// </summary>
    Debtor,

    /// <summary>
    /// Creditor bears all charges (CRED).
    /// </summary>
    Creditor,

    /// <summary>
    /// Charges are shared (SHAR) - SEPA standard.
    /// </summary>
    Shared,

    /// <summary>
    /// Following service level instruction (SLEV).
    /// </summary>
    FollowingServiceLevel
}
