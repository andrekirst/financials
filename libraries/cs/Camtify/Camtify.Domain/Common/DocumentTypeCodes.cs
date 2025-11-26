namespace Camtify.Domain.Common;

/// <summary>
/// ISO 20022 document type codes for referred documents in remittance information.
/// </summary>
/// <remarks>
/// These codes identify the type of document being referenced in payment remittance.
/// XML Path: RfrdDocInf/Tp/CdOrPrtry/Cd
/// </remarks>
public static class DocumentTypeCodes
{
    /// <summary>
    /// Commercial Invoice - Standard commercial invoice.
    /// </summary>
    public const string CINV = "CINV";

    /// <summary>
    /// Credit Note - Credit note issued to correct an invoice.
    /// </summary>
    public const string CREN = "CREN";

    /// <summary>
    /// Debit Note - Debit note for additional charges.
    /// </summary>
    public const string DEBN = "DEBN";

    /// <summary>
    /// Dispatch Advice - Notification of goods dispatch.
    /// </summary>
    public const string DISP = "DISP";

    /// <summary>
    /// Debit Note Financial Adjustment - Financial adjustment debit note.
    /// </summary>
    public const string DNFA = "DNFA";

    /// <summary>
    /// Hire Invoice - Invoice for rental or hire services.
    /// </summary>
    public const string HIRI = "HIRI";

    /// <summary>
    /// Metered Service Invoice - Invoice for metered services (utilities).
    /// </summary>
    public const string MSIN = "MSIN";

    /// <summary>
    /// Remittance Advice - Payment advice document.
    /// </summary>
    public const string RADM = "RADM";

    /// <summary>
    /// Statement of Account - Account statement document.
    /// </summary>
    public const string SOAC = "SOAC";

    /// <summary>
    /// Self-Billed Invoice - Invoice created by the buyer.
    /// </summary>
    public const string SBIN = "SBIN";

    /// <summary>
    /// Commercial Contract - Reference to a commercial contract.
    /// </summary>
    public const string CMCN = "CMCN";

    /// <summary>
    /// Credit Note Related To Financial Adjustment.
    /// </summary>
    public const string CNFA = "CNFA";

    /// <summary>
    /// Purchase Order - Reference to a purchase order.
    /// </summary>
    public const string PUOR = "PUOR";

    /// <summary>
    /// Gets all available document type codes.
    /// </summary>
    public static IReadOnlyList<string> All { get; } = new[]
    {
        CINV, CREN, DEBN, DISP, DNFA, HIRI, MSIN, RADM, SOAC, SBIN, CMCN, CNFA, PUOR
    };

    /// <summary>
    /// Gets the description for a document type code.
    /// </summary>
    /// <param name="code">The document type code.</param>
    /// <returns>The description, or the code itself if unknown.</returns>
    public static string GetDescription(string? code) => code switch
    {
        CINV => "Commercial Invoice",
        CREN => "Credit Note",
        DEBN => "Debit Note",
        DISP => "Dispatch Advice",
        DNFA => "Debit Note Financial Adjustment",
        HIRI => "Hire Invoice",
        MSIN => "Metered Service Invoice",
        RADM => "Remittance Advice",
        SOAC => "Statement of Account",
        SBIN => "Self-Billed Invoice",
        CMCN => "Commercial Contract",
        CNFA => "Credit Note Financial Adjustment",
        PUOR => "Purchase Order",
        _ => code ?? string.Empty
    };
}
