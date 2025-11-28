using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Parsers;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for Pain001Parser.
/// </summary>
public sealed class Pain001ParserTests
{

    [Fact]
    public async Task ParseAsync_SimpleVersion09_ParsesSuccessfully()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v09_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);

        // Act
        var result = await Pain001Parser.ParseAsync(xml);

        // Assert
        result.ShouldNotBeNull();
        result.Version.ShouldBe("009");
        result.Namespace.ShouldBe("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
    }

    [Fact]
    public async Task ParseAsync_SimpleVersion09_ParsesGroupHeaderCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v09_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);

        // Act
        var result = await Pain001Parser.ParseAsync(xml);

        // Assert
        var grpHdr = result.CreditTransferInitiation.GroupHeader;
        grpHdr.ShouldNotBeNull();
        grpHdr.MessageIdentification.ShouldBe("MSG-20231127-001");
        grpHdr.CreationDateTime.Year.ShouldBe(2023);
        grpHdr.CreationDateTime.Month.ShouldBe(11);
        grpHdr.CreationDateTime.Day.ShouldBe(27);
        grpHdr.NumberOfTransactions.ShouldBe(2);
        grpHdr.ControlSum.ShouldBe(1500.00m);

        grpHdr.InitiatingParty.ShouldNotBeNull();
        grpHdr.InitiatingParty.Name.ShouldBe("Acme Corporation");
        grpHdr.InitiatingParty.PostalAddress.ShouldNotBeNull();
        grpHdr.InitiatingParty.PostalAddress.Country.ShouldBe("DE");
        grpHdr.InitiatingParty.PostalAddress.AddressLines.ShouldNotBeNull();
        grpHdr.InitiatingParty.PostalAddress.AddressLines.Count.ShouldBe(2);

        grpHdr.InitiatingParty.Identification.ShouldNotBeNull();
        grpHdr.InitiatingParty.Identification.ShouldBeOfType<OrganisationPartyId>();

        var orgId = (OrganisationPartyId)grpHdr.InitiatingParty.Identification;
        orgId.Organisation.ShouldNotBeNull();
        orgId.Organisation.Lei.ShouldBe("529900T8BM49AURSDO55");
    }

    [Fact]
    public async Task ParseAsync_SimpleVersion09_ParsesPaymentInformationCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v09_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);

        // Act
        var result = await Pain001Parser.ParseAsync(xml);

        // Assert
        var pmtInf = result.CreditTransferInitiation.PaymentInformation;
        pmtInf.ShouldNotBeNull();
        pmtInf.Count.ShouldBe(1);

        var pmt = pmtInf[0];
        pmt.PaymentInformationIdentification.ShouldBe("PMT-20231127-001");
        pmt.PaymentMethod.ShouldBe("TRF");
        pmt.BatchBooking.ShouldBe(true);
        pmt.NumberOfTransactions.ShouldBe(2);
        pmt.ControlSum.ShouldBe(1500.00m);

        pmt.PaymentTypeInformation.ShouldNotBeNull();
        pmt.PaymentTypeInformation.ServiceLevelCode.ShouldBe("SEPA");
        pmt.PaymentTypeInformation.LocalInstrumentCode.ShouldBe("CORE");

        pmt.RequestedExecutionDate.Year.ShouldBe(2023);
        pmt.RequestedExecutionDate.Month.ShouldBe(11);
        pmt.RequestedExecutionDate.Day.ShouldBe(28);

        pmt.Debtor.ShouldNotBeNull();
        pmt.Debtor.Name.ShouldBe("Acme Corporation");

        pmt.DebtorAccount.ShouldNotBeNull();
        pmt.DebtorAccount.Identification.ShouldBeOfType<IbanAccountIdentification>();
        var debtorIban = (IbanAccountIdentification)pmt.DebtorAccount.Identification;
        debtorIban.Iban.ToString().ShouldBe("DE68210501700012345678");
        pmt.DebtorAccount.Currency.ShouldNotBeNull();
        pmt.DebtorAccount.Currency.Value.Code.ShouldBe("EUR");

        pmt.DebtorAgent.ShouldNotBeNull();
        pmt.DebtorAgent.FinancialInstitutionIdentification.ShouldNotBeNull();
        pmt.DebtorAgent.FinancialInstitutionIdentification.Bic.ShouldBe("COBADEFFXXX");
        pmt.DebtorAgent.FinancialInstitutionIdentification.Name.ShouldBe("Commerzbank AG");

        pmt.ChargeBearer.ShouldBe(ChargeBearer.FollowingServiceLevel);
    }

    [Fact]
    public async Task ParseAsync_SimpleVersion09_ParsesCreditTransferTransactionInformationCorrectly()
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", "pain001_v09_simple.xml");
        var xml = await File.ReadAllTextAsync(xmlPath);

        // Act
        var result = await Pain001Parser.ParseAsync(xml);

        // Assert
        var pmtInf = result.CreditTransferInitiation.PaymentInformation[0];
        var txs = pmtInf.CreditTransferTransactionInformation;
        txs.ShouldNotBeNull();
        txs.Count.ShouldBe(2);

        // First transaction
        var tx1 = txs[0];
        tx1.PaymentIdentification.ShouldNotBeNull();
        tx1.PaymentIdentification.InstructionIdentification.ShouldBe("INSTR-001");
        tx1.PaymentIdentification.EndToEndIdentification.ShouldBe("E2E-001");

        tx1.InstructedAmount.Amount.ShouldBe(1000.00m);
        tx1.InstructedAmount.Currency.Code.ShouldBe("EUR");

        tx1.CreditorAgent.ShouldNotBeNull();
        tx1.CreditorAgent.FinancialInstitutionIdentification.Bic.ShouldBe("DEUTDEFFXXX");
        tx1.CreditorAgent.FinancialInstitutionIdentification.Name.ShouldBe("Deutsche Bank AG");

        tx1.Creditor.ShouldNotBeNull();
        tx1.Creditor.Name.ShouldBe("Supplier Company GmbH");

        tx1.CreditorAccount.ShouldNotBeNull();
        tx1.CreditorAccount.Identification.ShouldBeOfType<IbanAccountIdentification>();
        var creditorIban = (IbanAccountIdentification)tx1.CreditorAccount.Identification;
        creditorIban.Iban.ToString().ShouldBe("DE02100100100006820101");

        tx1.RemittanceInformation.ShouldNotBeNull();
        tx1.RemittanceInformation.Value.HasUnstructured.ShouldBeTrue();
        tx1.RemittanceInformation.Value.Unstructured.ShouldNotBeNull();
        tx1.RemittanceInformation.Value.Unstructured[0].ShouldBe("Invoice INV-2023-1001");

        // Second transaction
        var tx2 = txs[1];
        tx2.PaymentIdentification.InstructionIdentification.ShouldBe("INSTR-002");
        tx2.PaymentIdentification.EndToEndIdentification.ShouldBe("E2E-002");

        tx2.InstructedAmount.Amount.ShouldBe(500.00m);
        tx2.InstructedAmount.Currency.Code.ShouldBe("EUR");

        tx2.RemittanceInformation.ShouldNotBeNull();
        tx2.RemittanceInformation.Value.HasStructured.ShouldBeTrue();
        tx2.RemittanceInformation.Value.Structured.ShouldNotBeNull();
        var structuredRmtInf = tx2.RemittanceInformation.Value.Structured[0];
        structuredRmtInf.CreditorReferenceInformation.ShouldNotBeNull();
        structuredRmtInf.CreditorReferenceInformation.Value.Reference.ShouldBe("RF18539007547034");
    }

    [Fact]
    public async Task ParseAsync_UnsupportedVersion_ThrowsArgumentException()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.02"">
  <CstmrCdtTrfInitn>
    <GrpHdr>
      <MsgId>TEST</MsgId>
      <CreDtTm>2023-11-27T14:30:00Z</CreDtTm>
      <NbOfTxs>0</NbOfTxs>
      <InitgPty><Nm>Test</Nm></InitgPty>
    </GrpHdr>
  </CstmrCdtTrfInitn>
</Document>";

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await Pain001Parser.ParseAsync(xml));
    }

    [Fact]
    public async Task ParseAsync_NullXml_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await Pain001Parser.ParseAsync((string)null!));
    }

    [Fact]
    public async Task ParseAsync_EmptyXml_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await Pain001Parser.ParseAsync(string.Empty));
    }
}
