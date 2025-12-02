namespace Camtify.Parsing;

/// <summary>
/// Variants of ISO 20022 documents.
/// </summary>
public enum MessageVariant
{
    /// <summary>
    /// Standalone Document without Business Application Header.
    /// </summary>
    Standalone,

    /// <summary>
    /// Document with Business Application Header (BAH).
    /// </summary>
    WithApplicationHeader,

    /// <summary>
    /// SWIFT-specific format.
    /// </summary>
    Swift,

    /// <summary>
    /// CBPR+ format (Cross-Border Payments and Reporting Plus).
    /// </summary>
    CbprPlus
}
