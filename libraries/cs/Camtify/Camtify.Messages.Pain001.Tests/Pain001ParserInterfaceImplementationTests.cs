using Camtify.Core;
using Camtify.Messages.Pain001.Parsers;
using Camtify.Messages.Pain001.Models.Pain001;
using Camtify.Parsing;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for new IIso20022Parser and IStreamingParser interface implementations.
/// Verifies Issue #23 acceptance criteria for parser interface methods.
/// </summary>
public class Pain001ParserInterfaceImplementationTests
{
    [Fact]
    public void Pain001v03Parser_MessageIdentifier_ShouldReturnCorrectValue()
    {
        // Arrange
        var xml = TestData.GetPain001v03Xml();
        var parser = Pain001v03Parser.FromString(xml);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("001", messageId.Variant);
        Assert.Equal("03", messageId.Version);
        Assert.Equal("pain.001.001.03", messageId.Value);
    }

    [Fact]
    public void Pain001v08Parser_MessageIdentifier_ShouldReturnCorrectValue()
    {
        // Arrange
        var xml = TestData.GetPain001v08Xml();
        var parser = Pain001v08Parser.FromString(xml);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("001", messageId.Variant);
        Assert.Equal("08", messageId.Version);
        Assert.Equal("pain.001.001.08", messageId.Value);
    }

    [Fact]
    public void Pain001v09Parser_MessageIdentifier_ShouldReturnCorrectValue()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        var parser = Pain001v09Parser.FromString(xml);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("001", messageId.Variant);
        Assert.Equal("09", messageId.Version);
        Assert.Equal("pain.001.001.09", messageId.Value);
    }

    [Fact]
    public void Pain001v10Parser_MessageIdentifier_ShouldReturnCorrectValue()
    {
        // Arrange
        var xml = TestData.GetPain001v10Xml();
        var parser = Pain001v10Parser.FromString(xml);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("001", messageId.Variant);
        Assert.Equal("10", messageId.Version);
        Assert.Equal("pain.001.001.10", messageId.Value);
    }

    [Fact]
    public void Pain001v11Parser_MessageIdentifier_ShouldReturnCorrectValue()
    {
        // Arrange
        var xml = TestData.GetPain001v11Xml();
        var parser = Pain001v11Parser.FromString(xml);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        Assert.Equal("pain", messageId.BusinessArea);
        Assert.Equal("001", messageId.MessageNumber);
        Assert.Equal("001", messageId.Variant);
        Assert.Equal("11", messageId.Version);
        Assert.Equal("pain.001.001.11", messageId.Value);
    }

    [Fact]
    public async Task Pain001v09Parser_CanParseAsync_ShouldReturnTrue_ForValidStream()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        var parser = new Pain001v09Parser(stream, leaveOpen: true);

        // Act
        var canParse = await parser.CanParseAsync(stream);

        // Assert
        Assert.True(canParse);
    }

    [Fact]
    public async Task Pain001v09Parser_CanParseAsync_ShouldReturnFalse_ForWrongNamespace()
    {
        // Arrange
        var wrongXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.08"">
</Document>";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(wrongXml));
        var parser = new Pain001v09Parser(stream, leaveOpen: true);

        // Act
        var canParse = await parser.CanParseAsync(stream);

        // Assert
        Assert.False(canParse);
    }

    [Fact]
    public async Task Pain001v09Parser_CanParseAsync_ShouldResetStreamPosition()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        var parser = new Pain001v09Parser(stream, leaveOpen: true);
        var initialPosition = stream.Position;

        // Act
        await parser.CanParseAsync(stream);

        // Assert
        Assert.Equal(initialPosition, stream.Position);
    }

    [Fact]
    public async Task Pain001v09Parser_ParseAsync_ShouldReturnValidDocument()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        var parser = new Pain001v09Parser(stream);

        // Act
        var document = await parser.ParseAsync(stream);

        // Assert
        Assert.NotNull(document);
        Assert.NotNull(document.Message);
        Assert.NotNull(document.Message.GroupHeader);
    }

    [Fact]
    public async Task Pain001v09Parser_ParseAsync_ShouldImplementIIso20022Parser()
    {
        // Arrange
        var xml = TestData.GetPain001v09Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        IIso20022Parser<IIso20022Document<CustomerCreditTransferInitiation>> parser = new Pain001v09Parser(stream);

        // Act
        var document = await parser.ParseAsync(stream);

        // Assert
        Assert.NotNull(document);
        Assert.IsAssignableFrom<IIso20022Document<CustomerCreditTransferInitiation>>(document);
    }

    [Fact]
    public async Task Pain001v09Parser_StreamEntriesAsync_ShouldImplementIStreamingParser()
    {
        // Arrange
        var xml = TestData.GetPain001v09XmlWithMultiplePayments();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        IStreamingParser<PaymentInformation> parser = new Pain001v09Parser(stream);

        // Act
        var entries = new List<PaymentInformation>();
        await foreach (var entry in parser.StreamEntriesAsync(stream))
        {
            entries.Add(entry);
        }

        // Assert
        Assert.NotEmpty(entries);
        Assert.All(entries, entry => Assert.NotNull(entry));
    }

    [Fact]
    public async Task Pain001v09Parser_StreamEntriesAsync_ShouldStreamPaymentInformation()
    {
        // Arrange
        var xml = TestData.GetPain001v09XmlWithMultiplePayments();
        var parser = Pain001v09Parser.FromString(xml);

        // Act
        var entries = new List<PaymentInformation>();
        await foreach (var entry in parser.GetPaymentInformationEntriesAsync())
        {
            entries.Add(entry);
        }

        // Assert
        Assert.NotEmpty(entries);
        Assert.All(entries, entry =>
        {
            Assert.NotNull(entry.PaymentInformationIdentification);
            Assert.NotNull(entry.PaymentMethod);
        });
    }

    [Fact]
    public async Task Pain001v11Parser_CanParseAsync_ShouldValidateNamespace()
    {
        // Arrange
        var xml = TestData.GetPain001v11Xml();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        var parser = new Pain001v11Parser(stream, leaveOpen: true);

        // Act
        var canParse = await parser.CanParseAsync(stream);

        // Assert
        Assert.True(canParse);
    }

    [Theory]
    [InlineData("pain.001.001.03")]
    [InlineData("pain.001.001.08")]
    [InlineData("pain.001.001.09")]
    [InlineData("pain.001.001.10")]
    [InlineData("pain.001.001.11")]
    public void AllParsers_MessageIdentifier_ShouldMatchExpectedVersion(string expectedVersion)
    {
        // Arrange
        var parts = expectedVersion.Split('.');
        var version = parts[3];
        var xml = GetTestXmlForVersion(version);
        var parser = CreateParserForVersion(version, xml);

        // Act
        var messageId = parser.MessageIdentifier;

        // Assert
        Assert.Equal(expectedVersion, messageId.Value);
    }

    [Theory]
    [InlineData("03")]
    [InlineData("08")]
    [InlineData("09")]
    [InlineData("10")]
    [InlineData("11")]
    public async Task AllParsers_ParseAsync_ShouldWorkWithIIso20022ParserInterface(string version)
    {
        // Arrange
        var xml = GetTestXmlForVersion(version);
        var parser = CreateParserForVersion(version, xml);

        // Act
        var document = await parser.ParseAsync(GetStreamFromXml(xml));

        // Assert
        Assert.NotNull(document);
        Assert.NotNull(document.Message);
    }

    private static IPain001Parser CreateParserForVersion(string version, string xml)
    {
        return version switch
        {
            "03" => Pain001v03Parser.FromString(xml),
            "08" => Pain001v08Parser.FromString(xml),
            "09" => Pain001v09Parser.FromString(xml),
            "10" => Pain001v10Parser.FromString(xml),
            "11" => Pain001v11Parser.FromString(xml),
            _ => throw new ArgumentException($"Unsupported version: {version}")
        };
    }

    private static string GetTestXmlForVersion(string version)
    {
        return version switch
        {
            "03" => TestData.GetPain001v03Xml(),
            "08" => TestData.GetPain001v08Xml(),
            "09" => TestData.GetPain001v09Xml(),
            "10" => TestData.GetPain001v10Xml(),
            "11" => TestData.GetPain001v11Xml(),
            _ => throw new ArgumentException($"Unsupported version: {version}")
        };
    }

    private static Stream GetStreamFromXml(string xml)
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
    }
}
