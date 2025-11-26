using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class StatusReasonTests
{
    [Fact]
    public void FromCode_ValidCode_ShouldCreateStatusReason()
    {
        // Act
        var reason = StatusReason.FromCode(ReasonCodes.AC01);

        // Assert
        reason.Code.ShouldBe(ReasonCodes.AC01);
        reason.Proprietary.ShouldBeNull();
        reason.IsCode.ShouldBeTrue();
        reason.IsProprietary.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromCode_EmptyCode_ShouldThrow(string? code)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => StatusReason.FromCode(code!));
    }

    [Fact]
    public void FromProprietary_ValidText_ShouldCreateStatusReason()
    {
        // Act
        var reason = StatusReason.FromProprietary("Custom reason text");

        // Assert
        reason.Proprietary.ShouldBe("Custom reason text");
        reason.Code.ShouldBeNull();
        reason.IsProprietary.ShouldBeTrue();
        reason.IsCode.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromProprietary_EmptyText_ShouldThrow(string? text)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => StatusReason.FromProprietary(text!));
    }

    [Fact]
    public void GetDescription_WithCode_ShouldReturnGermanDescription()
    {
        // Arrange
        var reason = StatusReason.FromCode(ReasonCodes.AM04);

        // Act
        var description = reason.GetDescription();

        // Assert
        description.ShouldBe("Unzureichende Deckung");
    }

    [Fact]
    public void GetDescription_WithUnknownCode_ShouldReturnCode()
    {
        // Arrange
        var reason = StatusReason.FromCode("UNKNOWN");

        // Act
        var description = reason.GetDescription();

        // Assert
        description.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void GetDescription_WithProprietary_ShouldReturnProprietaryText()
    {
        // Arrange
        var reason = StatusReason.FromProprietary("Custom reason");

        // Act
        var description = reason.GetDescription();

        // Assert
        description.ShouldBe("Custom reason");
    }

    [Fact]
    public void ToString_WithCode_ShouldFormatCorrectly()
    {
        // Arrange
        var reason = StatusReason.FromCode(ReasonCodes.AC01);

        // Act
        var result = reason.ToString();

        // Assert
        result.ShouldBe("Code: AC01");
    }

    [Fact]
    public void ToString_WithProprietary_ShouldFormatCorrectly()
    {
        // Arrange
        var reason = StatusReason.FromProprietary("Custom reason");

        // Act
        var result = reason.ToString();

        // Assert
        result.ShouldBe("Proprietary: Custom reason");
    }

    [Fact]
    public void Equality_SameCode_ShouldBeEqual()
    {
        // Arrange
        var reason1 = StatusReason.FromCode(ReasonCodes.AC01);
        var reason2 = StatusReason.FromCode(ReasonCodes.AC01);

        // Assert
        reason1.ShouldBe(reason2);
        (reason1 == reason2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentCodes_ShouldNotBeEqual()
    {
        // Arrange
        var reason1 = StatusReason.FromCode(ReasonCodes.AC01);
        var reason2 = StatusReason.FromCode(ReasonCodes.AM04);

        // Assert
        reason1.ShouldNotBe(reason2);
        (reason1 != reason2).ShouldBeTrue();
    }
}
