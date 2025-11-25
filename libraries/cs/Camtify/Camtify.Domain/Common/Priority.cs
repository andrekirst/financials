namespace Camtify.Domain.Common;

/// <summary>
/// Message priority indicator (XML: Prty).
/// </summary>
/// <remarks>
/// Specifies the priority level for processing the message.
/// Used in the Business Application Header to indicate urgency.
/// </remarks>
public enum Priority
{
    /// <summary>
    /// Normal priority (NORM).
    /// Standard processing without expedited handling.
    /// </summary>
    Normal,

    /// <summary>
    /// High priority (HIGH).
    /// Expedited processing requested.
    /// </summary>
    High
}
