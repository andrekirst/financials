namespace Camtify.Domain.Common;

/// <summary>
/// Exchange rate type (XML: RateTp).
/// </summary>
public enum ExchangeRateType
{
    /// <summary>
    /// Spot rate (SPOT).
    /// </summary>
    Spot,

    /// <summary>
    /// Forward rate (FRWD).
    /// </summary>
    Forward,

    /// <summary>
    /// Agreed rate (AGRD).
    /// </summary>
    Agreed
}
