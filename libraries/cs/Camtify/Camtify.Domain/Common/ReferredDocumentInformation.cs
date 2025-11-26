namespace Camtify.Domain.Common;

/// <summary>
/// Information about a document referenced in remittance information.
/// </summary>
/// <remarks>
/// XML Element: RfrdDocInf within Strd (Structured Remittance).
/// Used to reference invoices, credit notes, or other documents.
/// </remarks>
public readonly record struct ReferredDocumentInformation
{
    /// <summary>
    /// Gets the document type.
    /// </summary>
    public ReferredDocumentType? Type { get; }

    /// <summary>
    /// Gets the document number.
    /// </summary>
    /// <remarks>Max 35 characters.</remarks>
    public string? Number { get; }

    /// <summary>
    /// Gets the related date (e.g., invoice date).
    /// </summary>
    public DateOnly? RelatedDate { get; }

    /// <summary>
    /// Gets the line details for partial payments.
    /// </summary>
    public IReadOnlyList<DocumentLineInformation>? LineDetails { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferredDocumentInformation"/> struct.
    /// </summary>
    /// <param name="type">The document type.</param>
    /// <param name="number">The document number.</param>
    /// <param name="relatedDate">The related date.</param>
    /// <param name="lineDetails">The line details.</param>
    public ReferredDocumentInformation(
        ReferredDocumentType? type = null,
        string? number = null,
        DateOnly? relatedDate = null,
        IReadOnlyList<DocumentLineInformation>? lineDetails = null)
    {
        Type = type;
        Number = number;
        RelatedDate = relatedDate;
        LineDetails = lineDetails;
    }

    /// <summary>
    /// Creates a ReferredDocumentInformation for a commercial invoice.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="invoiceDate">The invoice date.</param>
    /// <returns>A new ReferredDocumentInformation.</returns>
    public static ReferredDocumentInformation ForInvoice(string invoiceNumber, DateOnly? invoiceDate = null)
    {
        return new ReferredDocumentInformation(
            type: ReferredDocumentType.FromCode(DocumentTypeCodes.CINV),
            number: invoiceNumber,
            relatedDate: invoiceDate);
    }

    /// <summary>
    /// Creates a ReferredDocumentInformation for a credit note.
    /// </summary>
    /// <param name="creditNoteNumber">The credit note number.</param>
    /// <param name="creditNoteDate">The credit note date.</param>
    /// <returns>A new ReferredDocumentInformation.</returns>
    public static ReferredDocumentInformation ForCreditNote(string creditNoteNumber, DateOnly? creditNoteDate = null)
    {
        return new ReferredDocumentInformation(
            type: ReferredDocumentType.FromCode(DocumentTypeCodes.CREN),
            number: creditNoteNumber,
            relatedDate: creditNoteDate);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string>();

        if (Type.HasValue)
        {
            parts.Add(Type.Value.GetDescription());
        }

        if (!string.IsNullOrWhiteSpace(Number))
        {
            parts.Add(Number);
        }

        if (RelatedDate.HasValue)
        {
            parts.Add(RelatedDate.Value.ToString("yyyy-MM-dd"));
        }

        return parts.Count > 0 ? string.Join(" ", parts) : "Empty";
    }
}
