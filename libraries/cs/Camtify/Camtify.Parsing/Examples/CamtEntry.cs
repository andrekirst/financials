namespace Camtify.Parsing.Examples;

/// <summary>
/// Example entry type for CAMT messages.
/// </summary>
/// <remarks>
/// This is a simplified example to demonstrate the streaming parser pattern.
/// </remarks>
public sealed class CamtEntry
{
    /// <summary>
    /// Gets or sets the entry reference.
    /// </summary>
    public string? EntryReference { get; set; }

    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the credit/debit indicator.
    /// </summary>
    public string? CreditDebitIndicator { get; set; }

    /// <summary>
    /// Gets or sets the booking date.
    /// </summary>
    public DateTime? BookingDate { get; set; }

    /// <summary>
    /// Gets or sets the value date.
    /// </summary>
    public DateTime? ValueDate { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string? Status { get; set; }
}
