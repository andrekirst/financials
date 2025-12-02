using System.Text;
using System.Xml;
using Camtify.Core;
using Camtify.Core.Parsing;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for the <see cref="Iso20022ParserBase{TDocument}"/> class.
/// </summary>
public class Iso20022ParserBaseTests
{
    private const string ValidPain001Xml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <Document xmlns="urn:iso:std:iso:20022:tech:xsd:pain.001.001.09">
            <CstmrCdtTrfInitn>
                <GrpHdr>
                    <MsgId>TEST-MSG-001</MsgId>
                </GrpHdr>
            </CstmrCdtTrfInitn>
        </Document>
        """;

    private const string ValidPain001XmlWithBah = """
        <?xml version="1.0" encoding="UTF-8"?>
        <Document xmlns="urn:iso:std:iso:20022:tech:xsd:pain.001.001.09">
            <AppHdr xmlns="urn:iso:std:iso:20022:tech:xsd:head.001.001.01">
                <BizMsgIdr>TEST-BAH-001</BizMsgIdr>
                <MsgDefIdr>pain.001.001.09</MsgDefIdr>
                <CreDt>2024-01-01T12:00:00Z</CreDt>
            </AppHdr>
            <CstmrCdtTrfInitn>
                <GrpHdr>
                    <MsgId>TEST-MSG-002</MsgId>
                </GrpHdr>
            </CstmrCdtTrfInitn>
        </Document>
        """;

    private const string ValidPain001V10Xml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <Document xmlns="urn:iso:std:iso:20022:tech:xsd:pain.001.001.10">
            <CstmrCdtTrfInitn>
                <GrpHdr>
                    <MsgId>TEST-MSG-003</MsgId>
                </GrpHdr>
            </CstmrCdtTrfInitn>
        </Document>
        """;

    private const string UnsupportedCamt053Xml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <Document xmlns="urn:iso:std:iso:20022:tech:xsd:camt.053.001.08">
            <BkToCstmrStmt>
                <GrpHdr>
                    <MsgId>TEST-MSG-004</MsgId>
                </GrpHdr>
            </BkToCstmrStmt>
        </Document>
        """;

    /// <summary>
    /// Mock document for testing.
    /// </summary>
    public sealed record MockDocument
    {
        public required string MessageId { get; init; }
        public BusinessApplicationHeader? ApplicationHeader { get; init; }
        public MessageIdentifier? DetectedMessageId { get; init; }
    }

    /// <summary>
    /// Mock parser implementation for testing.
    /// </summary>
    private sealed class MockParser : Iso20022ParserBase<MockDocument>
    {
        public override IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; } = new[]
        {
            new MessageIdentifier("pain.001.001.09"),
            new MessageIdentifier("pain.001.001.10")
        };

        public MockParser(
            IXmlReaderFactory xmlReaderFactory,
            IMessageDetector messageDetector)
            : base(xmlReaderFactory, messageDetector, NullLogger.Instance)
        {
        }

        protected override async Task<MockDocument> ParseDocumentCoreAsync(
            XmlReader reader,
            MessageIdentifier messageId,
            BusinessApplicationHeader? applicationHeader,
            ParseOptions options,
            List<ParseError> errors,
            List<ParseWarning> warnings,
            CancellationToken cancellationToken)
        {
            // Reader is already positioned at CstmrCdtTrfInitn or AppHdr
            // Navigate to message root (CstmrCdtTrfInitn) if needed
            if (reader.LocalName != "CstmrCdtTrfInitn")
            {
                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.LocalName == "CstmrCdtTrfInitn")
                    {
                        break;
                    }
                }
            }

            // Navigate to GrpHdr (need to enter CstmrCdtTrfInitn first)
            if (!await MoveToElementAsync(reader, "GrpHdr", cancellationToken))
            {
                AddError(errors, "GrpHdr element not found", reader);
                throw new Iso20022ParsingException("GrpHdr element not found", errors, warnings);
            }

            // Navigate to MsgId
            if (!await MoveToElementAsync(reader, "MsgId", cancellationToken))
            {
                AddError(errors, "MsgId element not found", reader);
                throw new Iso20022ParsingException("MsgId element not found", errors, warnings);
            }

            var msgId = await ReadElementContentAsync(reader);

            if (string.IsNullOrEmpty(msgId))
            {
                AddWarning(warnings, "MsgId is empty", reader);
            }

            return new MockDocument
            {
                MessageId = msgId ?? string.Empty,
                ApplicationHeader = applicationHeader,
                DetectedMessageId = messageId
            };
        }
    }

    [Fact]
    public async Task ParseAsync_WithValidPain001_ShouldParseSuccessfully()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001Xml));

        // Act
        var result = await parser.ParseAsync(stream, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe("TEST-MSG-001");
        result.DetectedMessageId.ShouldBe(new MessageIdentifier("pain.001.001.09"));
        result.ApplicationHeader.ShouldBeNull();
    }

    [Fact]
    public async Task ParseAsync_WithBusinessApplicationHeader_ShouldParseBah()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001XmlWithBah));
        var options = new ParseOptions { ParseApplicationHeader = true };

        // Act
        var result = await parser.ParseAsync(stream, options);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe("TEST-MSG-002");
        result.ApplicationHeader.ShouldNotBeNull();
        result.ApplicationHeader!.BusinessMessageIdentifier.ShouldBe("TEST-BAH-001");
    }

    [Fact]
    public async Task ParseAsync_WithProgressReporter_ShouldReportProgress()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        var progressReports = new List<ParseProgress>();
        var progress = new Progress<ParseProgress>(p => progressReports.Add(p));

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001Xml));
        var options = new ParseOptions { Progress = progress };

        // Act
        var result = await parser.ParseAsync(stream, options);

        // Assert
        result.ShouldNotBeNull();
        progressReports.ShouldNotBeEmpty();
        progressReports.ShouldContain(p => p.Status == ParseStatus.Starting);
        progressReports.ShouldContain(p => p.Status == ParseStatus.ParsingBody);
        progressReports.ShouldContain(p => p.Status == ParseStatus.Completed);
    }

    [Fact]
    public async Task ParseAsync_WithDifferentSupportedVersion_ShouldParseSuccessfully()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001V10Xml));

        // Act
        var result = await parser.ParseAsync(stream, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe("TEST-MSG-003");
        result.DetectedMessageId.ShouldBe(new MessageIdentifier("pain.001.001.10"));
    }

    [Fact]
    public async Task ParseAsync_WithUnsupportedMessage_ShouldThrowParserNotFoundException()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(UnsupportedCamt053Xml));

        // Act & Assert
        var ex = await Should.ThrowAsync<ParserNotFoundException>(async () =>
            await parser.ParseAsync(stream, CancellationToken.None));

        ex.Message.ShouldContain("camt.053.001.08");
    }

    [Fact]
    public async Task ParseAsync_FromString_ShouldParseSuccessfully()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        // Act
        var result = await parser.ParseFromStringAsync(ValidPain001Xml);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe("TEST-MSG-001");
    }

    [Fact]
    public async Task ParseAsync_FromFile_ShouldParseSuccessfully()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, ValidPain001Xml);

            // Act
            var result = await parser.ParseAsync(tempFile);

            // Assert
            result.ShouldNotBeNull();
            result.MessageId.ShouldBe("TEST-MSG-001");
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task CanParseAsync_WithSupportedMessage_ShouldReturnTrue()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001Xml));

        // Act
        var canParse = await parser.CanParseAsync(stream);

        // Assert
        canParse.ShouldBeTrue();
    }

    [Fact]
    public async Task CanParseAsync_WithUnsupportedMessage_ShouldReturnFalse()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(UnsupportedCamt053Xml));

        // Act
        var canParse = await parser.CanParseAsync(stream);

        // Assert
        canParse.ShouldBeFalse();
    }

    [Fact]
    public void MessageIdentifier_ShouldReturnFirstSupportedMessage()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        messageId.ShouldBe(new MessageIdentifier("pain.001.001.09"));
    }

    [Fact]
    public async Task ParseAsync_WithNullOptions_ShouldUseDefaults()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001Xml));

        // Act
        var result = await parser.ParseAsync(stream, null);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe("TEST-MSG-001");
    }

    [Fact]
    public async Task ParseAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Iso20022MessageDetector();
        var parser = new MockParser(xmlReaderFactory, messageDetector);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidPain001Xml));

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await parser.ParseAsync(stream, null, cts.Token));
    }
}
