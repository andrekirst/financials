using System.Text;
using Camtify.Messages.Pain001.Models.Pain001;
using Camtify.Parsing;

namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Parser for pain.001.001.08 (Customer Credit Transfer Initiation) messages.
/// </summary>
/// <remarks>
/// Version 008 introduced instant payment support (TISS) and enhanced payment type information.
/// </remarks>
[Pain001Version("008", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.08")]
public sealed class Pain001v08Parser :
    Pain001ParserBase<Pain001v08Document>,
    IPain001Parser,
    IStreamingParser<PaymentInformation>
{
    private const string ExpectedNamespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.08";
    private const string VersionIdentifier = "008";

    /// <summary>
    /// Initializes a new instance of the <see cref="Pain001v08Parser"/> class.
    /// </summary>
    /// <param name="xmlStream">The XML stream to parse. Must be readable.</param>
    /// <param name="leaveOpen">If true, the stream will not be disposed after parsing.</param>
    /// <param name="cacheGroupHeader">If true, the GroupHeader will be cached during streaming.</param>
    public Pain001v08Parser(Stream xmlStream, bool leaveOpen = false, bool cacheGroupHeader = true)
        : base(xmlStream, leaveOpen, cacheGroupHeader)
    {
    }

    /// <summary>
    /// Gets the expected namespace for this parser version.
    /// </summary>
    protected override string GetExpectedNamespace() => ExpectedNamespace;

    /// <summary>
    /// Gets the version identifier for this parser.
    /// </summary>
    protected override string GetVersion() => VersionIdentifier;

    /// <summary>
    /// Parses the entire document into a Pain001v08Document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed Pain001v08Document.</returns>
    public override async Task<Pain001v08Document> ParseDocumentAsync(CancellationToken cancellationToken = default)
    {
        var groupHeader = await ParseGroupHeaderAsync(cancellationToken);
        var paymentInformationList = new List<PaymentInformation>();

        await foreach (var pmtInf in GetPaymentInformationEntriesAsync(cancellationToken))
        {
            paymentInformationList.Add(pmtInf);
        }

        return new Pain001v08Document
        {
            Message = new CustomerCreditTransferInitiation
            {
                GroupHeader = groupHeader,
                PaymentInformation = paymentInformationList
            }
        };
    }

    /// <summary>
    /// Creates a Pain001v08Parser from an XML string.
    /// </summary>
    /// <param name="xml">The XML content as string.</param>
    /// <returns>A new Pain001v08Parser instance.</returns>
    /// <exception cref="ArgumentException">Thrown if xml is null or empty.</exception>
    public static Pain001v08Parser FromString(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException("XML content cannot be null or empty.", nameof(xml));
        }

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new Pain001v08Parser(stream, leaveOpen: false);
    }

    /// <summary>
    /// Creates a Pain001v08Parser from a file.
    /// </summary>
    /// <param name="filePath">The path to the XML file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A new Pain001v08Parser instance.</returns>
    /// <exception cref="ArgumentException">Thrown if filePath is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    public static async Task<Pain001v08Parser> FromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return new Pain001v08Parser(stream, leaveOpen: false);
    }

    /// <summary>
    /// Streams payment information entries from the XML document.
    /// </summary>
    /// <param name="stream">The stream containing the XML message (unused - uses internal stream).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of payment information entries.</returns>
    /// <remarks>
    /// Explicit implementation of IStreamingParser interface.
    /// The stream parameter is ignored as the parser uses its internal stream.
    /// </remarks>
    IAsyncEnumerable<PaymentInformation> IStreamingParser<PaymentInformation>.StreamEntriesAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        return GetPaymentInformationEntriesAsync(cancellationToken);
    }
}
