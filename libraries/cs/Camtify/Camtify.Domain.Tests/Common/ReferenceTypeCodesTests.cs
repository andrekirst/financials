using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class ReferenceTypeCodesTests
{
    [Theory]
    [InlineData(ReferenceTypeCodes.SCOR, "Structured Communication Reference (RF)")]
    [InlineData(ReferenceTypeCodes.RADM, "Remittance Advice Reference")]
    [InlineData(ReferenceTypeCodes.RPIN, "Related Payment Instruction Reference")]
    [InlineData(ReferenceTypeCodes.FXDR, "Foreign Exchange Deal Reference")]
    public void GetDescription_KnownCode_ShouldReturnDescription(string code, string expectedDescription)
    {
        // Act
        var description = ReferenceTypeCodes.GetDescription(code);

        // Assert
        description.ShouldBe(expectedDescription);
    }

    [Fact]
    public void GetDescription_UnknownCode_ShouldReturnCode()
    {
        // Act
        var description = ReferenceTypeCodes.GetDescription("UNKNOWN");

        // Assert
        description.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void GetDescription_NullCode_ShouldReturnEmpty()
    {
        // Act
        var description = ReferenceTypeCodes.GetDescription(null);

        // Assert
        description.ShouldBe(string.Empty);
    }

    [Fact]
    public void All_ShouldContainAllCodes()
    {
        // Assert
        ReferenceTypeCodes.All.ShouldContain(ReferenceTypeCodes.SCOR);
        ReferenceTypeCodes.All.ShouldContain(ReferenceTypeCodes.RADM);
        ReferenceTypeCodes.All.ShouldContain(ReferenceTypeCodes.RPIN);
        ReferenceTypeCodes.All.ShouldContain(ReferenceTypeCodes.FXDR);
    }

    [Fact]
    public void SCOR_ShouldBeForRfReference()
    {
        // Assert - SCOR is the code for structured creditor reference (RF)
        ReferenceTypeCodes.SCOR.ShouldBe("SCOR");
    }
}
