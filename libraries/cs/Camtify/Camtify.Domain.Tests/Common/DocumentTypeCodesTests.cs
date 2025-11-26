using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class DocumentTypeCodesTests
{
    [Theory]
    [InlineData(DocumentTypeCodes.CINV, "Commercial Invoice")]
    [InlineData(DocumentTypeCodes.CREN, "Credit Note")]
    [InlineData(DocumentTypeCodes.DEBN, "Debit Note")]
    [InlineData(DocumentTypeCodes.DISP, "Dispatch Advice")]
    [InlineData(DocumentTypeCodes.DNFA, "Debit Note Financial Adjustment")]
    [InlineData(DocumentTypeCodes.HIRI, "Hire Invoice")]
    [InlineData(DocumentTypeCodes.MSIN, "Metered Service Invoice")]
    [InlineData(DocumentTypeCodes.RADM, "Remittance Advice")]
    [InlineData(DocumentTypeCodes.SOAC, "Statement of Account")]
    public void GetDescription_KnownCode_ShouldReturnDescription(string code, string expectedDescription)
    {
        // Act
        var description = DocumentTypeCodes.GetDescription(code);

        // Assert
        description.ShouldBe(expectedDescription);
    }

    [Fact]
    public void GetDescription_UnknownCode_ShouldReturnCode()
    {
        // Act
        var description = DocumentTypeCodes.GetDescription("UNKNOWN");

        // Assert
        description.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void GetDescription_NullCode_ShouldReturnEmpty()
    {
        // Act
        var description = DocumentTypeCodes.GetDescription(null);

        // Assert
        description.ShouldBe(string.Empty);
    }

    [Fact]
    public void All_ShouldContainAllCodes()
    {
        // Assert
        DocumentTypeCodes.All.ShouldContain(DocumentTypeCodes.CINV);
        DocumentTypeCodes.All.ShouldContain(DocumentTypeCodes.CREN);
        DocumentTypeCodes.All.ShouldContain(DocumentTypeCodes.DEBN);
        DocumentTypeCodes.All.ShouldContain(DocumentTypeCodes.SOAC);
        DocumentTypeCodes.All.Count.ShouldBeGreaterThanOrEqualTo(9);
    }

    [Fact]
    public void Constants_ShouldHaveFourCharacters()
    {
        // Assert
        DocumentTypeCodes.CINV.Length.ShouldBe(4);
        DocumentTypeCodes.CREN.Length.ShouldBe(4);
        DocumentTypeCodes.DEBN.Length.ShouldBe(4);
        DocumentTypeCodes.SOAC.Length.ShouldBe(4);
    }
}
