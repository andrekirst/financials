namespace Camtify.Domain.Common;

/// <summary>
/// Status reason information according to ISO 20022.
/// </summary>
/// <remarks>
/// Provides detailed information about a status reason including the reason itself,
/// additional information, and who originated the reason.
/// </remarks>
public readonly record struct StatusReasonInformation
{
    /// <summary>
    /// Gets the status reason (code or proprietary).
    /// </summary>
    public StatusReason? Reason { get; }

    /// <summary>
    /// Gets additional information about the status reason.
    /// </summary>
    public IReadOnlyList<string>? AdditionalInformation { get; }

    /// <summary>
    /// Gets the party that originated the status.
    /// </summary>
    public PartyIdentification? Originator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusReasonInformation"/> struct.
    /// </summary>
    /// <param name="reason">The status reason.</param>
    /// <param name="additionalInformation">Additional information.</param>
    /// <param name="originator">The originating party.</param>
    public StatusReasonInformation(
        StatusReason? reason = null,
        IReadOnlyList<string>? additionalInformation = null,
        PartyIdentification? originator = null)
    {
        Reason = reason;
        AdditionalInformation = additionalInformation;
        Originator = originator;
    }

    /// <summary>
    /// Creates a status reason information with only a reason code.
    /// </summary>
    /// <param name="reasonCode">The reason code.</param>
    /// <returns>A new StatusReasonInformation instance.</returns>
    public static StatusReasonInformation FromCode(string reasonCode)
    {
        return new StatusReasonInformation(reason: StatusReason.FromCode(reasonCode));
    }

    /// <summary>
    /// Creates a status reason information with a reason code and additional information.
    /// </summary>
    /// <param name="reasonCode">The reason code.</param>
    /// <param name="additionalInfo">Additional information text.</param>
    /// <returns>A new StatusReasonInformation instance.</returns>
    public static StatusReasonInformation FromCodeWithInfo(string reasonCode, params string[] additionalInfo)
    {
        return new StatusReasonInformation(
            reason: StatusReason.FromCode(reasonCode),
            additionalInformation: additionalInfo);
    }

    /// <summary>
    /// Gets a value indicating whether this instance has a reason.
    /// </summary>
    public bool HasReason => Reason.HasValue;

    /// <summary>
    /// Gets a value indicating whether this instance has additional information.
    /// </summary>
    public bool HasAdditionalInformation => AdditionalInformation?.Count > 0;

    /// <summary>
    /// Gets a value indicating whether this instance has an originator.
    /// </summary>
    public bool HasOriginator => Originator is not null;

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string>();

        if (HasReason)
        {
            parts.Add($"Reason: {Reason}");
        }

        if (HasAdditionalInformation)
        {
            parts.Add($"Info: {string.Join(", ", AdditionalInformation!)}");
        }

        if (HasOriginator)
        {
            parts.Add($"Originator: {Originator}");
        }

        return parts.Count > 0 ? string.Join(" | ", parts) : "Empty";
    }
}
