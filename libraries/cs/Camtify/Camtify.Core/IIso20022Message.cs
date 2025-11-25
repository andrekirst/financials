namespace Camtify.Core;

/// <summary>
/// Base interface for all ISO 20022 messages.
/// Every concrete message (pain.001, camt.053, etc.) implements this interface.
/// </summary>
/// <remarks>
/// This interface defines the common properties that all ISO 20022 messages share,
/// regardless of their business area (PAIN, CAMT, PACS, etc.).
/// </remarks>
/// <example>
/// <code>
/// public class CustomerCreditTransferInitiation : IIso20022Message
/// {
///     public MessageIdentifier MessageType => "pain.001.001.09";
///     public DateTime CreationDateTime => GroupHeader.CreationDateTime;
///     public string MessageIdentification => GroupHeader.MessageIdentification;
///
///     public GroupHeaderPain GroupHeader { get; init; }
///     // ... other properties
/// }
/// </code>
/// </example>
public interface IIso20022Message
{
    /// <summary>
    /// Unique identifier of the message type (e.g., "pain.001.001.09").
    /// </summary>
    /// <remarks>
    /// The message identifier follows the ISO 20022 naming convention:
    /// [business area].[message].[variant].[version]
    /// Examples:
    /// - pain.001.001.09: Customer Credit Transfer Initiation
    /// - camt.053.001.08: Bank to Customer Statement
    /// - pacs.008.001.08: FI to FI Customer Credit Transfer
    /// </remarks>
    MessageIdentifier MessageType { get; }

    /// <summary>
    /// Creation date and time of the message (from GroupHeader/CreDtTm).
    /// </summary>
    /// <remarks>
    /// This timestamp is taken from the GroupHeader's CreationDateTime field.
    /// It represents when the message was created, not when it was sent or received.
    /// </remarks>
    DateTime CreationDateTime { get; }

    /// <summary>
    /// Message ID from the GroupHeader (MsgId).
    /// </summary>
    /// <remarks>
    /// This identifier is unique within the context of the sender and is used
    /// for message tracking and duplicate detection.
    /// Maximum length: 35 characters (as per ISO 20022 specification).
    /// </remarks>
    string MessageIdentification { get; }
}
