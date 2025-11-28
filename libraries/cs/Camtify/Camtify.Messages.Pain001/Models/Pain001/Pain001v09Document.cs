using Camtify.Core;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents a pain.001.001.09 Customer Credit Transfer Initiation document.
/// </summary>
/// <remarks>
/// Version 009 is the most commonly used version and introduced Legal Entity Identifier (LEI) support,
/// enhanced regulatory reporting, and structured cross-border data.
/// SEPA migration deadline to v09 or higher is November 2026.
/// </remarks>
public sealed record Pain001v09Document : IIso20022Document<CustomerCreditTransferInitiation>
{
    /// <summary>
    /// Gets the customer credit transfer initiation message.
    /// </summary>
    public CustomerCreditTransferInitiation Message { get; init; } = null!;

    /// <summary>
    /// Gets the XML namespace URI for pain.001.001.09.
    /// </summary>
    public string Namespace => "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09";

    /// <summary>
    /// Gets the optional Business Application Header.
    /// </summary>
    /// <remarks>
    /// Currently not supported by this parser. Always returns null.
    /// </remarks>
    public BusinessApplicationHeader? Header => null;

    /// <summary>
    /// Gets the ISO 20022 version identifier (always "009" for this document type).
    /// </summary>
    public string Version => "009";
}
