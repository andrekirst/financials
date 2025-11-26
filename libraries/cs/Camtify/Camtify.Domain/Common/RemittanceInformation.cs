namespace Camtify.Domain.Common;

/// <summary>
/// Remittance information (payment purpose) which can be unstructured or structured.
/// </summary>
/// <remarks>
/// XML Element: RmtInf.
/// SEPA allows max 140 characters unstructured OR structured information.
/// </remarks>
public readonly record struct RemittanceInformation
{
    /// <summary>
    /// Gets the unstructured remittance information (free text).
    /// </summary>
    /// <remarks>
    /// Max 140 characters per line, multiple lines possible.
    /// XML Element: Ustrd
    /// </remarks>
    public IReadOnlyList<string>? Unstructured { get; }

    /// <summary>
    /// Gets the structured remittance information.
    /// </summary>
    /// <remarks>
    /// XML Element: Strd
    /// </remarks>
    public IReadOnlyList<StructuredRemittanceInformation>? Structured { get; }

    /// <summary>
    /// Gets a value indicating whether this contains unstructured information.
    /// </summary>
    public bool HasUnstructured => Unstructured?.Count > 0;

    /// <summary>
    /// Gets a value indicating whether this contains structured information.
    /// </summary>
    public bool HasStructured => Structured?.Count > 0;

    /// <summary>
    /// Gets the combined text representation of all remittance information.
    /// </summary>
    /// <remarks>
    /// Useful for display purposes, combining unstructured text with
    /// document references and creditor references from structured data.
    /// </remarks>
    public string CombinedText
    {
        get
        {
            var parts = new List<string>();

            if (Unstructured?.Count > 0)
            {
                parts.AddRange(Unstructured);
            }

            if (Structured?.Count > 0)
            {
                foreach (var s in Structured)
                {
                    if (s.CreditorReferenceInformation?.Reference is not null)
                    {
                        parts.Add($"Ref: {s.CreditorReferenceInformation.Value.Reference}");
                    }

                    if (s.ReferredDocumentInformation?.Count > 0)
                    {
                        foreach (var doc in s.ReferredDocumentInformation)
                        {
                            if (doc.Number is not null)
                            {
                                parts.Add($"Doc: {doc.Number}");
                            }
                        }
                    }

                    if (s.AdditionalRemittanceInformation?.Count > 0)
                    {
                        parts.AddRange(s.AdditionalRemittanceInformation);
                    }
                }
            }

            return string.Join(" / ", parts);
        }
    }

    /// <summary>
    /// Gets the first creditor reference found in structured information.
    /// </summary>
    public string? FirstCreditorReference =>
        Structured?.FirstOrDefault(s => s.CreditorReferenceInformation?.Reference is not null)
            .CreditorReferenceInformation?.Reference;

    /// <summary>
    /// Gets the first document number found in structured information.
    /// </summary>
    public string? FirstDocumentNumber =>
        Structured?.SelectMany(s => s.ReferredDocumentInformation ?? Array.Empty<ReferredDocumentInformation>())
            .FirstOrDefault(d => d.Number is not null)
            .Number;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemittanceInformation"/> struct.
    /// </summary>
    public RemittanceInformation(
        IReadOnlyList<string>? unstructured = null,
        IReadOnlyList<StructuredRemittanceInformation>? structured = null)
    {
        Unstructured = unstructured;
        Structured = structured;
    }

    /// <summary>
    /// Creates remittance information from unstructured text.
    /// </summary>
    /// <param name="text">The unstructured text (will be split at 140 chars if needed).</param>
    /// <returns>A new RemittanceInformation.</returns>
    public static RemittanceInformation FromUnstructured(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return default;
        }

        // Split into 140-character chunks if necessary
        var lines = new List<string>();
        var remaining = text;

        while (remaining.Length > 0)
        {
            var chunk = remaining.Length > 140 ? remaining[..140] : remaining;
            lines.Add(chunk);
            remaining = remaining.Length > 140 ? remaining[140..] : string.Empty;
        }

        return new RemittanceInformation(unstructured: lines);
    }

    /// <summary>
    /// Creates remittance information from multiple unstructured lines.
    /// </summary>
    /// <param name="lines">The unstructured text lines.</param>
    /// <returns>A new RemittanceInformation.</returns>
    public static RemittanceInformation FromUnstructured(params string[] lines)
    {
        return new RemittanceInformation(unstructured: lines);
    }

    /// <summary>
    /// Creates remittance information from structured data.
    /// </summary>
    /// <param name="structured">The structured remittance information.</param>
    /// <returns>A new RemittanceInformation.</returns>
    public static RemittanceInformation FromStructured(params StructuredRemittanceInformation[] structured)
    {
        return new RemittanceInformation(structured: structured);
    }

    /// <summary>
    /// Creates remittance information with just an RF reference.
    /// </summary>
    /// <param name="rfReference">The RF reference.</param>
    /// <returns>A new RemittanceInformation.</returns>
    public static RemittanceInformation FromRfReference(string rfReference)
    {
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);
        var structured = new StructuredRemittanceInformation(creditorReferenceInformation: creditorRef);
        return new RemittanceInformation(structured: new[] { structured });
    }

    /// <summary>
    /// Creates remittance information for an invoice payment.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="invoiceDate">Optional invoice date.</param>
    /// <param name="rfReference">Optional RF reference.</param>
    /// <returns>A new RemittanceInformation.</returns>
    public static RemittanceInformation ForInvoice(
        string invoiceNumber,
        DateOnly? invoiceDate = null,
        string? rfReference = null)
    {
        var structured = rfReference is not null
            ? StructuredRemittanceInformation.ForInvoiceWithRfReference(invoiceNumber, rfReference, invoiceDate)
            : StructuredRemittanceInformation.ForInvoice(invoiceNumber, invoiceDate);

        return new RemittanceInformation(structured: new[] { structured });
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (HasUnstructured && HasStructured)
        {
            return $"Mixed: {CombinedText}";
        }

        if (HasUnstructured)
        {
            return $"Unstructured: {string.Join(" ", Unstructured!)}";
        }

        if (HasStructured)
        {
            return $"Structured: {CombinedText}";
        }

        return "Empty";
    }
}
