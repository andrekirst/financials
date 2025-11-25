namespace Camtify.Domain.Common;

/// <summary>
/// Preferred method of contact (XML: PrefrdMtd).
/// </summary>
public enum PreferredContactMethod
{
    /// <summary>
    /// Letter/postal mail (LETT).
    /// </summary>
    Letter,

    /// <summary>
    /// Email (MAIL).
    /// </summary>
    Email,

    /// <summary>
    /// Phone call (PHON).
    /// </summary>
    Phone,

    /// <summary>
    /// Fax (FAXX).
    /// </summary>
    Fax,

    /// <summary>
    /// Mobile/cell phone (CELL).
    /// </summary>
    Mobile
}
