using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class RemittanceInformationTests
{
    #region Unstructured Remittance

    [Fact]
    public void FromUnstructured_SingleLine_ShouldCreateUnstructured()
    {
        // Act
        var remittance = RemittanceInformation.FromUnstructured("Payment for Invoice 2024-001");

        // Assert
        remittance.HasUnstructured.ShouldBeTrue();
        remittance.HasStructured.ShouldBeFalse();
        remittance.Unstructured.ShouldNotBeNull();
        remittance.Unstructured!.Count.ShouldBe(1);
        remittance.Unstructured[0].ShouldBe("Payment for Invoice 2024-001");
    }

    [Fact]
    public void FromUnstructured_LongText_ShouldSplitAt140Chars()
    {
        // Arrange
        var longText = new string('A', 280); // 280 characters

        // Act
        var remittance = RemittanceInformation.FromUnstructured(longText);

        // Assert
        remittance.Unstructured.ShouldNotBeNull();
        remittance.Unstructured!.Count.ShouldBe(2);
        remittance.Unstructured[0].Length.ShouldBe(140);
        remittance.Unstructured[1].Length.ShouldBe(140);
    }

    [Fact]
    public void FromUnstructured_MultipleLines_ShouldCreateMultipleEntries()
    {
        // Act
        var remittance = RemittanceInformation.FromUnstructured(
            "Line 1: Invoice reference",
            "Line 2: Customer number",
            "Line 3: Order details");

        // Assert
        remittance.Unstructured.ShouldNotBeNull();
        remittance.Unstructured!.Count.ShouldBe(3);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromUnstructured_EmptyText_ShouldReturnEmpty(string? text)
    {
        // Act
        var remittance = RemittanceInformation.FromUnstructured(text!);

        // Assert
        remittance.HasUnstructured.ShouldBeFalse();
        remittance.HasStructured.ShouldBeFalse();
    }

    #endregion

    #region Structured Remittance

    [Fact]
    public void FromStructured_WithStructuredInfo_ShouldCreateStructured()
    {
        // Arrange
        var structured = StructuredRemittanceInformation.ForInvoice("INV-2024-001");

        // Act
        var remittance = RemittanceInformation.FromStructured(structured);

        // Assert
        remittance.HasStructured.ShouldBeTrue();
        remittance.HasUnstructured.ShouldBeFalse();
        remittance.Structured.ShouldNotBeNull();
        remittance.Structured!.Count.ShouldBe(1);
    }

    [Fact]
    public void FromRfReference_ShouldCreateStructuredWithRf()
    {
        // Act
        var remittance = RemittanceInformation.FromRfReference("RF18539007547034");

        // Assert
        remittance.HasStructured.ShouldBeTrue();
        remittance.FirstCreditorReference.ShouldBe("RF18539007547034");
    }

    [Fact]
    public void ForInvoice_WithRfReference_ShouldCreateComplete()
    {
        // Act
        var remittance = RemittanceInformation.ForInvoice(
            invoiceNumber: "2024-001",
            invoiceDate: new DateOnly(2024, 1, 15),
            rfReference: "RF18539007547034");

        // Assert
        remittance.HasStructured.ShouldBeTrue();
        remittance.FirstDocumentNumber.ShouldBe("2024-001");
        remittance.FirstCreditorReference.ShouldBe("RF18539007547034");
    }

    [Fact]
    public void ForInvoice_WithoutRfReference_ShouldCreateWithDocOnly()
    {
        // Act
        var remittance = RemittanceInformation.ForInvoice(
            invoiceNumber: "2024-001",
            invoiceDate: new DateOnly(2024, 1, 15));

        // Assert
        remittance.HasStructured.ShouldBeTrue();
        remittance.FirstDocumentNumber.ShouldBe("2024-001");
        remittance.FirstCreditorReference.ShouldBeNull();
    }

    #endregion

    #region Combined Text

    [Fact]
    public void CombinedText_UnstructuredOnly_ShouldReturnText()
    {
        // Arrange
        var remittance = RemittanceInformation.FromUnstructured(
            "Payment for services",
            "Customer: John Doe");

        // Act
        var combined = remittance.CombinedText;

        // Assert
        combined.ShouldContain("Payment for services");
        combined.ShouldContain("Customer: John Doe");
        combined.ShouldContain(" / "); // Separator
    }

    [Fact]
    public void CombinedText_StructuredWithRfReference_ShouldIncludeRef()
    {
        // Arrange
        var remittance = RemittanceInformation.FromRfReference("RF18539007547034");

        // Act
        var combined = remittance.CombinedText;

        // Assert
        combined.ShouldContain("Ref: RF18539007547034");
    }

    [Fact]
    public void CombinedText_StructuredWithDocument_ShouldIncludeDoc()
    {
        // Arrange
        var remittance = RemittanceInformation.ForInvoice("INV-2024-001");

        // Act
        var combined = remittance.CombinedText;

        // Assert
        combined.ShouldContain("Doc: INV-2024-001");
    }

    [Fact]
    public void CombinedText_MixedContent_ShouldCombineAll()
    {
        // Arrange
        var structured = StructuredRemittanceInformation.ForInvoiceWithRfReference(
            "INV-2024-001",
            "RF18539007547034");

        var remittance = new RemittanceInformation(
            unstructured: new[] { "Additional payment info" },
            structured: new[] { structured });

        // Act
        var combined = remittance.CombinedText;

        // Assert
        combined.ShouldContain("Additional payment info");
        combined.ShouldContain("Ref: RF18539007547034");
        combined.ShouldContain("Doc: INV-2024-001");
    }

    [Fact]
    public void CombinedText_Empty_ShouldReturnEmpty()
    {
        // Arrange
        var remittance = new RemittanceInformation();

        // Act
        var combined = remittance.CombinedText;

        // Assert
        combined.ShouldBe(string.Empty);
    }

    #endregion

    #region First Accessors

    [Fact]
    public void FirstCreditorReference_MultipleStructured_ShouldReturnFirst()
    {
        // Arrange
        var structured1 = new StructuredRemittanceInformation(
            creditorReferenceInformation: CreditorReferenceInformation.ForRfReference("RF18539007547034"));
        var structured2 = new StructuredRemittanceInformation(
            creditorReferenceInformation: CreditorReferenceInformation.ForRfReference("RF712348231"));

        var remittance = new RemittanceInformation(structured: new[] { structured1, structured2 });

        // Assert
        remittance.FirstCreditorReference.ShouldBe("RF18539007547034");
    }

    [Fact]
    public void FirstDocumentNumber_MultipleDocuments_ShouldReturnFirst()
    {
        // Arrange
        var doc1 = ReferredDocumentInformation.ForInvoice("INV-001");
        var doc2 = ReferredDocumentInformation.ForInvoice("INV-002");
        var structured = new StructuredRemittanceInformation(
            referredDocumentInformation: new[] { doc1, doc2 });

        var remittance = new RemittanceInformation(structured: new[] { structured });

        // Assert
        remittance.FirstDocumentNumber.ShouldBe("INV-001");
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_Unstructured_ShouldIndicateType()
    {
        // Arrange
        var remittance = RemittanceInformation.FromUnstructured("Payment info");

        // Act
        var result = remittance.ToString();

        // Assert
        result.ShouldStartWith("Unstructured:");
    }

    [Fact]
    public void ToString_Structured_ShouldIndicateType()
    {
        // Arrange
        var remittance = RemittanceInformation.FromRfReference("RF18539007547034");

        // Act
        var result = remittance.ToString();

        // Assert
        result.ShouldStartWith("Structured:");
    }

    [Fact]
    public void ToString_Mixed_ShouldIndicateMixed()
    {
        // Arrange
        var structured = StructuredRemittanceInformation.ForInvoice("INV-001");
        var remittance = new RemittanceInformation(
            unstructured: new[] { "Text" },
            structured: new[] { structured });

        // Act
        var result = remittance.ToString();

        // Assert
        result.ShouldStartWith("Mixed:");
    }

    [Fact]
    public void ToString_Empty_ShouldShowEmpty()
    {
        // Arrange
        var remittance = new RemittanceInformation();

        // Act
        var result = remittance.ToString();

        // Assert
        result.ShouldBe("Empty");
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void RealWorld_SepaPaymentWithRfReference_ShouldWork()
    {
        // Arrange - typical SEPA payment with structured reference
        var remittance = RemittanceInformation.ForInvoice(
            invoiceNumber: "2024-INV-00123",
            invoiceDate: new DateOnly(2024, 6, 15),
            rfReference: "RF18539007547034");

        // Assert
        remittance.HasStructured.ShouldBeTrue();
        remittance.CombinedText.ShouldContain("RF18539007547034");
        remittance.CombinedText.ShouldContain("2024-INV-00123");

        // The RF reference should validate
        var creditorRef = remittance.Structured![0].CreditorReferenceInformation;
        creditorRef.ShouldNotBeNull();
        creditorRef.Value.ValidateRfReference().ShouldBeTrue();
    }

    [Fact]
    public void RealWorld_UnstructuredPaymentPurpose_ShouldWork()
    {
        // Arrange - simple unstructured payment purpose
        var remittance = RemittanceInformation.FromUnstructured(
            "Rent payment for June 2024, Apartment 3B, Contract #12345");

        // Assert
        remittance.HasUnstructured.ShouldBeTrue();
        remittance.CombinedText.ShouldContain("Rent payment");
        remittance.CombinedText.ShouldContain("Contract #12345");
    }

    #endregion
}
