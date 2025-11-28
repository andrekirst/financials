using Camtify.Core;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents a pain.001.001.03 Customer Credit Transfer Initiation document.
/// </summary>
/// <remarks>
/// Version 003 is the SEPA baseline version for customer credit transfers.
/// This version introduced the foundational structure for SEPA payments.
/// </remarks>
public sealed record Pain001v03Document : IIso20022Document<CustomerCreditTransferInitiation>
{
    /// <summary>
    /// Gets the customer credit transfer initiation message.
    /// </summary>
    public CustomerCreditTransferInitiation Message { get; init; } = null!;

    /// <summary>
    /// Gets the XML namespace URI for pain.001.001.03.
    /// </summary>
    public string Namespace => "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03";

    /// <summary>
    /// Gets the optional Business Application Header.
    /// </summary>
    /// <remarks>
    /// Currently not supported by this parser. Always returns null.
    /// </remarks>
    public BusinessApplicationHeader? Header => null;

    /// <summary>
    /// Gets the ISO 20022 version identifier (always "003" for this document type).
    /// </summary>
    public string Version => "003";
}
