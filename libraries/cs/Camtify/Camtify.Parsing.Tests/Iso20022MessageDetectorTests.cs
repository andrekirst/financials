using System.Text;
using Camtify.Core;
using Camtify.Parsing;

namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for <see cref="Iso20022MessageDetector"/>.
/// </summary>
public class Iso20022MessageDetectorTests
{
    private readonly Iso20022MessageDetector _detector;

    public Iso20022MessageDetectorTests()
    {
        _detector = new Iso20022MessageDetector();
    }

    [Theory]
    [InlineData("pain.001.001.09", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData("camt.053.001.08", "urn:iso:std:iso:20022:tech:xsd:camt.053.001.08")]
    [InlineData("camt.052.001.10", "urn:iso:std:iso:20022:tech:xsd:camt.052.001.10")]
    [InlineData("camt.054.001.08", "urn:iso:std:iso:20022:tech:xsd:camt.054.001.08")]
    [InlineData("pacs.008.001.10", "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.10")]
    public async Task DetectAsync_WithValidIsoNamespace_ReturnsCorrectMessageId(
        string expectedId,
        string namespaceUri)
    {
        // Arrange
        var xml = $@"<?xml version=""1.0""?>
<Document xmlns=""{namespaceUri}"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectAsync(stream);

        // Assert
        result.ToString().ShouldBe(expectedId);
    }

    [Theory]
    [InlineData("pain.001.001.09", "urn:swift:xsd:pain.001.001.09")]
    [InlineData("pacs.008.001.10", "urn:swift:xsd:pacs.008.001.10")]
    public async Task DetectAsync_WithSwiftNamespace_ReturnsCorrectMessageId(
        string expectedId,
        string namespaceUri)
    {
        // Arrange
        var xml = $@"<?xml version=""1.0""?>
<Document xmlns=""{namespaceUri}"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectAsync(stream);

        // Assert
        result.ToString().ShouldBe(expectedId);
    }

    [Fact]
    public async Task DetectAsync_WithBizMsgEnvlp_ExtractsMessageIdFromDocument()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<BizMsgEnvlp>
    <AppHdr xmlns=""urn:iso:std:iso:20022:tech:xsd:head.001.001.02"">
        <Fr>SENDER</Fr>
        <To>RECEIVER</To>
        <BizMsgIdr>MSG123</BizMsgIdr>
        <MsgDefIdr>pain.001.001.09</MsgDefIdr>
        <CreDt>2024-01-15T10:30:00Z</CreDt>
    </AppHdr>
    <Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
        <CstmrCdtTrfInitn/>
    </Document>
</BizMsgEnvlp>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectAsync(stream);

        // Assert
        result.ToString().ShouldBe("pain.001.001.09");
    }

    [Fact]
    public async Task DetectWithDetailsAsync_WithStandaloneDocument_ReturnsCorrectDetails()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectWithDetailsAsync(stream);

        // Assert
        result.MessageId.ToString().ShouldBe("pain.001.001.09");
        result.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
        result.HasApplicationHeader.ShouldBeFalse();
        result.ApplicationHeaderMessageId.ShouldBeNull();
        result.MessageDefinitionIdentifier.ShouldBeNull();
        result.RootElementName.ShouldBe("Document");
        result.MessageElementName.ShouldBe("CstmrCdtTrfInitn");
        result.Variant.ShouldBe(MessageVariant.Standalone);
    }

    [Fact]
    public async Task DetectWithDetailsAsync_WithBizMsgEnvlp_ReturnsCorrectDetails()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<BizMsgEnvlp>
    <AppHdr xmlns=""urn:iso:std:iso:20022:tech:xsd:head.001.001.02"">
        <Fr>SENDER</Fr>
        <To>RECEIVER</To>
        <BizMsgIdr>MSG123</BizMsgIdr>
        <MsgDefIdr>pain.001.001.09</MsgDefIdr>
        <CreDt>2024-01-15T10:30:00Z</CreDt>
    </AppHdr>
    <Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
        <CstmrCdtTrfInitn/>
    </Document>
</BizMsgEnvlp>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectWithDetailsAsync(stream);

        // Assert
        result.MessageId.ToString().ShouldBe("pain.001.001.09");
        result.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
        result.HasApplicationHeader.ShouldBeTrue();
        result.ApplicationHeaderMessageId.ShouldNotBeNull();
        result.ApplicationHeaderMessageId!.Value.ToString().ShouldBe("head.001.001.02");
        result.MessageDefinitionIdentifier.ShouldBe("pain.001.001.09");
        result.RootElementName.ShouldBe("BizMsgEnvlp");
        result.MessageElementName.ShouldBe("CstmrCdtTrfInitn");
        result.Variant.ShouldBe(MessageVariant.WithApplicationHeader);
    }

    [Fact]
    public async Task DetectWithDetailsAsync_WithSwiftNamespace_ReturnsSwiftVariant()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:swift:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectWithDetailsAsync(stream);

        // Assert
        result.MessageId.ToString().ShouldBe("pain.001.001.09");
        result.Variant.ShouldBe(MessageVariant.Swift);
    }

    [Fact]
    public async Task DetectWithDetailsAsync_WithCbprPlusNamespace_ReturnsCbprPlusVariant()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09$cbpr_plus"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectWithDetailsAsync(stream);

        // Assert
        result.MessageId.ToString().ShouldBe("pain.001.001.09");
        result.Variant.ShouldBe(MessageVariant.CbprPlus);
    }

    [Fact]
    public async Task TryDetectAsync_WithValidXml_ReturnsSuccess()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var (success, messageId, error) = await _detector.TryDetectAsync(stream);

        // Assert
        success.ShouldBeTrue();
        messageId.ShouldNotBeNull();
        messageId!.Value.ToString().ShouldBe("pain.001.001.09");
        error.ShouldBeNull();
    }

    [Fact]
    public async Task TryDetectAsync_WithInvalidXml_ReturnsFailure()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:invalid.format"">
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var (success, messageId, error) = await _detector.TryDetectAsync(stream);

        // Assert
        success.ShouldBeFalse();
        messageId.ShouldBeNull();
        error.ShouldNotBeNull();
    }

    [Fact]
    public async Task DetectAsync_WithInvalidRootElement_ThrowsMessageDetectionException()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<InvalidRoot xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</InvalidRoot>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act & Assert
        var exception = await Should.ThrowAsync<MessageDetectionException>(
            async () => await _detector.DetectAsync(stream));

        exception.Message.ShouldContain("InvalidRoot");
        exception.FoundRootElement.ShouldBe("InvalidRoot");
    }

    [Fact]
    public async Task DetectAsync_WithMissingNamespace_ThrowsMessageDetectionException()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document>
    <CstmrCdtTrfInitn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act & Assert
        await Should.ThrowAsync<MessageDetectionException>(
            async () => await _detector.DetectAsync(stream));
    }

    [Fact]
    public async Task DetectAsync_WithEmptyStream_ThrowsXmlException()
    {
        // Arrange
        var xml = @"";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act & Assert
        await Should.ThrowAsync<System.Xml.XmlException>(
            async () => await _detector.DetectAsync(stream));
    }

    [Fact]
    public async Task DetectAsync_WithNullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(
            async () => await _detector.DetectAsync(null!));
    }

    [Fact]
    public async Task DetectAsync_WithCamt053_ReturnsCorrectMessageId()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:camt.053.001.08"">
    <BkToCstmrStmt/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectAsync(stream);

        // Assert
        result.ToString().ShouldBe("camt.053.001.08");
    }

    [Fact]
    public async Task DetectAsync_WithCamt052_ReturnsCorrectMessageId()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:camt.052.001.10"">
    <BkToCstmrAcctRpt/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectAsync(stream);

        // Assert
        result.ToString().ShouldBe("camt.052.001.10");
    }

    [Fact]
    public async Task DetectAsync_WithCamt054_ReturnsCorrectMessageId()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:camt.054.001.08"">
    <BkToCstmrDbtCdtNtfctn/>
</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await _detector.DetectAsync(stream);

        // Assert
        result.ToString().ShouldBe("camt.054.001.08");
    }
}
