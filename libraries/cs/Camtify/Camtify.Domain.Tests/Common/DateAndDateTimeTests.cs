using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class DateAndDateTimeTests
{
    [Fact]
    public void FromDateTime_ShouldHaveTimeComponent()
    {
        // Arrange
        var dateTime = new DateTime(2024, 6, 15, 14, 30, 45);

        // Act
        var result = DateAndDateTime.FromDateTime(dateTime);

        // Assert
        result.HasTime.ShouldBeTrue();
        result.AsDateTime.ShouldBe(dateTime);
    }

    [Fact]
    public void FromDate_ShouldNotHaveTimeComponent()
    {
        // Arrange
        var date = new DateOnly(2024, 6, 15);

        // Act
        var result = DateAndDateTime.FromDate(date);

        // Assert
        result.HasTime.ShouldBeFalse();
        result.AsDateOnly.ShouldBe(date);
    }

    [Theory]
    [InlineData("2024-06-15")]
    [InlineData("2024-12-31")]
    public void Parse_DateOnly_ShouldNotHaveTime(string value)
    {
        // Act
        var result = DateAndDateTime.Parse(value);

        // Assert
        result.HasTime.ShouldBeFalse();
    }

    [Theory]
    [InlineData("2024-06-15T14:30:45")]
    [InlineData("2024-06-15T00:00:00")]
    [InlineData("2024-06-15T14:30:45Z")]
    public void Parse_DateTime_ShouldHaveTime(string value)
    {
        // Act
        var result = DateAndDateTime.Parse(value);

        // Assert
        result.HasTime.ShouldBeTrue();
    }

    [Fact]
    public void Parse_EmptyString_ShouldThrow()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => DateAndDateTime.Parse(""));
        Should.Throw<ArgumentException>(() => DateAndDateTime.Parse("   "));
    }

    [Fact]
    public void TryParse_ValidDate_ShouldSucceed()
    {
        // Act
        var success = DateAndDateTime.TryParse("2024-06-15", out var result);

        // Assert
        success.ShouldBeTrue();
        result.AsDateOnly.ShouldBe(new DateOnly(2024, 6, 15));
    }

    [Fact]
    public void TryParse_InvalidDate_ShouldFail()
    {
        // Act
        var success = DateAndDateTime.TryParse("not-a-date", out _);

        // Assert
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryParse_Null_ShouldFail()
    {
        // Act
        var success = DateAndDateTime.TryParse(null, out _);

        // Assert
        success.ShouldBeFalse();
    }

    [Fact]
    public void CompareTo_ShouldCompareCorrectly()
    {
        // Arrange
        var earlier = DateAndDateTime.FromDate(new DateOnly(2024, 1, 1));
        var later = DateAndDateTime.FromDate(new DateOnly(2024, 12, 31));

        // Assert
        (earlier.CompareTo(later) < 0).ShouldBeTrue();
        (later.CompareTo(earlier) > 0).ShouldBeTrue();
    }

    [Fact]
    public void ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var earlier = DateAndDateTime.FromDate(new DateOnly(2024, 1, 1));
        var later = DateAndDateTime.FromDate(new DateOnly(2024, 12, 31));

        // Assert
        (earlier < later).ShouldBeTrue();
        (later > earlier).ShouldBeTrue();
        (earlier <= later).ShouldBeTrue();
        (later >= earlier).ShouldBeTrue();
    }

    [Fact]
    public void Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var a = DateAndDateTime.FromDate(new DateOnly(2024, 6, 15));
        var b = DateAndDateTime.FromDate(new DateOnly(2024, 6, 15));
        var c = DateAndDateTime.FromDateTime(new DateTime(2024, 6, 15, 0, 0, 0));

        // Assert
        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.ShouldNotBe(c); // Same date but different HasTime flag
    }

    [Fact]
    public void ToString_DateOnly_ShouldReturnIsoFormat()
    {
        // Arrange
        var date = DateAndDateTime.FromDate(new DateOnly(2024, 6, 15));

        // Act
        var result = date.ToString();

        // Assert
        result.ShouldBe("2024-06-15");
    }

    [Fact]
    public void ToString_DateTime_ShouldReturnIsoFormat()
    {
        // Arrange
        var dateTime = DateAndDateTime.FromDateTime(new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc));

        // Act
        var result = dateTime.ToString();

        // Assert
        result.ShouldContain("2024-06-15");
        result.ShouldContain("T");
    }

    [Fact]
    public void ImplicitConversion_FromDateTime_ShouldWork()
    {
        // Arrange
        DateTime dateTime = new DateTime(2024, 6, 15, 14, 30, 0);

        // Act
        DateAndDateTime result = dateTime;

        // Assert
        result.HasTime.ShouldBeTrue();
        result.AsDateTime.ShouldBe(dateTime);
    }

    [Fact]
    public void ImplicitConversion_FromDateOnly_ShouldWork()
    {
        // Arrange
        DateOnly date = new DateOnly(2024, 6, 15);

        // Act
        DateAndDateTime result = date;

        // Assert
        result.HasTime.ShouldBeFalse();
        result.AsDateOnly.ShouldBe(date);
    }

    [Fact]
    public void AsDateOnly_ShouldExtractDatePart()
    {
        // Arrange
        var dateTime = DateAndDateTime.FromDateTime(new DateTime(2024, 6, 15, 14, 30, 45));

        // Act
        var dateOnly = dateTime.AsDateOnly;

        // Assert
        dateOnly.ShouldBe(new DateOnly(2024, 6, 15));
    }
}
