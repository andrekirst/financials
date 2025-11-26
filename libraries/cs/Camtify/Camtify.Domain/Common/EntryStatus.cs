using System.ComponentModel;

namespace Camtify.Domain.Common;

/// <summary>
/// Entry status codes according to ISO 20022.
/// </summary>
/// <remarks>
/// Indicates the booking status of an entry on an account statement.
/// </remarks>
public enum EntryStatus
{
    /// <summary>
    /// Booked - Entry has been posted to the account on the account servicer's books.
    /// </summary>
    [Description("BOOK")]
    Booked,

    /// <summary>
    /// Pending - Entry is pending processing.
    /// </summary>
    [Description("PDNG")]
    Pending,

    /// <summary>
    /// Information - Entry is provided for information purposes only.
    /// </summary>
    [Description("INFO")]
    Information,

    /// <summary>
    /// Future - Entry is expected to be booked in the future.
    /// </summary>
    [Description("FUTR")]
    Future
}
