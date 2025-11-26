namespace Camtify.Domain.Common;

/// <summary>
/// Structured remittance information with document references and creditor reference.
/// </summary>
/// <remarks>
/// XML Element: Strd within RmtInf.
/// Contains detailed structured payment purpose information.
/// </remarks>
public readonly record struct StructuredRemittanceInformation
{
    /// <summary>
    /// Gets the referenced documents (e.g., invoices, credit notes).
    /// </summary>
    public IReadOnlyList<ReferredDocumentInformation>? ReferredDocumentInformation { get; }

    /// <summary>
    /// Gets the amounts of the referenced documents.
    /// </summary>
    public RemittanceAmount? ReferredDocumentAmount { get; }

    /// <summary>
    /// Gets the creditor reference information (e.g., RF reference).
    /// </summary>
    public CreditorReferenceInformation? CreditorReferenceInformation { get; }

    /// <summary>
    /// Gets the invoicer (party that issued the invoice).
    /// </summary>
    public PartyIdentification? Invoicer { get; }

    /// <summary>
    /// Gets the invoicee (party that received the invoice).
    /// </summary>
    public PartyIdentification? Invoicee { get; }

    /// <summary>
    /// Gets the tax remittance information.
    /// </summary>
    public TaxRemittance? TaxRemittance { get; }

    /// <summary>
    /// Gets additional remittance information (max 3 lines of 140 chars).
    /// </summary>
    public IReadOnlyList<string>? AdditionalRemittanceInformation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredRemittanceInformation"/> struct.
    /// </summary>
    public StructuredRemittanceInformation(
        IReadOnlyList<ReferredDocumentInformation>? referredDocumentInformation = null,
        RemittanceAmount? referredDocumentAmount = null,
        CreditorReferenceInformation? creditorReferenceInformation = null,
        PartyIdentification? invoicer = null,
        PartyIdentification? invoicee = null,
        TaxRemittance? taxRemittance = null,
        IReadOnlyList<string>? additionalRemittanceInformation = null)
    {
        ReferredDocumentInformation = referredDocumentInformation;
        ReferredDocumentAmount = referredDocumentAmount;
        CreditorReferenceInformation = creditorReferenceInformation;
        Invoicer = invoicer;
        Invoicee = invoicee;
        TaxRemittance = taxRemittance;
        AdditionalRemittanceInformation = additionalRemittanceInformation;
    }

    /// <summary>
    /// Creates structured remittance for an invoice with RF reference.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="rfReference">The RF reference.</param>
    /// <param name="invoiceDate">Optional invoice date.</param>
    /// <param name="amount">Optional amount.</param>
    /// <returns>A new StructuredRemittanceInformation.</returns>
    public static StructuredRemittanceInformation ForInvoiceWithRfReference(
        string invoiceNumber,
        string rfReference,
        DateOnly? invoiceDate = null,
        Money? amount = null)
    {
        var docInfo = Common.ReferredDocumentInformation.ForInvoice(invoiceNumber, invoiceDate);
        var creditorRef = Common.CreditorReferenceInformation.ForRfReference(rfReference);
        var docAmount = amount.HasValue ? RemittanceAmount.ForDueAmount(amount.Value) : (RemittanceAmount?)null;

        return new StructuredRemittanceInformation(
            referredDocumentInformation: new[] { docInfo },
            referredDocumentAmount: docAmount,
            creditorReferenceInformation: creditorRef);
    }

    /// <summary>
    /// Creates structured remittance for an invoice without RF reference.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="invoiceDate">Optional invoice date.</param>
    /// <param name="amount">Optional amount.</param>
    /// <returns>A new StructuredRemittanceInformation.</returns>
    public static StructuredRemittanceInformation ForInvoice(
        string invoiceNumber,
        DateOnly? invoiceDate = null,
        Money? amount = null)
    {
        var docInfo = Common.ReferredDocumentInformation.ForInvoice(invoiceNumber, invoiceDate);
        var docAmount = amount.HasValue ? RemittanceAmount.ForDueAmount(amount.Value) : (RemittanceAmount?)null;

        return new StructuredRemittanceInformation(
            referredDocumentInformation: new[] { docInfo },
            referredDocumentAmount: docAmount);
    }

    /// <summary>
    /// Gets the primary document number if available.
    /// </summary>
    public string? PrimaryDocumentNumber =>
        ReferredDocumentInformation?.FirstOrDefault().Number;

    /// <summary>
    /// Gets the creditor reference if available.
    /// </summary>
    public string? CreditorReference => CreditorReferenceInformation?.Reference;

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string>();

        if (CreditorReferenceInformation?.Reference is not null)
        {
            parts.Add($"Ref: {CreditorReferenceInformation.Value.Reference}");
        }

        if (ReferredDocumentInformation?.Count > 0)
        {
            foreach (var doc in ReferredDocumentInformation)
            {
                if (doc.Number is not null)
                {
                    parts.Add($"Doc: {doc.Number}");
                }
            }
        }

        if (ReferredDocumentAmount?.DuePayableAmount is not null)
        {
            parts.Add($"Amount: {ReferredDocumentAmount.Value.DuePayableAmount}");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "Empty";
    }
}
