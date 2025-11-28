using Camtify.Messages.Pain001.Parsers;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for Pain001v03Parser (pain.001.001.03 - SEPA baseline).
/// </summary>
public sealed class Pain001v03ParserTests
{
    [Fact]
    public async Task ParseDocumentAsync_SimpleVersion03_ParsesSuccessfully()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v03_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);
        var parser = Pain001v03Parser.FromString(xml);

        // Act
        var result = await parser.ParseDocumentAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Version.ShouldBe("003");
        result.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.03");
    }

    [Fact]
    public async Task ParseDocumentAsync_SimpleVersion03_ParsesGroupHeaderCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v03_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);
        var parser = Pain001v03Parser.FromString(xml);

        // Act
        var result = await parser.ParseDocumentAsync();

        // Assert
        var grpHdr = result.Message.GroupHeader;
        grpHdr.ShouldNotBeNull();
        grpHdr.MessageIdentification.ShouldBe("MSG-20231127-001");
        grpHdr.NumberOfTransactions.ShouldBe(2);
        grpHdr.ControlSum.ShouldBe(1500.00m);
        grpHdr.InitiatingParty.ShouldNotBeNull();
        grpHdr.InitiatingParty.Name.ShouldBe("Acme Corporation");
    }

    [Fact]
    public async Task ParseDocumentAsync_SimpleVersion03_ParsesPaymentInformationCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v03_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);
        var parser = Pain001v03Parser.FromString(xml);

        // Act
        var result = await parser.ParseDocumentAsync();

        // Assert
        var pmtInf = result.Message.PaymentInformation;
        pmtInf.ShouldNotBeNull();
        pmtInf.Count.ShouldBe(1);
        pmtInf[0].PaymentInformationIdentification.ShouldBe("PMT-20231127-001");
        pmtInf[0].CreditTransferTransactionInformation.Count.ShouldBe(2);
    }

    [Fact]
    public void Version_ReturnsCorrectValue()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.03"">
  <CstmrCdtTrfInitn>
    <GrpHdr>
      <MsgId>TEST</MsgId>
      <CreDtTm>2023-11-27T14:30:00Z</CreDtTm>
      <NbOfTxs>0</NbOfTxs>
      <InitgPty><Nm>Test</Nm></InitgPty>
    </GrpHdr>
  </CstmrCdtTrfInitn>
</Document>";
        var parser = Pain001v03Parser.FromString(xml);

        // Act & Assert
        parser.Version.ShouldBe("003");
        parser.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.03");
    }
}
