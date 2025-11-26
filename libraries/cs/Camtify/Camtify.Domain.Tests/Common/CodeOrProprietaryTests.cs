using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class CodeOrProprietaryTests
{
    [Fact]
    public void FromCode_ValidCode_ShouldCreateWithCode()
    {
        // Act
        var result = CodeOrProprietary.FromCode("CINV");

        // Assert
        result.Code.ShouldBe("CINV");
        result.Proprietary.ShouldBeNull();
        result.IsCode.ShouldBeTrue();
        result.IsProprietary.ShouldBeFalse();
        result.Value.ShouldBe("CINV");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromCode_EmptyCode_ShouldThrow(string? code)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CodeOrProprietary.FromCode(code!));
    }

    [Fact]
    public void FromProprietary_ValidValue_ShouldCreateWithProprietary()
    {
        // Act
        var result = CodeOrProprietary.FromProprietary("CustomType");

        // Assert
        result.Proprietary.ShouldBe("CustomType");
        result.Code.ShouldBeNull();
        result.IsProprietary.ShouldBeTrue();
        result.IsCode.ShouldBeFalse();
        result.Value.ShouldBe("CustomType");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromProprietary_EmptyValue_ShouldThrow(string? value)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CodeOrProprietary.FromProprietary(value!));
    }

    [Fact]
    public void Value_WithCode_ShouldPreferCode()
    {
        // Arrange
        var codeResult = CodeOrProprietary.FromCode("CODE");

        // Assert
        codeResult.Value.ShouldBe("CODE");
    }

    [Fact]
    public void ToString_WithCode_ShouldFormatCorrectly()
    {
        // Arrange
        var result = CodeOrProprietary.FromCode("CINV");

        // Act
        var str = result.ToString();

        // Assert
        str.ShouldBe("Code: CINV");
    }

    [Fact]
    public void ToString_WithProprietary_ShouldFormatCorrectly()
    {
        // Arrange
        var result = CodeOrProprietary.FromProprietary("CustomType");

        // Act
        var str = result.ToString();

        // Assert
        str.ShouldBe("Proprietary: CustomType");
    }

    [Fact]
    public void Equality_SameCode_ShouldBeEqual()
    {
        // Arrange
        var result1 = CodeOrProprietary.FromCode("CINV");
        var result2 = CodeOrProprietary.FromCode("CINV");

        // Assert
        result1.ShouldBe(result2);
        (result1 == result2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentCodes_ShouldNotBeEqual()
    {
        // Arrange
        var result1 = CodeOrProprietary.FromCode("CINV");
        var result2 = CodeOrProprietary.FromCode("CREN");

        // Assert
        result1.ShouldNotBe(result2);
        (result1 != result2).ShouldBeTrue();
    }
}
