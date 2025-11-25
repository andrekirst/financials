using Camtify.Core;

namespace Camtify.Domain.Common;

/// <summary>
/// Business Application Header (BAH) - Envelope for ISO 20022 messages (XML: AppHdr).
/// </summary>
/// <remarks>
/// <para>
/// The Business Application Header (head.001.001.xx) is a standardized envelope element
/// that precedes the actual ISO 20022 message. It contains routing and metadata information.
/// </para>
/// <para>
/// The BAH is used for:
/// <list type="bullet">
///   <item><description>Routing: Identifies sender (Fr) and receiver (To)</description></item>
///   <item><description>Message identification: Unique business message ID</description></item>
///   <item><description>Duplicate detection: CopyDuplicate and PossibleDuplicate flags</description></item>
///   <item><description>Digital signature: Optional XML-DSIG signature</description></item>
/// </list>
/// </para>
/// <para>
/// XML namespace: urn:iso:std:iso:20022:tech:xsd:head.001.001.02
/// </para>
/// </remarks>
/// <example>
/// Example BAH XML structure:
/// <code>
/// &lt;AppHdr xmlns="urn:iso:std:iso:20022:tech:xsd:head.001.001.02"&gt;
///   &lt;Fr&gt;
///     &lt;FIId&gt;
///       &lt;FinInstnId&gt;
///         &lt;BICFI&gt;DEUTDEFF&lt;/BICFI&gt;
///       &lt;/FinInstnId&gt;
///     &lt;/FIId&gt;
///   &lt;/Fr&gt;
///   &lt;To&gt;
///     &lt;FIId&gt;
///       &lt;FinInstnId&gt;
///         &lt;BICFI&gt;COBADEFF&lt;/BICFI&gt;
///       &lt;/FinInstnId&gt;
///     &lt;/FIId&gt;
///   &lt;/To&gt;
///   &lt;BizMsgIdr&gt;MSG123456789&lt;/BizMsgIdr&gt;
///   &lt;MsgDefIdr&gt;pain.001.001.09&lt;/MsgDefIdr&gt;
///   &lt;BizSvc&gt;swift.cbprplus.02&lt;/BizSvc&gt;
///   &lt;CreDt&gt;2024-01-15T10:30:00Z&lt;/CreDt&gt;
/// &lt;/AppHdr&gt;
/// </code>
/// </example>
public sealed record BusinessApplicationHeader
{
    /// <summary>
    /// Version of the BAH schema (e.g., "head.001.001.01", "head.001.001.02") (XML: xmlns).
    /// </summary>
    /// <remarks>
    /// Derived from the XML namespace. Common versions:
    /// <list type="bullet">
    ///   <item><description>head.001.001.01: Initial version</description></item>
    ///   <item><description>head.001.001.02: Added BizSvc (Business Service) and Rltd (Related)</description></item>
    /// </list>
    /// </remarks>
    public required MessageIdentifier Version { get; init; }

    /// <summary>
    /// Sender of the message (XML: Fr).
    /// </summary>
    /// <remarks>
    /// Can be a financial institution or another party.
    /// Contains identification like BIC, LEI, or clearing system member ID.
    /// </remarks>
    public required Party From { get; init; }

    /// <summary>
    /// Receiver of the message (XML: To).
    /// </summary>
    /// <remarks>
    /// The intended recipient of the message.
    /// Can be a financial institution or another party.
    /// </remarks>
    public required Party To { get; init; }

    /// <summary>
    /// Unique business message identifier (XML: BizMsgIdr).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters, assigned by the sender.
    /// Used for end-to-end tracking and duplicate detection.
    /// Must be unique within the scope of the sender.
    /// </remarks>
    public required string BusinessMessageIdentifier { get; init; }

    /// <summary>
    /// Identifier of the enclosed message type (XML: MsgDefIdr).
    /// </summary>
    /// <remarks>
    /// Specifies the type of the wrapped ISO 20022 message.
    /// Examples: "pain.001.001.09", "camt.053.001.08", "pacs.008.001.08"
    /// </remarks>
    public required string MessageDefinitionIdentifier { get; init; }

    /// <summary>
    /// Business service identifier (XML: BizSvc).
    /// </summary>
    /// <remarks>
    /// Available in head.001.001.02 and later.
    /// Identifies the business service context, e.g., "swift.cbprplus.02".
    /// Used by SWIFT for routing and service identification.
    /// </remarks>
    public string? BusinessService { get; init; }

    /// <summary>
    /// Creation date and time (XML: CreDt).
    /// </summary>
    /// <remarks>
    /// Timestamp when the BAH was created.
    /// Should be in UTC or include timezone information.
    /// Format: ISO 8601 (e.g., "2024-01-15T10:30:00Z")
    /// </remarks>
    public required DateTime CreationDate { get; init; }

    /// <summary>
    /// Character set of the message (XML: CharSet).
    /// </summary>
    /// <remarks>
    /// Optional. Default is UTF-8.
    /// Specifies the character encoding used in the message.
    /// </remarks>
    public string? CharacterSet { get; init; }

    /// <summary>
    /// Copy/duplicate indicator (XML: CpyDplct).
    /// </summary>
    /// <remarks>
    /// Indicates whether this message is the original or a copy/duplicate.
    /// Used for audit and reconciliation purposes.
    /// </remarks>
    public CopyDuplicate? CopyDuplicate { get; init; }

    /// <summary>
    /// Possible duplicate indicator (XML: PssblDplct).
    /// </summary>
    /// <remarks>
    /// True if the sender is not certain whether the message was already sent.
    /// The receiver should check for duplicates when this flag is set.
    /// </remarks>
    public bool? PossibleDuplicate { get; init; }

    /// <summary>
    /// Priority of the message (XML: Prty).
    /// </summary>
    /// <remarks>
    /// Indicates the urgency of the message processing.
    /// </remarks>
    public Priority? Priority { get; init; }

    /// <summary>
    /// Digital signature (XML: Sgntr).
    /// </summary>
    /// <remarks>
    /// Contains XML-DSIG signature for message authentication and integrity.
    /// Optional but may be required by certain market infrastructures.
    /// </remarks>
    public Signature? Signature { get; init; }

    /// <summary>
    /// Reference to a related message (XML: Rltd).
    /// </summary>
    /// <remarks>
    /// Available in head.001.001.02 and later.
    /// Used in request-response scenarios to link related messages.
    /// For example, a status response can reference the original request.
    /// </remarks>
    public BusinessApplicationHeader? Related { get; init; }
}
