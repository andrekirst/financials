using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class StructuredRemittanceInformationTests
{
    [Fact]
    public void ForInvoice_ShouldCreateWithDocument()
    {
        // Act
        var structured = StructuredRemittanceInformation.ForInvoice("INV-2024-001");

        // Assert
        structured.ReferredDocumentInformation.ShouldNotBeNull();
        structured.ReferredDocumentInformation!.Count.ShouldBe(1);
        structured.PrimaryDocumentNumber.ShouldBe("INV-2024-001");
    }

    [Fact]
    public void ForInvoice_WithDateAndAmount_ShouldIncludeAll()
    {
        // Arrange
        var invoiceDate = new DateOnly(2024, 6, 15);
        var amount = new Money(1234.56m, CurrencyCode.EUR);

        // Act
        var structured = StructuredRemittanceInformation.ForInvoice(
            "INV-2024-001",
            invoiceDate,
            amount);

        // Assert
        structured.ReferredDocumentInformation![0].RelatedDate.ShouldBe(invoiceDate);
        structured.ReferredDocumentAmount.ShouldNotBeNull();
        structured.ReferredDocumentAmount.Value.DuePayableAmount.ShouldBe(amount);
    }

    [Fact]
    public void ForInvoiceWithRfReference_ShouldIncludeBoth()
    {
        // Act
        var structured = StructuredRemittanceInformation.ForInvoiceWithRfReference(
            "INV-2024-001",
            "RF18539007547034");

        // Assert
        structured.PrimaryDocumentNumber.ShouldBe("INV-2024-001");
        structured.CreditorReference.ShouldBe("RF18539007547034");
        structured.CreditorReferenceInformation.ShouldNotBeNull();
        structured.CreditorReferenceInformation.Value.ValidateRfReference().ShouldBeTrue();
    }

    [Fact]
    public void Constructor_AllParameters_ShouldSetProperties()
    {
        // Arrange
        var docs = new[] { ReferredDocumentInformation.ForInvoice("INV-001") };
        var amount = RemittanceAmount.ForDueAmount(new Money(100m, CurrencyCode.EUR));
        var creditorRef = CreditorReferenceInformation.ForRfReference("RF18539007547034");
        var additionalInfo = new[] { "Line 1", "Line 2" };

        // Act
        var structured = new StructuredRemittanceInformation(
            referredDocumentInformation: docs,
            referredDocumentAmount: amount,
            creditorReferenceInformation: creditorRef,
            additionalRemittanceInformation: additionalInfo);

        // Assert
        structured.ReferredDocumentInformation.ShouldBe(docs);
        structured.ReferredDocumentAmount.ShouldBe(amount);
        structured.CreditorReferenceInformation.ShouldBe(creditorRef);
        structured.AdditionalRemittanceInformation.ShouldBe(additionalInfo);
    }

    [Fact]
    public void PrimaryDocumentNumber_NoDocuments_ShouldReturnNull()
    {
        // Arrange
        var structured = new StructuredRemittanceInformation();

        // Assert
        structured.PrimaryDocumentNumber.ShouldBeNull();
    }

    [Fact]
    public void CreditorReference_NoReference_ShouldReturnNull()
    {
        // Arrange
        var structured = StructuredRemittanceInformation.ForInvoice("INV-001");

        // Assert
        structured.CreditorReference.ShouldBeNull();
    }

    [Fact]
    public void ToString_WithRefAndDoc_ShouldFormatCorrectly()
    {
        // Arrange
        var structured = StructuredRemittanceInformation.ForInvoiceWithRfReference(
            "INV-2024-001",
            "RF18539007547034");

        // Act
        var result = structured.ToString();

        // Assert
        result.ShouldContain("Ref: RF18539007547034");
        result.ShouldContain("Doc: INV-2024-001");
    }

    [Fact]
    public void ToString_WithAmount_ShouldIncludeAmount()
    {
        // Arrange
        var amount = new Money(1234.56m, CurrencyCode.EUR);
        var structured = StructuredRemittanceInformation.ForInvoice("INV-001", amount: amount);

        // Act
        var result = structured.ToString();

        // Assert
        result.ShouldContain("Amount:");
        result.ShouldContain("EUR");
    }

    [Fact]
    public void ToString_Empty_ShouldReturnEmpty()
    {
        // Arrange
        var structured = new StructuredRemittanceInformation();

        // Act
        var result = structured.ToString();

        // Assert
        result.ShouldBe("Empty");
    }
}

public class RemittanceAmountTests
{
    [Fact]
    public void ForDueAmount_ShouldCreateWithDuePayable()
    {
        // Arrange
        var amount = new Money(1000m, CurrencyCode.EUR);

        // Act
        var remittanceAmount = RemittanceAmount.ForDueAmount(amount);

        // Assert
        remittanceAmount.DuePayableAmount.ShouldBe(amount);
        remittanceAmount.RemittedAmount.ShouldBeNull();
    }

    [Fact]
    public void ForPayment_ShouldCreateWithBothAmounts()
    {
        // Arrange
        var due = new Money(1000m, CurrencyCode.EUR);
        var remitted = new Money(950m, CurrencyCode.EUR);

        // Act
        var remittanceAmount = RemittanceAmount.ForPayment(due, remitted);

        // Assert
        remittanceAmount.DuePayableAmount.ShouldBe(due);
        remittanceAmount.RemittedAmount.ShouldBe(remitted);
    }

    [Fact]
    public void Constructor_AllParameters_ShouldSetProperties()
    {
        // Arrange
        var dueAmount = new Money(1000m, CurrencyCode.EUR);
        var discountAmount = new DiscountAmountAndType(new Money(50m, CurrencyCode.EUR));
        var creditNote = new Money(100m, CurrencyCode.EUR);
        var taxAmount = new TaxAmountAndType(new Money(190m, CurrencyCode.EUR));
        var remittedAmount = new Money(850m, CurrencyCode.EUR);

        // Act
        var remittance = new RemittanceAmount(
            duePayableAmount: dueAmount,
            discountAppliedAmount: discountAmount,
            creditNoteAmount: creditNote,
            taxAmount: taxAmount,
            remittedAmount: remittedAmount);

        // Assert
        remittance.DuePayableAmount.ShouldBe(dueAmount);
        remittance.DiscountAppliedAmount.ShouldBe(discountAmount);
        remittance.CreditNoteAmount.ShouldBe(creditNote);
        remittance.TaxAmount.ShouldBe(taxAmount);
        remittance.RemittedAmount.ShouldBe(remittedAmount);
    }

    [Fact]
    public void ToString_WithDueAndRemitted_ShouldFormatCorrectly()
    {
        // Arrange
        var due = new Money(1000m, CurrencyCode.EUR);
        var remitted = new Money(950m, CurrencyCode.EUR);
        var remittanceAmount = RemittanceAmount.ForPayment(due, remitted);

        // Act
        var result = remittanceAmount.ToString();

        // Assert
        result.ShouldContain("Due:");
        result.ShouldContain("Remitted:");
    }

    [Fact]
    public void ToString_Empty_ShouldReturnEmpty()
    {
        // Arrange
        var remittanceAmount = new RemittanceAmount();

        // Act
        var result = remittanceAmount.ToString();

        // Assert
        result.ShouldBe("Empty");
    }
}
