namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents Payment Identification (PmtId).
/// </summary>
/// <remarks>
/// Contains identification information for the payment instruction.
/// </remarks>
public sealed record PaymentIdentification
{
    /// <summary>
    /// Gets the instruction identification.
    /// </summary>
    /// <remarks>
    /// XML Element: InstrId
    /// Unique identification assigned by the instructing party.
    /// </remarks>
    public string? InstructionIdentification { get; init; }

    /// <summary>
    /// Gets the end-to-end identification.
    /// </summary>
    /// <remarks>
    /// XML Element: EndToEndId
    /// Unique identification assigned by the initiating party that is passed on throughout the transaction chain.
    /// Mandatory in SEPA. If not provided, use "NOTPROVIDED".
    /// </remarks>
    public string EndToEndIdentification { get; init; } = null!;
}
