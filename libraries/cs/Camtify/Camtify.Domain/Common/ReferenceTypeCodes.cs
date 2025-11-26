namespace Camtify.Domain.Common;

/// <summary>
/// ISO 20022 reference type codes for creditor reference information.
/// </summary>
/// <remarks>
/// These codes identify the type of creditor reference in structured remittance.
/// XML Path: CdtrRefInf/Tp/CdOrPrtry/Cd
/// </remarks>
public static class ReferenceTypeCodes
{
    /// <summary>
    /// Structured Communication Reference - RF reference (ISO 11649).
    /// </summary>
    /// <remarks>
    /// Used for structured creditor references following the RF format.
    /// Example: RF18539007547034
    /// </remarks>
    public const string SCOR = "SCOR";

    /// <summary>
    /// Remittance Advice Reference - Reference to a remittance advice.
    /// </summary>
    public const string RADM = "RADM";

    /// <summary>
    /// Related Payment Instruction Reference - Reference to related payment.
    /// </summary>
    public const string RPIN = "RPIN";

    /// <summary>
    /// Foreign Exchange Deal Reference - Reference to FX transaction.
    /// </summary>
    public const string FXDR = "FXDR";

    /// <summary>
    /// Document Number - Generic document reference.
    /// </summary>
    public const string DISP = "DISP";

    /// <summary>
    /// Purchase Order Number - Reference to a purchase order.
    /// </summary>
    public const string PUOR = "PUOR";

    /// <summary>
    /// Gets all available reference type codes.
    /// </summary>
    public static IReadOnlyList<string> All { get; } = new[]
    {
        SCOR, RADM, RPIN, FXDR, DISP, PUOR
    };

    /// <summary>
    /// Gets the description for a reference type code.
    /// </summary>
    /// <param name="code">The reference type code.</param>
    /// <returns>The description, or the code itself if unknown.</returns>
    public static string GetDescription(string? code) => code switch
    {
        SCOR => "Structured Communication Reference (RF)",
        RADM => "Remittance Advice Reference",
        RPIN => "Related Payment Instruction Reference",
        FXDR => "Foreign Exchange Deal Reference",
        DISP => "Document Number",
        PUOR => "Purchase Order Number",
        _ => code ?? string.Empty
    };
}
