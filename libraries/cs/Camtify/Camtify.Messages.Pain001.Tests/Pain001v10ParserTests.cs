using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Parsers;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for Pain001v10Parser (pain.001.001.10).
/// </summary>
public sealed class Pain001v10ParserTests
{
    [Fact]
    public async Task ParseDocumentAsync_SimpleVersion10_ParsesSuccessfully()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v10_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);
        var parser = Pain001v10Parser.FromString(xml);

        // Act
        var result = await parser.ParseDocumentAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Version.ShouldBe("010");
        result.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.10");
    }

    [Fact]
    public async Task ParseDocumentAsync_SimpleVersion10_ParsesGroupHeaderCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v10_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);
        var parser = Pain001v10Parser.FromString(xml);

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

        // v10 includes LEI support
        grpHdr.InitiatingParty.Identification.ShouldNotBeNull();
        grpHdr.InitiatingParty.Identification.ShouldBeOfType<OrganisationPartyId>();
        var orgId = (OrganisationPartyId)grpHdr.InitiatingParty.Identification;
        orgId.Organisation.Lei.ShouldBe("529900T8BM49AURSDO55");
    }

    [Fact]
    public async Task ParseDocumentAsync_SimpleVersion10_ParsesPaymentInformationCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v10_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);
        var parser = Pain001v10Parser.FromString(xml);

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
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.10"">
  <CstmrCdtTrfInitn>
    <GrpHdr>
      <MsgId>TEST</MsgId>
      <CreDtTm>2023-11-27T14:30:00Z</CreDtTm>
      <NbOfTxs>0</NbOfTxs>
      <InitgPty><Nm>Test</Nm></InitgPty>
    </GrpHdr>
  </CstmrCdtTrfInitn>
</Document>";
        var parser = Pain001v10Parser.FromString(xml);

        // Act & Assert
        parser.Version.ShouldBe("010");
        parser.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.10");
    }
}
