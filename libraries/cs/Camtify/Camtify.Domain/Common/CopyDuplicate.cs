namespace Camtify.Domain.Common;

/// <summary>
/// Indicator whether message is original or copy/duplicate (XML: CpyDplct).
/// </summary>
/// <remarks>
/// Used in the Business Application Header to identify whether a message
/// is the original transmission or a copy/duplicate for reference purposes.
/// </remarks>
public enum CopyDuplicate
{
    /// <summary>
    /// Copy of the original message (COPY).
    /// The message is a copy sent for information purposes.
    /// </summary>
    Copy,

    /// <summary>
    /// Duplicate of an already sent message (DUPL).
    /// The message is a re-transmission of a previously sent message.
    /// </summary>
    Duplicate
}
