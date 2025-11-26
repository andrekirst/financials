namespace Camtify.Domain.Common;

/// <summary>
/// Tax remittance information for structured remittance.
/// </summary>
/// <remarks>
/// XML Element: TaxRmt within Strd.
/// Contains tax-related details for the payment.
/// </remarks>
public readonly record struct TaxRemittance
{
    /// <summary>
    /// Gets the creditor tax identification.
    /// </summary>
    public string? CreditorTaxIdentification { get; }

    /// <summary>
    /// Gets the creditor tax type.
    /// </summary>
    public string? CreditorTaxType { get; }

    /// <summary>
    /// Gets the debtor tax identification.
    /// </summary>
    public string? DebtorTaxIdentification { get; }

    /// <summary>
    /// Gets the ultimate debtor tax identification.
    /// </summary>
    public string? UltimateDebtorTaxIdentification { get; }

    /// <summary>
    /// Gets the administration zone.
    /// </summary>
    public string? AdministrationZone { get; }

    /// <summary>
    /// Gets the reference number.
    /// </summary>
    public string? ReferenceNumber { get; }

    /// <summary>
    /// Gets the method used to indicate the underlying business.
    /// </summary>
    public string? Method { get; }

    /// <summary>
    /// Gets the total taxable base amount.
    /// </summary>
    public Money? TotalTaxableBaseAmount { get; }

    /// <summary>
    /// Gets the total tax amount.
    /// </summary>
    public Money? TotalTaxAmount { get; }

    /// <summary>
    /// Gets the date of the tax record.
    /// </summary>
    public DateOnly? Date { get; }

    /// <summary>
    /// Gets the sequence number.
    /// </summary>
    public decimal? SequenceNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaxRemittance"/> struct.
    /// </summary>
    public TaxRemittance(
        string? creditorTaxIdentification = null,
        string? creditorTaxType = null,
        string? debtorTaxIdentification = null,
        string? ultimateDebtorTaxIdentification = null,
        string? administrationZone = null,
        string? referenceNumber = null,
        string? method = null,
        Money? totalTaxableBaseAmount = null,
        Money? totalTaxAmount = null,
        DateOnly? date = null,
        decimal? sequenceNumber = null)
    {
        CreditorTaxIdentification = creditorTaxIdentification;
        CreditorTaxType = creditorTaxType;
        DebtorTaxIdentification = debtorTaxIdentification;
        UltimateDebtorTaxIdentification = ultimateDebtorTaxIdentification;
        AdministrationZone = administrationZone;
        ReferenceNumber = referenceNumber;
        Method = method;
        TotalTaxableBaseAmount = totalTaxableBaseAmount;
        TotalTaxAmount = totalTaxAmount;
        Date = date;
        SequenceNumber = sequenceNumber;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(ReferenceNumber))
        {
            parts.Add($"Ref: {ReferenceNumber}");
        }

        if (TotalTaxAmount.HasValue)
        {
            parts.Add($"Tax: {TotalTaxAmount.Value}");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "Empty";
    }
}
