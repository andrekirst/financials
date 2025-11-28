using Camtify.Core;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents a pain.001.001.10 Customer Credit Transfer Initiation document.
/// </summary>
/// <remarks>
/// Version 010 provides incremental updates from version 009 with enhanced validation rules
/// and additional regulatory reporting fields. Backward-compatible with v09.
/// </remarks>
public sealed record Pain001v10Document : IIso20022Document<CustomerCreditTransferInitiation>
{
    /// <summary>
    /// Gets the customer credit transfer initiation message.
    /// </summary>
    public CustomerCreditTransferInitiation Message { get; init; } = null!;

    /// <summary>
    /// Gets the XML namespace URI for pain.001.001.10.
    /// </summary>
    public string Namespace => "urn:iso:std:iso:20022:tech:xsd:pain.001.001.10";

    /// <summary>
    /// Gets the optional Business Application Header.
    /// </summary>
    /// <remarks>
    /// Currently not supported by this parser. Always returns null.
    /// </remarks>
    public BusinessApplicationHeader? Header => null;

    /// <summary>
    /// Gets the ISO 20022 version identifier (always "010" for this document type).
    /// </summary>
    public string Version => "010";
}
