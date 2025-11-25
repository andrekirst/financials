namespace Camtify.Domain.Common;

/// <summary>
/// Digital signature according to XML-DSIG standard (XML: Sgntr).
/// </summary>
/// <remarks>
/// Contains the digital signature for message authentication and integrity.
/// The signature follows the W3C XML Signature standard (XML-DSIG).
/// Detailed parsing of the signature structure is outside the scope of this library.
/// </remarks>
public sealed record Signature
{
    /// <summary>
    /// Raw XML content of the signature element.
    /// </summary>
    /// <remarks>
    /// Contains the complete XML-DSIG signature structure.
    /// Applications requiring signature validation should use
    /// specialized cryptographic libraries to process this content.
    /// </remarks>
    public required string RawXml { get; init; }

    /// <summary>
    /// Signature algorithm identifier (e.g., "RSA-SHA256", "ECDSA-SHA384").
    /// </summary>
    /// <remarks>
    /// Extracted from the SignatureMethod element of the XML-DSIG structure.
    /// Common algorithms include:
    /// - http://www.w3.org/2001/04/xmldsig-more#rsa-sha256
    /// - http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384
    /// </remarks>
    public string? Algorithm { get; init; }

    /// <summary>
    /// Certificate reference or identifier used for signing.
    /// </summary>
    public string? CertificateReference { get; init; }
}
