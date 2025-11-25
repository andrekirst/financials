namespace Camtify.Domain.Common;

/// <summary>
/// Name prefix/salutation for a person (XML: NmPrfx).
/// </summary>
public enum NamePrefix
{
    /// <summary>
    /// Doctor (DOCT).
    /// </summary>
    Doctor,

    /// <summary>
    /// Madame (MADM).
    /// </summary>
    Madame,

    /// <summary>
    /// Miss (MISS).
    /// </summary>
    Miss,

    /// <summary>
    /// Mister (MIST).
    /// </summary>
    Mister,

    /// <summary>
    /// Gender-neutral title (MX) - available in newer ISO 20022 versions.
    /// </summary>
    Mx
}
