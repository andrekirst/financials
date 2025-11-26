using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class StatusReasonInformationTests
{
    [Fact]
    public void Constructor_Default_ShouldCreateEmptyInstance()
    {
        // Act
        var info = new StatusReasonInformation();

        // Assert
        info.Reason.ShouldBeNull();
        info.AdditionalInformation.ShouldBeNull();
        info.Originator.ShouldBeNull();
        info.HasReason.ShouldBeFalse();
        info.HasAdditionalInformation.ShouldBeFalse();
        info.HasOriginator.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WithReason_ShouldSetReason()
    {
        // Arrange
        var reason = StatusReason.FromCode(ReasonCodes.AC01);

        // Act
        var info = new StatusReasonInformation(reason: reason);

        // Assert
        info.Reason.ShouldNotBeNull();
        info.Reason.Value.Code.ShouldBe(ReasonCodes.AC01);
        info.HasReason.ShouldBeTrue();
    }

    [Fact]
    public void Constructor_WithAdditionalInfo_ShouldSetAdditionalInfo()
    {
        // Arrange
        var additionalInfo = new[] { "Info 1", "Info 2" };

        // Act
        var info = new StatusReasonInformation(additionalInformation: additionalInfo);

        // Assert
        info.AdditionalInformation.ShouldNotBeNull();
        info.AdditionalInformation.Count.ShouldBe(2);
        info.AdditionalInformation[0].ShouldBe("Info 1");
        info.AdditionalInformation[1].ShouldBe("Info 2");
        info.HasAdditionalInformation.ShouldBeTrue();
    }

    [Fact]
    public void FromCode_ShouldCreateWithReasonCode()
    {
        // Act
        var info = StatusReasonInformation.FromCode(ReasonCodes.AM04);

        // Assert
        info.Reason.ShouldNotBeNull();
        info.Reason.Value.Code.ShouldBe(ReasonCodes.AM04);
        info.HasReason.ShouldBeTrue();
        info.HasAdditionalInformation.ShouldBeFalse();
    }

    [Fact]
    public void FromCodeWithInfo_ShouldCreateWithReasonAndInfo()
    {
        // Act
        var info = StatusReasonInformation.FromCodeWithInfo(
            ReasonCodes.AM04,
            "Additional info 1",
            "Additional info 2");

        // Assert
        info.Reason.ShouldNotBeNull();
        info.Reason.Value.Code.ShouldBe(ReasonCodes.AM04);
        info.AdditionalInformation.ShouldNotBeNull();
        info.AdditionalInformation.Count.ShouldBe(2);
        info.HasReason.ShouldBeTrue();
        info.HasAdditionalInformation.ShouldBeTrue();
    }

    [Fact]
    public void FromCodeWithInfo_NoAdditionalInfo_ShouldWork()
    {
        // Act
        var info = StatusReasonInformation.FromCodeWithInfo(ReasonCodes.AC01);

        // Assert
        info.Reason.ShouldNotBeNull();
        info.AdditionalInformation.ShouldNotBeNull();
        info.AdditionalInformation.Count.ShouldBe(0);
    }

    [Fact]
    public void ToString_Empty_ShouldReturnEmpty()
    {
        // Arrange
        var info = new StatusReasonInformation();

        // Act
        var result = info.ToString();

        // Assert
        result.ShouldBe("Empty");
    }

    [Fact]
    public void ToString_WithReason_ShouldFormatCorrectly()
    {
        // Arrange
        var reason = StatusReason.FromCode(ReasonCodes.AC01);
        var info = new StatusReasonInformation(reason: reason);

        // Act
        var result = info.ToString();

        // Assert
        result.ShouldContain("Reason:");
        result.ShouldContain("AC01");
    }

    [Fact]
    public void ToString_WithAdditionalInfo_ShouldFormatCorrectly()
    {
        // Arrange
        var info = StatusReasonInformation.FromCodeWithInfo(
            ReasonCodes.AM04,
            "Info 1",
            "Info 2");

        // Act
        var result = info.ToString();

        // Assert
        result.ShouldContain("Reason:");
        result.ShouldContain("Info:");
        result.ShouldContain("Info 1");
        result.ShouldContain("Info 2");
    }

    [Fact]
    public void HasAdditionalInformation_EmptyList_ShouldReturnFalse()
    {
        // Arrange
        var info = new StatusReasonInformation(additionalInformation: Array.Empty<string>());

        // Act & Assert
        info.HasAdditionalInformation.ShouldBeFalse();
    }

    [Fact]
    public void Equality_SameContent_ShouldBeEqual()
    {
        // Arrange
        var info1 = StatusReasonInformation.FromCode(ReasonCodes.AC01);
        var info2 = StatusReasonInformation.FromCode(ReasonCodes.AC01);

        // Assert
        info1.ShouldBe(info2);
    }

    [Fact]
    public void Equality_DifferentContent_ShouldNotBeEqual()
    {
        // Arrange
        var info1 = StatusReasonInformation.FromCode(ReasonCodes.AC01);
        var info2 = StatusReasonInformation.FromCode(ReasonCodes.AM04);

        // Assert
        info1.ShouldNotBe(info2);
    }
}
