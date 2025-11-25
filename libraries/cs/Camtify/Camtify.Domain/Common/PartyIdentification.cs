namespace Camtify.Domain.Common;

/// <summary>
/// Identification of a party (person or organisation) (XML: various).
/// </summary>
/// <remarks>
/// Used for identifying parties in ISO 20022 messages including:
/// - Debtor (XML: Dbtr)
/// - Creditor (XML: Cdtr)
/// - Initiating Party (XML: InitgPty)
/// - Ultimate Debtor (XML: UltmtDbtr)
/// - Ultimate Creditor (XML: UltmtCdtr)
/// </remarks>
/// <example>
/// XML structure:
/// <code>
/// &lt;Dbtr&gt;
///   &lt;Nm&gt;Max Mustermann GmbH&lt;/Nm&gt;
///   &lt;PstlAdr&gt;...&lt;/PstlAdr&gt;
///   &lt;Id&gt;
///     &lt;OrgId&gt;
///       &lt;LEI&gt;529900T8BM49AURSDO55&lt;/LEI&gt;
///     &lt;/OrgId&gt;
///   &lt;/Id&gt;
///   &lt;CtctDtls&gt;...&lt;/CtctDtls&gt;
/// &lt;/Dbtr&gt;
/// </code>
/// </example>
public sealed record PartyIdentification
{
    /// <summary>
    /// Gets the name of the party (XML: Nm).
    /// </summary>
    /// <remarks>
    /// Maximum 140 characters.
    /// </remarks>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the postal address (XML: PstlAdr).
    /// </summary>
    public PostalAddress? PostalAddress { get; init; }

    /// <summary>
    /// Gets the identification of the party (XML: Id).
    /// </summary>
    /// <remarks>
    /// Either <see cref="OrganisationPartyId"/> for organisations
    /// or <see cref="PersonPartyId"/> for natural persons.
    /// </remarks>
    public PartyId? Identification { get; init; }

    /// <summary>
    /// Gets the country of residence (XML: CtryOfRes).
    /// </summary>
    /// <remarks>
    /// ISO 3166-1 alpha-2 country code (e.g., "DE", "US", "GB").
    /// </remarks>
    public string? CountryOfResidence { get; init; }

    /// <summary>
    /// Gets the contact details (XML: CtctDtls).
    /// </summary>
    public ContactDetails? ContactDetails { get; init; }
}
