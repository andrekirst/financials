namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents a pain.001 Customer Credit Transfer Initiation document.
/// </summary>
/// <remarks>
/// This is the root element for pain.001 messages used to initiate credit transfers.
/// Supports versions: 003, 008, 009, 010, 011.
/// </remarks>
public sealed record Pain001Document
{
    /// <summary>
    /// Gets the Customer Credit Transfer Initiation message.
    /// </summary>
    public CustomerCreditTransferInitiation CreditTransferInitiation { get; init; } = null!;

    /// <summary>
    /// Gets the version of the pain.001 message.
    /// </summary>
    public string Version { get; init; } = null!;

    /// <summary>
    /// Gets the namespace used for this version.
    /// </summary>
    public string Namespace { get; init; } = null!;
}
