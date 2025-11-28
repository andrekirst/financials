using Camtify.Core;
using Camtify.Messages.Pain001.Parsers;
using Camtify.Messages.Pain001.Models.Pain001;
using Camtify.Parsing;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for Pain001ParserFactory IParserFactory interface implementation.
/// Verifies Issue #23 acceptance criteria.
/// </summary>
public class Pain001ParserFactoryIParserFactoryTests
{
    private readonly Pain001ParserFactory _factory;

    public Pain001ParserFactoryIParserFactoryTests()
    {
        _factory = new Pain001ParserFactory();
    }

    [Theory]
    [InlineData("pain", "001", "001", "03")]
    [InlineData("pain", "001", "001", "08")]
    [InlineData("pain", "001", "001", "09")]
    [InlineData("pain", "001", "001", "10")]
    [InlineData("pain", "001", "001", "11")]
    public void SupportsMessage_ShouldReturnTrue_ForSupportedVersions(
        string businessArea,
        string messageNumber,
        string variant,
        string version)
    {
        // Arrange
        var messageId = MessageIdentifier.Create(businessArea, messageNumber, variant, version);

        // Act
        var isSupported = _factory.SupportsMessage(messageId);

        // Assert
        Assert.True(isSupported, $"Expected factory to support {messageId}");
    }

    [Theory]
    [InlineData("pain", "001", "001", "01")]
    [InlineData("pain", "001", "001", "02")]
    [InlineData("pain", "002", "001", "09")]
    [InlineData("camt", "053", "001", "08")]
    [InlineData("pacs", "008", "001", "10")]
    public void SupportsMessage_ShouldReturnFalse_ForUnsupportedVersions(
        string businessArea,
        string messageNumber,
        string variant,
        string version)
    {
        // Arrange
        var messageId = MessageIdentifier.Create(businessArea, messageNumber, variant, version);

        // Act
        var isSupported = _factory.SupportsMessage(messageId);

        // Assert
        Assert.False(isSupported, $"Expected factory not to support {messageId}");
    }

    [Fact]
    public void SupportsBusinessArea_ShouldReturnTrue_ForPain()
    {
        // Act
        var isSupported = _factory.SupportsBusinessArea("pain");

        // Assert
        Assert.True(isSupported);
    }

    [Theory]
    [InlineData("Pain")]
    [InlineData("PAIN")]
    [InlineData("PaIn")]
    public void SupportsBusinessArea_ShouldBeCaseInsensitive(string businessArea)
    {
        // Act
        var isSupported = _factory.SupportsBusinessArea(businessArea);

        // Assert
        Assert.True(isSupported);
    }

    [Theory]
    [InlineData("camt")]
    [InlineData("pacs")]
    [InlineData("acmt")]
    public void SupportsBusinessArea_ShouldReturnFalse_ForUnsupportedAreas(string businessArea)
    {
        // Act
        var isSupported = _factory.SupportsBusinessArea(businessArea);

        // Assert
        Assert.False(isSupported);
    }

    [Fact]
    public void SupportedMessages_ShouldContainAllVersions()
    {
        // Act
        var supportedMessages = _factory.SupportedMessages;

        // Assert
        Assert.NotEmpty(supportedMessages);
        Assert.Equal(5, supportedMessages.Count); // v03, v08, v09, v10, v11

        var versions = supportedMessages.Select(m => m.Version).OrderBy(v => v).ToList();
        Assert.Contains("03", versions);
        Assert.Contains("08", versions);
        Assert.Contains("09", versions);
        Assert.Contains("10", versions);
        Assert.Contains("11", versions);
    }

    [Fact]
    public void SupportedMessages_ShouldReturnReadOnlyCollection()
    {
        // Act
        var supportedMessages = _factory.SupportedMessages;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyCollection<MessageIdentifier>>(supportedMessages);
    }

    [Fact]
    public void GetSupportedVersions_ShouldReturnPain001Versions()
    {
        // Act
        var versions = _factory.GetSupportedVersions("pain", "001");

        // Assert
        Assert.NotEmpty(versions);
        Assert.Equal(5, versions.Count);

        var versionNumbers = versions.Select(v => v.Version).OrderBy(v => v).ToList();
        Assert.Equal(new[] { "03", "08", "09", "10", "11" }, versionNumbers);
    }

    [Fact]
    public void GetSupportedVersions_ShouldBeCaseInsensitive()
    {
        // Act
        var versionsLower = _factory.GetSupportedVersions("pain", "001");
        var versionsUpper = _factory.GetSupportedVersions("PAIN", "001");
        var versionsMixed = _factory.GetSupportedVersions("PaIn", "001");

        // Assert
        Assert.Equal(versionsLower.Count, versionsUpper.Count);
        Assert.Equal(versionsLower.Count, versionsMixed.Count);
    }

    [Fact]
    public void GetSupportedVersions_ShouldReturnEmpty_ForUnsupportedMessages()
    {
        // Act
        var versions = _factory.GetSupportedVersions("pain", "002");

        // Assert
        Assert.Empty(versions);
    }

    [Fact]
    public async Task DetectAndCreateParserAsync_ShouldCreateCorrectParser_ForV09()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act
        var (parser, messageId) = await _factory.DetectAndCreateParserAsync(stream);

        // Assert
        Assert.NotNull(parser);
        Assert.IsType<Pain001v09Parser>(parser);
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("09", messageId.Version);
    }

    [Fact]
    public async Task DetectAndCreateParserAsync_Generic_ShouldCreateTypedParser_ForV09()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act
        var (parser, messageId) = await _factory.DetectAndCreateParserAsync<IIso20022Document<CustomerCreditTransferInitiation>>(stream);

        // Assert
        Assert.NotNull(parser);
        Assert.IsAssignableFrom<IIso20022Parser<IIso20022Document<CustomerCreditTransferInitiation>>>(parser);
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("09", messageId.Version);
    }

    [Fact]
    public async Task DetectAndCreateParserAsync_ShouldThrowXmlException_ForInvalidXml()
    {
        // Arrange
        var invalidXml = "not valid xml";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(invalidXml));

        // Act & Assert
        await Assert.ThrowsAsync<System.Xml.XmlException>(
            async () => await _factory.DetectAndCreateParserAsync(stream));
    }

    [Fact]
    public async Task DetectAndCreateParserAsync_ShouldThrowParserNotFoundException_ForUnsupportedVersion()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.01"">
</Document>";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ParserNotFoundException>(
            async () => await _factory.DetectAndCreateParserAsync(stream));

        Assert.Contains("pain.001.001.01", exception.Message);
    }

    [Fact]
    public void CreateParser_ShouldThrowNotSupportedException()
    {
        // Arrange
        var messageId = MessageIdentifier.Pain.V001_09;

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(
            () => _factory.CreateParser<IIso20022Document<CustomerCreditTransferInitiation>>(messageId));

        Assert.Contains("Pain.001 parsers require a stream at construction time", exception.Message);
        Assert.Contains("DetectAndCreateParserAsync", exception.Message);
    }

    [Fact]
    public void CreateStreamingParser_ShouldThrowNotSupportedException()
    {
        // Arrange
        var messageId = MessageIdentifier.Pain.V001_09;

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(
            () => _factory.CreateStreamingParser<PaymentInformation>(messageId));

        Assert.Contains("Pain.001 parsers require a stream at construction time", exception.Message);
    }

    [Fact]
    public void CreateParser_Untyped_ShouldThrowNotSupportedException()
    {
        // Arrange
        var messageId = MessageIdentifier.Pain.V001_09;

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(
            () => _factory.CreateParser(messageId));

        Assert.Contains("Pain.001 parsers require a stream at construction time", exception.Message);
    }
}
