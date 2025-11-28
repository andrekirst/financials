namespace Camtify.Messages.Pain001.Models.Pain001;

/// <summary>
/// Represents Payment Type Information (PmtTpInf).
/// </summary>
/// <remarks>
/// Provides details about the payment type, service level, and category purpose.
/// </remarks>
public sealed record PaymentTypeInformation
{
    /// <summary>
    /// Gets the instruction priority.
    /// </summary>
    /// <remarks>
    /// XML Element: InstrPrty
    /// HIGH, NORM (default).
    /// </remarks>
    public string? InstructionPriority { get; init; }

    /// <summary>
    /// Gets the service level code.
    /// </summary>
    /// <remarks>
    /// XML Element: SvcLvl/Cd
    /// For SEPA: "SEPA", for non-SEPA: "NURG", "URGP".
    /// </remarks>
    public string? ServiceLevelCode { get; init; }

    /// <summary>
    /// Gets the local instrument code.
    /// </summary>
    /// <remarks>
    /// XML Element: LclInstrm/Cd
    /// For SEPA: "INST" (instant), "CORE" (core), "B2B".
    /// </remarks>
    public string? LocalInstrumentCode { get; init; }

    /// <summary>
    /// Gets the category purpose code.
    /// </summary>
    /// <remarks>
    /// XML Element: CtgyPurp/Cd
    /// SALA (salary), PENS (pension), etc.
    /// </remarks>
    public string? CategoryPurposeCode { get; init; }
}
