namespace Camtify.Domain.Common;

/// <summary>
/// Contact details of a party (XML: CtctDtls).
/// </summary>
/// <remarks>
/// Contains various methods to contact a person or organisation,
/// including phone, email, fax, and other communication channels.
/// </remarks>
public sealed record ContactDetails
{
    /// <summary>
    /// Gets the name prefix/salutation (XML: NmPrfx).
    /// </summary>
    public NamePrefix? NamePrefix { get; init; }

    /// <summary>
    /// Gets the contact person name (XML: Nm).
    /// </summary>
    /// <remarks>
    /// Maximum 140 characters.
    /// </remarks>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the phone number (XML: PhneNb).
    /// </summary>
    /// <remarks>
    /// E.164 format recommended (e.g., "+49 30 12345678").
    /// Maximum 35 characters.
    /// </remarks>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Gets the mobile phone number (XML: MobNb).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? MobileNumber { get; init; }

    /// <summary>
    /// Gets the fax number (XML: FaxNb).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? FaxNumber { get; init; }

    /// <summary>
    /// Gets the email address (XML: EmailAdr).
    /// </summary>
    /// <remarks>
    /// Maximum 2048 characters.
    /// </remarks>
    public string? EmailAddress { get; init; }

    /// <summary>
    /// Gets the purpose of the email address (XML: EmailPurp).
    /// </summary>
    /// <remarks>
    /// Describes the purpose of the email address.
    /// Maximum 35 characters.
    /// </remarks>
    public string? EmailPurpose { get; init; }

    /// <summary>
    /// Gets the job title or function (XML: JobTitl).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? JobTitle { get; init; }

    /// <summary>
    /// Gets the area of responsibility (XML: Rspnsblty).
    /// </summary>
    /// <remarks>
    /// Maximum 35 characters.
    /// </remarks>
    public string? Responsibility { get; init; }

    /// <summary>
    /// Gets the department name (XML: Dept).
    /// </summary>
    /// <remarks>
    /// Maximum 70 characters.
    /// </remarks>
    public string? Department { get; init; }

    /// <summary>
    /// Gets other contact methods (XML: Othr).
    /// </summary>
    /// <remarks>
    /// Additional contact channels like social media, messaging apps, etc.
    /// </remarks>
    public IReadOnlyList<OtherContact>? Other { get; init; }

    /// <summary>
    /// Gets the preferred contact method (XML: PrefrdMtd).
    /// </summary>
    public PreferredContactMethod? PreferredMethod { get; init; }
}
