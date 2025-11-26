namespace Camtify.Domain.Common;

/// <summary>
/// Information about a specific line within a referred document.
/// </summary>
/// <remarks>
/// XML Element: LineDtls within RfrdDocInf.
/// Used for partial payments referencing specific invoice lines.
/// </remarks>
public readonly record struct DocumentLineInformation
{
    /// <summary>
    /// Gets the line identification.
    /// </summary>
    public IReadOnlyList<string>? Identification { get; }

    /// <summary>
    /// Gets the line description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the line amount.
    /// </summary>
    public Money? Amount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentLineInformation"/> struct.
    /// </summary>
    /// <param name="identification">The line identification.</param>
    /// <param name="description">The line description.</param>
    /// <param name="amount">The line amount.</param>
    public DocumentLineInformation(
        IReadOnlyList<string>? identification = null,
        string? description = null,
        Money? amount = null)
    {
        Identification = identification;
        Description = description;
        Amount = amount;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string>();

        if (Identification?.Count > 0)
        {
            parts.Add($"Id: {string.Join(", ", Identification)}");
        }

        if (!string.IsNullOrWhiteSpace(Description))
        {
            parts.Add(Description);
        }

        if (Amount.HasValue)
        {
            parts.Add(Amount.Value.ToString());
        }

        return parts.Count > 0 ? string.Join(" - ", parts) : "Empty";
    }
}
