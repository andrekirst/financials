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
public readonly record struct MessageIdentifier
{
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
    /// Gets the version number (e.g., "09", "08").
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageIdentifier"/> struct.
    /// </summary>
    /// <param name="value">The full message identifier (e.g., "pain.001.001.09").</param>
    /// <exception cref="ArgumentException">Thrown when the value format is invalid.</exception>
    public MessageIdentifier(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        string[] parts = value.Split('.');
        if (parts.Length != 4)
        {
            throw new ArgumentException(
                $"Invalid message identifier format. Expected 'area.message.variant.version', got '{value}'.",
                nameof(value));
        }

        Value = value;
        BusinessArea = parts[0];
        MessageNumber = parts[1];
        Variant = parts[2];
        Version = parts[3];
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
        return new MessageIdentifier($"{businessArea}.{messageNumber}.{variant}.{version}");
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
}
