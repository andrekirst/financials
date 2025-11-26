using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class ReasonCodeDescriptionsTests
{
    [Theory]
    [InlineData(ReasonCodes.AC01, "Kontonummer ungültig oder fehlt")]
    [InlineData(ReasonCodes.AC04, "Belastete Kontonummer ungültig oder fehlt")]
    [InlineData(ReasonCodes.AC06, "Konto geschlossen")]
    [InlineData(ReasonCodes.AM04, "Unzureichende Deckung")]
    [InlineData(ReasonCodes.AM05, "Doppelte Überweisung")]
    public void GetDescription_CommonCodes_ShouldReturnGermanText(string code, string expectedDescription)
    {
        // Act
        var description = ReasonCodeDescriptions.GetDescription(code);

        // Assert
        description.ShouldBe(expectedDescription);
    }

    [Fact]
    public void GetDescription_UnknownCode_ShouldReturnCode()
    {
        // Act
        var description = ReasonCodeDescriptions.GetDescription("UNKNOWN");

        // Assert
        description.ShouldBe("UNKNOWN");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetDescription_EmptyCode_ShouldReturnEmpty(string? code)
    {
        // Act
        var description = ReasonCodeDescriptions.GetDescription(code!);

        // Assert
        description.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(ReasonCodes.AC01)]
    [InlineData(ReasonCodes.AM04)]
    [InlineData(ReasonCodes.BE05)]
    [InlineData(ReasonCodes.DT01)]
    [InlineData(ReasonCodes.FF01)]
    [InlineData(ReasonCodes.MD01)]
    [InlineData(ReasonCodes.RR01)]
    [InlineData(ReasonCodes.TM01)]
    [InlineData(ReasonCodes.TS01)]
    [InlineData(ReasonCodes.NARR)]
    public void HasDescription_CommonCodes_ShouldReturnTrue(string code)
    {
        // Act
        var hasDescription = ReasonCodeDescriptions.HasDescription(code);

        // Assert
        hasDescription.ShouldBeTrue();
    }

    [Fact]
    public void HasDescription_UnknownCode_ShouldReturnFalse()
    {
        // Act
        var hasDescription = ReasonCodeDescriptions.HasDescription("UNKNOWN");

        // Assert
        hasDescription.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HasDescription_EmptyCode_ShouldReturnFalse(string? code)
    {
        // Act
        var hasDescription = ReasonCodeDescriptions.HasDescription(code!);

        // Assert
        hasDescription.ShouldBeFalse();
    }

    [Fact]
    public void GetAllReasonCodes_ShouldReturnAllCodes()
    {
        // Act
        var allCodes = ReasonCodeDescriptions.GetAllReasonCodes();

        // Assert
        allCodes.ShouldNotBeNull();
        allCodes.Count.ShouldBeGreaterThan(0);
        allCodes.ShouldContain(ReasonCodes.AC01);
        allCodes.ShouldContain(ReasonCodes.AM04);
        allCodes.ShouldContain(ReasonCodes.NARR);
    }

    [Fact]
    public void GetAllReasonCodes_ShouldNotBeModifiable()
    {
        // Act
        var allCodes = ReasonCodeDescriptions.GetAllReasonCodes();

        // Assert
        allCodes.ShouldNotBeNull();
        allCodes.ShouldBeAssignableTo<IReadOnlyCollection<string>>();
    }

    [Fact]
    public void GetDescription_AllDefinedCodes_ShouldHaveGermanDescription()
    {
        // Arrange
        var allCodes = new[]
        {
            ReasonCodes.AC01, ReasonCodes.AC04, ReasonCodes.AC06, ReasonCodes.AC13, ReasonCodes.AC14,
            ReasonCodes.AM01, ReasonCodes.AM02, ReasonCodes.AM04, ReasonCodes.AM05, ReasonCodes.AM09, ReasonCodes.AM10,
            ReasonCodes.BE01, ReasonCodes.BE04, ReasonCodes.BE05, ReasonCodes.BE06, ReasonCodes.BE07,
            ReasonCodes.DT01, ReasonCodes.DT02, ReasonCodes.DT03, ReasonCodes.DT04, ReasonCodes.DT05,
            ReasonCodes.FF01, ReasonCodes.FF02, ReasonCodes.FF03, ReasonCodes.FF04, ReasonCodes.FF05, ReasonCodes.FF06, ReasonCodes.FF07,
            ReasonCodes.MD01, ReasonCodes.MD02, ReasonCodes.MD03, ReasonCodes.MD06, ReasonCodes.MD07,
            ReasonCodes.RR01, ReasonCodes.RR02, ReasonCodes.RR03, ReasonCodes.RR04,
            ReasonCodes.TM01,
            ReasonCodes.TS01, ReasonCodes.TS02,
            ReasonCodes.NARR, ReasonCodes.FOCR, ReasonCodes.FRAD
        };

        // Act & Assert
        foreach (var code in allCodes)
        {
            var description = ReasonCodeDescriptions.GetDescription(code);
            description.ShouldNotBeNullOrWhiteSpace();
            description.ShouldNotBe(code); // Description should not be the same as the code
        }
    }
}
