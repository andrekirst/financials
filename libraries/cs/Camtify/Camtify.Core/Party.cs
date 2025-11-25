namespace Camtify.Core;

/// <summary>
/// Represents a party (sender or receiver) in the Business Application Header.
/// </summary>
public sealed record Party
{
    /// <summary>
    /// Gets the financial institution identification (BIC code).
    /// </summary>
    /// <remarks>
    /// Bank Identifier Code (BIC) as defined by ISO 9362.
    /// Example: "DEUTDEFF" for Deutsche Bank Frankfurt.
    /// </remarks>
    public string? FinancialInstitutionIdentification { get; init; }

    /// <summary>
    /// Gets the organization identification.
    /// </summary>
    public string? OrganizationIdentification { get; init; }
}
