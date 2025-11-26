using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class ReferredDocumentInformationTests
{
    [Fact]
    public void ForInvoice_ShouldCreateWithCINVType()
    {
        // Act
        var docInfo = ReferredDocumentInformation.ForInvoice("INV-2024-001");

        // Assert
        docInfo.Number.ShouldBe("INV-2024-001");
        docInfo.Type.ShouldNotBeNull();
        docInfo.Type.Value.TypeCode.ShouldBe(DocumentTypeCodes.CINV);
    }

    [Fact]
    public void ForInvoice_WithDate_ShouldIncludeDate()
    {
        // Arrange
        var invoiceDate = new DateOnly(2024, 6, 15);

        // Act
        var docInfo = ReferredDocumentInformation.ForInvoice("INV-2024-001", invoiceDate);

        // Assert
        docInfo.RelatedDate.ShouldBe(invoiceDate);
    }

    [Fact]
    public void ForCreditNote_ShouldCreateWithCRENType()
    {
        // Act
        var docInfo = ReferredDocumentInformation.ForCreditNote("CN-2024-001");

        // Assert
        docInfo.Number.ShouldBe("CN-2024-001");
        docInfo.Type.ShouldNotBeNull();
        docInfo.Type.Value.TypeCode.ShouldBe(DocumentTypeCodes.CREN);
    }

    [Fact]
    public void Constructor_AllParameters_ShouldSetProperties()
    {
        // Arrange
        var docType = ReferredDocumentType.FromCode(DocumentTypeCodes.DEBN);
        var lineDetails = new[] { new DocumentLineInformation(description: "Line 1") };

        // Act
        var docInfo = new ReferredDocumentInformation(
            type: docType,
            number: "DN-001",
            relatedDate: new DateOnly(2024, 1, 1),
            lineDetails: lineDetails);

        // Assert
        docInfo.Type.ShouldBe(docType);
        docInfo.Number.ShouldBe("DN-001");
        docInfo.RelatedDate.ShouldBe(new DateOnly(2024, 1, 1));
        docInfo.LineDetails.ShouldNotBeNull();
        docInfo.LineDetails!.Count.ShouldBe(1);
    }

    [Fact]
    public void ToString_WithAllInfo_ShouldFormatCorrectly()
    {
        // Arrange
        var docInfo = ReferredDocumentInformation.ForInvoice(
            "INV-2024-001",
            new DateOnly(2024, 6, 15));

        // Act
        var result = docInfo.ToString();

        // Assert
        result.ShouldContain("Commercial Invoice");
        result.ShouldContain("INV-2024-001");
        result.ShouldContain("2024-06-15");
    }

    [Fact]
    public void ToString_Empty_ShouldReturnEmpty()
    {
        // Arrange
        var docInfo = new ReferredDocumentInformation();

        // Act
        var result = docInfo.ToString();

        // Assert
        result.ShouldBe("Empty");
    }
}

public class ReferredDocumentTypeTests
{
    [Fact]
    public void FromCode_ValidCode_ShouldCreate()
    {
        // Act
        var docType = ReferredDocumentType.FromCode(DocumentTypeCodes.CINV);

        // Assert
        docType.TypeCode.ShouldBe(DocumentTypeCodes.CINV);
        docType.CodeOrProprietary.ShouldNotBeNull();
        docType.CodeOrProprietary.Value.IsCode.ShouldBeTrue();
    }

    [Fact]
    public void FromCode_WithIssuer_ShouldIncludeIssuer()
    {
        // Act
        var docType = ReferredDocumentType.FromCode(DocumentTypeCodes.CINV, "MYBANK");

        // Assert
        docType.Issuer.ShouldBe("MYBANK");
    }

    [Fact]
    public void FromProprietary_ShouldCreateProprietary()
    {
        // Act
        var docType = ReferredDocumentType.FromProprietary("CUSTOMTYPE");

        // Assert
        docType.TypeCode.ShouldBe("CUSTOMTYPE");
        docType.CodeOrProprietary.ShouldNotBeNull();
        docType.CodeOrProprietary.Value.IsProprietary.ShouldBeTrue();
    }

    [Fact]
    public void GetDescription_StandardCode_ShouldReturnDescription()
    {
        // Arrange
        var docType = ReferredDocumentType.FromCode(DocumentTypeCodes.CINV);

        // Act
        var description = docType.GetDescription();

        // Assert
        description.ShouldBe("Commercial Invoice");
    }

    [Fact]
    public void ToString_WithIssuer_ShouldIncludeIssuer()
    {
        // Arrange
        var docType = ReferredDocumentType.FromCode(DocumentTypeCodes.CINV, "MYBANK");

        // Act
        var result = docType.ToString();

        // Assert
        result.ShouldContain("Commercial Invoice");
        result.ShouldContain("MYBANK");
    }
}

public class DocumentLineInformationTests
{
    [Fact]
    public void Constructor_AllParameters_ShouldSetProperties()
    {
        // Arrange
        var amount = new Money(100.00m, CurrencyCode.EUR);

        // Act
        var lineInfo = new DocumentLineInformation(
            identification: new[] { "LINE001", "LINE002" },
            description: "Product description",
            amount: amount);

        // Assert
        lineInfo.Identification.ShouldNotBeNull();
        lineInfo.Identification!.Count.ShouldBe(2);
        lineInfo.Description.ShouldBe("Product description");
        lineInfo.Amount.ShouldBe(amount);
    }

    [Fact]
    public void ToString_WithAllInfo_ShouldFormatCorrectly()
    {
        // Arrange
        var amount = new Money(100.00m, CurrencyCode.EUR);
        var lineInfo = new DocumentLineInformation(
            identification: new[] { "LINE001" },
            description: "Test item",
            amount: amount);

        // Act
        var result = lineInfo.ToString();

        // Assert
        result.ShouldContain("LINE001");
        result.ShouldContain("Test item");
        result.ShouldContain("EUR");
    }

    [Fact]
    public void ToString_Empty_ShouldReturnEmpty()
    {
        // Arrange
        var lineInfo = new DocumentLineInformation();

        // Act
        var result = lineInfo.ToString();

        // Assert
        result.ShouldBe("Empty");
    }
}
