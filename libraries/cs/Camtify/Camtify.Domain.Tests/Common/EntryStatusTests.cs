using Camtify.Domain.Common;
using Camtify.Infrastructure.Extensions;

namespace Camtify.Domain.Tests.Common;

public class EntryStatusTests
{
    [Theory]
    [InlineData(EntryStatus.Booked, "BOOK")]
    [InlineData(EntryStatus.Pending, "PDNG")]
    [InlineData(EntryStatus.Information, "INFO")]
    [InlineData(EntryStatus.Future, "FUTR")]
    public void ToIso20022Code_ShouldReturnCorrectCode(EntryStatus status, string expectedCode)
    {
        // Act
        var code = status.ToIso20022Code();

        // Assert
        code.ShouldBe(expectedCode);
    }

    [Theory]
    [InlineData("BOOK", EntryStatus.Booked)]
    [InlineData("PDNG", EntryStatus.Pending)]
    [InlineData("INFO", EntryStatus.Information)]
    [InlineData("FUTR", EntryStatus.Future)]
    public void ParseEntryStatus_ValidCode_ShouldReturnCorrectEnum(string code, EntryStatus expected)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseEntryStatus(code);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("book")]
    [InlineData("Book")]
    [InlineData("BOOK")]
    public void ParseEntryStatus_CaseInsensitive_ShouldWork(string code)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseEntryStatus(code);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(EntryStatus.Booked);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseEntryStatus_EmptyCode_ShouldReturnNull(string? code)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseEntryStatus(code);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void ParseEntryStatus_UnknownCode_ShouldReturnNull()
    {
        // Act
        var result = Iso20022EnumExtensions.ParseEntryStatus("UNKNOWN");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void EntryStatus_AllValues_ShouldHaveDescriptionAttribute()
    {
        // Arrange
        var allStatuses = Enum.GetValues<EntryStatus>();

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var code = status.ToIso20022Code();
            code.ShouldNotBeNullOrWhiteSpace();
            code.Length.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public void EntryStatus_RoundTrip_ShouldWork()
    {
        // Arrange
        var original = EntryStatus.Booked;

        // Act
        var code = original.ToIso20022Code();
        var parsed = Iso20022EnumExtensions.ParseEntryStatus(code);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.Value.ShouldBe(original);
    }

    [Fact]
    public void GetDescription_ShouldReturnIsoCode()
    {
        // Act
        var bookedDesc = EntryStatus.Booked.GetDescription();
        var pendingDesc = EntryStatus.Pending.GetDescription();
        var infoDesc = EntryStatus.Information.GetDescription();
        var futureDesc = EntryStatus.Future.GetDescription();

        // Assert
        bookedDesc.ShouldBe("BOOK");
        pendingDesc.ShouldBe("PDNG");
        infoDesc.ShouldBe("INFO");
        futureDesc.ShouldBe("FUTR");
    }
}
