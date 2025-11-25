namespace Camtify.Core;

/// <summary>
/// Represents a complete ISO 20022 document.
/// Encapsulates the actual message and optional Business Application Header.
/// </summary>
/// <typeparam name="TMessage">The concrete message type</typeparam>
/// <remarks>
/// An ISO 20022 document consists of:
/// - An XML namespace that identifies the message type and version
/// - An optional Business Application Header (BAH) for routing and tracking
/// - The actual message content (e.g., CustomerCreditTransferInitiation, BankToCustomerStatement)
///
/// The document represents the complete XML structure that is transmitted between systems.
/// </remarks>
/// <example>
/// <code>
/// public class Pain001Document : IIso20022Document&lt;CustomerCreditTransferInitiation&gt;
/// {
///     public string Namespace => "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09";
///     public BusinessApplicationHeader? Header { get; init; }
///     public CustomerCreditTransferInitiation Message { get; init; }
/// }
/// </code>
/// </example>
public interface IIso20022Document<out TMessage>
    where TMessage : IIso20022Message
{
    /// <summary>
    /// The XML namespace of the document.
    /// Example: "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"
    /// </summary>
    /// <remarks>
    /// The namespace uniquely identifies the message type and version.
    /// It follows the pattern: urn:iso:std:iso:20022:tech:xsd:[message identifier]
    /// </remarks>
    string Namespace { get; }

    /// <summary>
    /// Optional Business Application Header (head.001).
    /// Null if no BAH is present.
    /// </summary>
    /// <remarks>
    /// The Business Application Header is optional but recommended for:
    /// - End-to-end message tracking
    /// - Sender and receiver identification
    /// - Message routing information
    /// - Duplicate detection
    /// </remarks>
    BusinessApplicationHeader? Header { get; }

    /// <summary>
    /// The actual ISO 20022 message.
    /// </summary>
    /// <remarks>
    /// This is the core business content of the document, such as:
    /// - CustomerCreditTransferInitiation (pain.001)
    /// - BankToCustomerStatement (camt.053)
    /// - FIToFICustomerCreditTransfer (pacs.008)
    /// </remarks>
    TMessage Message { get; }
}
