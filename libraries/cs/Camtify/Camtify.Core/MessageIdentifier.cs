using System.Text.RegularExpressions;

namespace Camtify.Core;

/// <summary>
/// Represents an ISO 20022 message identifier (e.g., "pain.001.001.09", "camt.053.001.08").
/// </summary>
/// <remarks>
/// ISO 20022 message identifiers follow the pattern: [business area].[message].[variant].[version]
/// Examples:
/// - pain.001.001.09: Customer Credit Transfer Initiation
/// - camt.053.001.08: Bank to Customer Statement
/// - pacs.008.001.08: FI to FI Customer Credit Transfer
/// </remarks>
public readonly record struct MessageIdentifier : IComparable<MessageIdentifier>
{
    private static readonly Regex ParsePattern = new(
        @"^([a-z]{4})\.(\d{3})\.(\d{3})\.(\d{2})$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex[] NamespacePatterns =
    {
        // Standard: urn:iso:std:iso:20022:tech:xsd:pain.001.001.09
        new(@"urn:iso:std:iso:20022:tech:xsd:([a-z]{4}\.\d{3}\.\d{3}\.\d{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        // Variante: urn:swift:xsd:pain.001.001.09
        new(@"urn:swift:xsd:([a-z]{4}\.\d{3}\.\d{3}\.\d{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        // CBPR+: urn:iso:std:iso:20022:tech:xsd:pain.001.001.09$cbpr_plus
        new(@"urn:iso:std:iso:20022:tech:xsd:([a-z]{4}\.\d{3}\.\d{3}\.\d{2})\$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
    };

    /// <summary>
    /// Gets the full message identifier string (e.g., "pain.001.001.09").
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the business area (e.g., "pain", "camt", "pacs").
    /// </summary>
    public string BusinessArea { get; }

    /// <summary>
    /// Gets the message number (e.g., "001", "053").
    /// </summary>
    public string MessageNumber { get; }

    /// <summary>
    /// Gets the variant number (e.g., "001").
    /// </summary>
    public string Variant { get; }

    /// <summary>
    /// Gets the version string (e.g., "09", "08").
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the version as an integer.
    /// </summary>
    public int VersionNumber => int.Parse(Version);

    /// <summary>
    /// Gets a short name for the message (e.g., "PAIN.001 v9").
    /// </summary>
    public string ShortName => $"{BusinessArea.ToUpperInvariant()}.{MessageNumber} v{VersionNumber}";

    /// <summary>
    /// Gets the full name with description (e.g., "PAIN.001 v9 - Customer Credit Transfer Initiation").
    /// </summary>
    public string FullName => $"{ShortName} - {GetMessageDescription()}";

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageIdentifier"/> struct.
    /// </summary>
    /// <param name="businessArea">The business area.</param>
    /// <param name="messageNumber">The message number.</param>
    /// <param name="variant">The variant.</param>
    /// <param name="version">The version.</param>
    private MessageIdentifier(string businessArea, string messageNumber, string variant, string version)
    {
        BusinessArea = businessArea.ToLowerInvariant();
        MessageNumber = messageNumber;
        Variant = variant;
        Version = version;
        Value = $"{BusinessArea}.{MessageNumber}.{Variant}.{Version}";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageIdentifier"/> struct by parsing a string.
    /// </summary>
    /// <param name="value">The full message identifier (e.g., "pain.001.001.09").</param>
    /// <exception cref="ArgumentException">Thrown when the value format is invalid.</exception>
    public MessageIdentifier(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        if (!TryParse(value, out var result, out var error))
        {
            throw new ArgumentException(error, nameof(value));
        }

        BusinessArea = result!.Value.BusinessArea;
        MessageNumber = result.Value.MessageNumber;
        Variant = result.Value.Variant;
        Version = result.Value.Version;
        Value = result.Value.Value;
    }

    /// <summary>
    /// Parses a message identifier from a string.
    /// </summary>
    /// <param name="value">The message identifier string (e.g., "pain.001.001.09").</param>
    /// <returns>A new MessageIdentifier instance.</returns>
    /// <exception cref="FormatException">Thrown when the format is invalid.</exception>
    public static MessageIdentifier Parse(string value)
    {
        if (!TryParse(value, out var result, out var error))
        {
            throw new FormatException(error);
        }

        return result!.Value;
    }

    /// <summary>
    /// Tries to parse a message identifier from a string.
    /// </summary>
    /// <param name="value">The message identifier string.</param>
    /// <param name="result">The parsed result if successful.</param>
    /// <param name="error">The error message if parsing failed.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string? value, out MessageIdentifier? result, out string? error)
    {
        result = null;
        error = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "Message identifier cannot be null or empty.";
            return false;
        }

        var match = ParsePattern.Match(value.Trim());
        if (!match.Success)
        {
            error = $"Invalid format: '{value}'. Expected format: 'xxxx.nnn.nnn.nn' (e.g., 'pain.001.001.09')";
            return false;
        }

        result = new MessageIdentifier(
            match.Groups[1].Value,
            match.Groups[2].Value,
            match.Groups[3].Value,
            match.Groups[4].Value);

        return true;
    }

    /// <summary>
    /// Extracts a message identifier from an XML namespace.
    /// </summary>
    /// <param name="namespace">The XML namespace (e.g., "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09").</param>
    /// <returns>A new MessageIdentifier instance.</returns>
    /// <exception cref="FormatException">Thrown when no valid identifier is found in the namespace.</exception>
    public static MessageIdentifier FromNamespace(string @namespace)
    {
        if (!TryFromNamespace(@namespace, out var result, out var error))
        {
            throw new FormatException(error);
        }

        return result!.Value;
    }

    /// <summary>
    /// Tries to extract a message identifier from an XML namespace.
    /// </summary>
    /// <param name="namespace">The XML namespace.</param>
    /// <param name="result">The parsed result if successful.</param>
    /// <param name="error">The error message if parsing failed.</param>
    /// <returns>True if extraction was successful, false otherwise.</returns>
    public static bool TryFromNamespace(string? @namespace, out MessageIdentifier? result, out string? error)
    {
        result = null;
        error = null;

        if (string.IsNullOrWhiteSpace(@namespace))
        {
            error = "Namespace cannot be null or empty.";
            return false;
        }

        foreach (var pattern in NamespacePatterns)
        {
            var match = pattern.Match(@namespace);
            if (match.Success)
            {
                return TryParse(match.Groups[1].Value, out result, out error);
            }
        }

        error = $"Namespace '{@namespace}' does not contain a valid ISO 20022 message identifier.";
        return false;
    }

    /// <summary>
    /// Creates a <see cref="MessageIdentifier"/> from its components.
    /// </summary>
    /// <param name="businessArea">The business area (e.g., "pain").</param>
    /// <param name="messageNumber">The message number (e.g., "001").</param>
    /// <param name="variant">The variant number (e.g., "001").</param>
    /// <param name="version">The version number (e.g., "09").</param>
    /// <returns>A new <see cref="MessageIdentifier"/> instance.</returns>
    public static MessageIdentifier Create(string businessArea, string messageNumber, string variant, string version)
    {
        return new MessageIdentifier(businessArea, messageNumber, variant, version);
    }

    /// <summary>
    /// Generates the full XML namespace for this message identifier.
    /// </summary>
    /// <returns>The XML namespace (e.g., "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09").</returns>
    public string ToNamespace() => $"urn:iso:std:iso:20022:tech:xsd:{Value}";

    /// <summary>
    /// Gets the human-readable description for this message type.
    /// </summary>
    /// <returns>The description of the message type.</returns>
    public string GetMessageDescription() => (BusinessArea, MessageNumber) switch
    {
        ("pain", "001") => "Customer Credit Transfer Initiation",
        ("pain", "002") => "Customer Payment Status Report",
        ("pain", "007") => "Customer Payment Reversal",
        ("pain", "008") => "Customer Direct Debit Initiation",
        ("pain", "013") => "Creditor Payment Activation Request",
        ("pain", "014") => "Creditor Payment Activation Request Status Report",

        ("camt", "029") => "Resolution Of Investigation",
        ("camt", "052") => "Bank To Customer Account Report",
        ("camt", "053") => "Bank To Customer Statement",
        ("camt", "054") => "Bank To Customer Debit Credit Notification",
        ("camt", "055") => "Customer Payment Cancellation Request",
        ("camt", "056") => "FI To FI Payment Cancellation Request",
        ("camt", "057") => "Notification To Receive",
        ("camt", "058") => "Notification To Receive Cancellation Advice",
        ("camt", "059") => "Notification To Receive Status Report",
        ("camt", "060") => "Account Reporting Request",

        ("pacs", "002") => "FI To FI Payment Status Report",
        ("pacs", "003") => "FI To FI Customer Direct Debit",
        ("pacs", "004") => "Payment Return",
        ("pacs", "007") => "FI To FI Payment Reversal",
        ("pacs", "008") => "FI To FI Customer Credit Transfer",
        ("pacs", "009") => "Financial Institution Credit Transfer",
        ("pacs", "010") => "Financial Institution Direct Debit",
        ("pacs", "028") => "FI To FI Payment Status Request",

        ("head", "001") => "Business Application Header",
        ("head", "002") => "Business File Header",

        ("reda", "001") => "Price Report",
        ("reda", "002") => "Price Report Cancellation",
        ("reda", "004") => "Fund Processing Passport Report",

        ("sese", "023") => "Securities Settlement Transaction Instruction",
        ("sese", "024") => "Securities Settlement Transaction Status Advice",
        ("sese", "025") => "Securities Settlement Transaction Confirmation",

        ("semt", "013") => "Intra-Position Movement Instruction",
        ("semt", "014") => "Intra-Position Movement Status Advice",
        ("semt", "017") => "Securities Transaction Posting Report",

        ("acmt", "001") => "Account Opening Instruction",
        ("acmt", "002") => "Account Details Confirmation",
        ("acmt", "003") => "Account Modification Instruction",

        _ => "Unknown Message Type"
    };

    /// <summary>
    /// Checks if this identifier belongs to the specified business area.
    /// </summary>
    /// <param name="area">The business area to check (e.g., "pain").</param>
    /// <returns>True if this identifier belongs to the area.</returns>
    public bool IsBusinessArea(string area) =>
        string.Equals(BusinessArea, area, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if this identifier represents a newer version than another identifier.
    /// </summary>
    /// <param name="other">The other identifier to compare with.</param>
    /// <returns>True if this identifier has a higher version number.</returns>
    /// <exception cref="InvalidOperationException">Thrown when comparing identifiers of different message types.</exception>
    public bool IsNewerThan(MessageIdentifier other)
    {
        if (!string.Equals(BusinessArea, other.BusinessArea, StringComparison.OrdinalIgnoreCase) ||
            MessageNumber != other.MessageNumber)
        {
            throw new InvalidOperationException(
                $"Cannot compare versions of different message types: {Value} vs {other.Value}");
        }

        return VersionNumber > other.VersionNumber;
    }

    /// <summary>
    /// Creates a new identifier with a different version.
    /// </summary>
    /// <param name="version">The new version number.</param>
    /// <returns>A new MessageIdentifier with the specified version.</returns>
    public MessageIdentifier WithVersion(int version) =>
        new(BusinessArea, MessageNumber, Variant, version.ToString("D2"));

    /// <inheritdoc/>
    public int CompareTo(MessageIdentifier other)
    {
        var areaCompare = string.Compare(BusinessArea, other.BusinessArea, StringComparison.OrdinalIgnoreCase);
        if (areaCompare != 0)
        {
            return areaCompare;
        }

        var typeCompare = string.Compare(MessageNumber, other.MessageNumber, StringComparison.Ordinal);
        if (typeCompare != 0)
        {
            return typeCompare;
        }

        var variantCompare = string.Compare(Variant, other.Variant, StringComparison.Ordinal);
        if (variantCompare != 0)
        {
            return variantCompare;
        }

        return VersionNumber.CompareTo(other.VersionNumber);
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="MessageIdentifier"/>.
    /// </summary>
    /// <param name="value">The message identifier string.</param>
    public static implicit operator MessageIdentifier(string value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="MessageIdentifier"/> to a string.
    /// </summary>
    /// <param name="identifier">The message identifier.</param>
    public static implicit operator string(MessageIdentifier identifier) => identifier.Value;

    /// <inheritdoc/>
    public override string ToString() => Value;

    #region Static Instances

    /// <summary>
    /// Predefined PAIN (Payment Initiation) message identifiers.
    /// </summary>
    public static class Pain
    {
        /// <summary>Customer Credit Transfer Initiation v9.</summary>
        public static readonly MessageIdentifier V001_09 = Create("pain", "001", "001", "09");

        /// <summary>Customer Credit Transfer Initiation v10.</summary>
        public static readonly MessageIdentifier V001_10 = Create("pain", "001", "001", "10");

        /// <summary>Customer Credit Transfer Initiation v11.</summary>
        public static readonly MessageIdentifier V001_11 = Create("pain", "001", "001", "11");

        /// <summary>Customer Payment Status Report v9.</summary>
        public static readonly MessageIdentifier V002_09 = Create("pain", "002", "001", "09");

        /// <summary>Customer Payment Status Report v10.</summary>
        public static readonly MessageIdentifier V002_10 = Create("pain", "002", "001", "10");

        /// <summary>Customer Direct Debit Initiation v8.</summary>
        public static readonly MessageIdentifier V008_08 = Create("pain", "008", "001", "08");

        /// <summary>Customer Direct Debit Initiation v9.</summary>
        public static readonly MessageIdentifier V008_09 = Create("pain", "008", "001", "09");
    }

    /// <summary>
    /// Predefined CAMT (Cash Management) message identifiers.
    /// </summary>
    public static class Camt
    {
        /// <summary>Bank To Customer Account Report v8.</summary>
        public static readonly MessageIdentifier V052_08 = Create("camt", "052", "001", "08");

        /// <summary>Bank To Customer Account Report v10.</summary>
        public static readonly MessageIdentifier V052_10 = Create("camt", "052", "001", "10");

        /// <summary>Bank To Customer Statement v8.</summary>
        public static readonly MessageIdentifier V053_08 = Create("camt", "053", "001", "08");

        /// <summary>Bank To Customer Statement v10.</summary>
        public static readonly MessageIdentifier V053_10 = Create("camt", "053", "001", "10");

        /// <summary>Bank To Customer Debit Credit Notification v8.</summary>
        public static readonly MessageIdentifier V054_08 = Create("camt", "054", "001", "08");

        /// <summary>Bank To Customer Debit Credit Notification v10.</summary>
        public static readonly MessageIdentifier V054_10 = Create("camt", "054", "001", "10");
    }

    /// <summary>
    /// Predefined PACS (Payments Clearing and Settlement) message identifiers.
    /// </summary>
    public static class Pacs
    {
        /// <summary>FI To FI Payment Status Report v10.</summary>
        public static readonly MessageIdentifier V002_10 = Create("pacs", "002", "001", "10");

        /// <summary>FI To FI Payment Status Report v11.</summary>
        public static readonly MessageIdentifier V002_11 = Create("pacs", "002", "001", "11");

        /// <summary>FI To FI Customer Credit Transfer v8.</summary>
        public static readonly MessageIdentifier V008_08 = Create("pacs", "008", "001", "08");

        /// <summary>FI To FI Customer Credit Transfer v10.</summary>
        public static readonly MessageIdentifier V008_10 = Create("pacs", "008", "001", "10");
    }

    /// <summary>
    /// Predefined HEAD (Header) message identifiers.
    /// </summary>
    public static class Head
    {
        /// <summary>Business Application Header v1.</summary>
        public static readonly MessageIdentifier V001_01 = Create("head", "001", "001", "01");

        /// <summary>Business Application Header v2.</summary>
        public static readonly MessageIdentifier V001_02 = Create("head", "001", "001", "02");

        /// <summary>Business Application Header v3.</summary>
        public static readonly MessageIdentifier V001_03 = Create("head", "001", "001", "03");
    }

    #endregion
}
