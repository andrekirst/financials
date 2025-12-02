namespace Camtify.Parsing.Examples;

/// <summary>
/// Example document type for CAMT.053 messages.
/// </summary>
/// <remarks>
/// This is a simplified example to demonstrate the streaming parser pattern.
/// </remarks>
public sealed class Camt053Document
{
    /// <summary>
    /// Gets or sets the statement identifier.
    /// </summary>
    public string? StatementId { get; set; }

    /// <summary>
    /// Gets or sets the account IBAN.
    /// </summary>
    public string? AccountIban { get; set; }

    /// <summary>
    /// Gets or sets the creation date time.
    /// </summary>
    public DateTime? CreationDateTime { get; set; }

    /// <summary>
    /// Gets or sets the number of entries.
    /// </summary>
    public int? NumberOfEntries { get; set; }
}
