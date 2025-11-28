using Camtify.Core;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents the Customer Credit Transfer Initiation (CstmrCdtTrfInitn) message.
/// </summary>
/// <remarks>
/// This is the main container for credit transfer instructions in pain.001 messages.
/// Contains group header and one or more payment information blocks.
/// </remarks>
public sealed record CustomerCreditTransferInitiation : IIso20022Message
{
    /// <summary>
    /// Gets the message type identifier.
    /// </summary>
    /// <remarks>
    /// The MessageType is determined from the document's namespace during parsing.
    /// This property is typically set by the parser based on the version being parsed.
    /// </remarks>
    public MessageIdentifier MessageType { get; init; }

    /// <summary>
    /// Gets the creation date and time from the GroupHeader.
    /// </summary>
    public DateTime CreationDateTime => GroupHeader.CreationDateTime.DateTime;

    /// <summary>
    /// Gets the message identification from the GroupHeader.
    /// </summary>
    public string MessageIdentification => GroupHeader.MessageIdentification;

    /// <summary>
    /// Gets the group header containing message-level information.
    /// </summary>
    /// <remarks>
    /// XML Element: GrpHdr
    /// </remarks>
    public GroupHeader GroupHeader { get; init; } = null!;

    /// <summary>
    /// Gets the list of payment information blocks.
    /// </summary>
    /// <remarks>
    /// XML Element: PmtInf
    /// At least one payment information block is required.
    /// </remarks>
    public IReadOnlyList<PaymentInformation> PaymentInformation { get; init; } = null!;
}
