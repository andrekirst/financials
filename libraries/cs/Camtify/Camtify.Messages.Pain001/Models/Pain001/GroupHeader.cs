using Camtify.Domain.Common;

namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents the Group Header (GrpHdr) of a pain.001 message.
/// </summary>
/// <remarks>
/// Contains message-level information that applies to all payment information blocks.
/// </remarks>
public sealed record GroupHeader
{
    /// <summary>
    /// Gets the message identification assigned by the instructing party.
    /// </summary>
    /// <remarks>
    /// XML Element: MsgId
    /// Must be unique per instructing party.
    /// </remarks>
    public string MessageIdentification { get; init; } = null!;

    /// <summary>
    /// Gets the date and time when the message was created.
    /// </summary>
    /// <remarks>
    /// XML Element: CreDtTm
    /// </remarks>
    public DateTimeOffset CreationDateTime { get; init; }

    /// <summary>
    /// Gets the total number of transactions contained in the message.
    /// </summary>
    /// <remarks>
    /// XML Element: NbOfTxs
    /// Sum of all individual transactions across all payment information blocks.
    /// </remarks>
    public int NumberOfTransactions { get; init; }

    /// <summary>
    /// Gets the total control sum of all amounts in the message.
    /// </summary>
    /// <remarks>
    /// XML Element: CtrlSum
    /// Optional but recommended for validation.
    /// Sum of all instructed amounts across all payment information blocks.
    /// </remarks>
    public decimal? ControlSum { get; init; }

    /// <summary>
    /// Gets the party that initiates the payment.
    /// </summary>
    /// <remarks>
    /// XML Element: InitgPty
    /// </remarks>
    public PartyIdentification InitiatingParty { get; init; } = null!;

    /// <summary>
    /// Gets the forwarding agent (optional).
    /// </summary>
    /// <remarks>
    /// XML Element: FwdgAgt
    /// Financial institution that forwards the message.
    /// </remarks>
    public BranchAndFinancialInstitutionIdentification? ForwardingAgent { get; init; }
}
