namespace Camtify.Core;

/// <summary>
/// Common properties of all GroupHeader (GrpHdr) elements.
/// Every ISO 20022 message starts with a GroupHeader.
/// </summary>
/// <remarks>
/// The GroupHeader (GrpHdr) is a mandatory element in all ISO 20022 payment messages.
/// It contains common information applicable to the entire message, such as:
/// - Unique message identification
/// - Creation timestamp
/// - Number of transactions
/// - Control sum for validation
///
/// Each business area may extend this interface with additional specific fields.
/// </remarks>
/// <example>
/// <code>
/// public class GroupHeaderPain : IGroupHeader
/// {
///     public string MessageIdentification { get; init; }
///     public DateTime CreationDateTime { get; init; }
///     public int NumberOfTransactions { get; init; }
///     public decimal? ControlSum { get; init; }
///
///     // PAIN-specific fields
///     public PartyIdentification InitiatingParty { get; init; }
/// }
/// </code>
/// </example>
public interface IGroupHeader
{
    /// <summary>
    /// Unique message ID (XML: MsgId).
    /// Maximum 35 characters, assigned by sender.
    /// </summary>
    /// <remarks>
    /// The MessageIdentification is assigned by the party creating the message.
    /// It must be unique within the context of the sender and is used for:
    /// - Message tracking and correlation
    /// - Duplicate detection
    /// - Error reporting and status updates
    ///
    /// Maximum length: 35 characters (as per ISO 20022 specification).
    /// Pattern: [A-Za-z0-9/\-?:().,'+\s]{1,35}
    /// </remarks>
    string MessageIdentification { get; }

    /// <summary>
    /// Creation date and time (XML: CreDtTm).
    /// ISO 8601 format: 2024-01-15T10:30:00Z
    /// </summary>
    /// <remarks>
    /// The creation date and time indicates when the message was created by the initiating party.
    /// It should be expressed in UTC or include timezone information.
    /// Format: ISO 8601 (e.g., "2024-01-15T10:30:00Z" or "2024-01-15T10:30:00+01:00")
    /// </remarks>
    DateTime CreationDateTime { get; }

    /// <summary>
    /// Number of transactions (XML: NbOfTxs).
    /// Total count of all individual transactions contained.
    /// </summary>
    /// <remarks>
    /// The NumberOfTransactions indicates the total count of individual transactions
    /// contained in the message. This is used for validation and integrity checking.
    ///
    /// For example:
    /// - In pain.001: Number of payment instructions × number of transactions per instruction
    /// - In camt.053: Total number of entries in all statements
    ///
    /// The receiving party should verify that the actual number of transactions
    /// matches this declared value.
    /// </remarks>
    int NumberOfTransactions { get; }

    /// <summary>
    /// Control sum of all amounts (XML: CtrlSum).
    /// Optional, used for integrity checking.
    /// </summary>
    /// <remarks>
    /// The ControlSum is the total of all individual amounts in the message.
    /// It is optional but highly recommended for integrity checking.
    ///
    /// Calculation rules:
    /// - Sum all transaction amounts in the message
    /// - Do not include any charges or fees
    /// - Use the original currency amounts (before any conversion)
    ///
    /// The receiving party should verify that the sum of all transaction amounts
    /// matches this control sum. A mismatch indicates a transmission error or data corruption.
    ///
    /// Set to null if not provided in the message.
    /// </remarks>
    decimal? ControlSum { get; }
}
