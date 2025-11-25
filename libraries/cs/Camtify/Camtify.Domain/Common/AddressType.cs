namespace Camtify.Domain.Common;

/// <summary>
/// Type of address (XML: AdrTp).
/// </summary>
public enum AddressType
{
    /// <summary>
    /// Postal address (ADDR).
    /// </summary>
    Postal,

    /// <summary>
    /// Post office box (PBOX).
    /// </summary>
    POBox,

    /// <summary>
    /// Residential address (HOME).
    /// </summary>
    Home,

    /// <summary>
    /// Business address (BIZZ).
    /// </summary>
    Business,

    /// <summary>
    /// Mail to address (MLTO).
    /// </summary>
    MailTo,

    /// <summary>
    /// Delivery address (DLVY).
    /// </summary>
    Delivery
}
