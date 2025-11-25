namespace Camtify.Domain.Common;

/// <summary>
/// Branch data (XML: BrnchId).
/// </summary>
/// <remarks>
/// Information about a specific branch of a financial institution.
/// </remarks>
public sealed record BranchData
{
    /// <summary>
    /// Gets the branch identification (XML: Id).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? Identification { get; init; }

    /// <summary>
    /// Gets the Legal Entity Identifier (XML: LEI).
    /// </summary>
    /// <remarks>
    /// ISO 17442 LEI, 20 alphanumeric characters.
    /// </remarks>
    public string? Lei { get; init; }

    /// <summary>
    /// Gets the branch name (XML: Nm).
    /// </summary>
    /// <remarks>
    /// Maximum 140 characters.
    /// </remarks>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the branch postal address (XML: PstlAdr).
    /// </summary>
    public PostalAddress? PostalAddress { get; init; }
}
