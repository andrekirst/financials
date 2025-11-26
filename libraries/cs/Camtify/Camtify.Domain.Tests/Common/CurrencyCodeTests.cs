using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class CurrencyCodeTests
{
    [Fact]
    public void StaticInstances_ShouldHaveCorrectCodes()
    {
        CurrencyCode.EUR.Code.ShouldBe("EUR");
        CurrencyCode.USD.Code.ShouldBe("USD");
        CurrencyCode.GBP.Code.ShouldBe("GBP");
        CurrencyCode.CHF.Code.ShouldBe("CHF");
        CurrencyCode.JPY.Code.ShouldBe("JPY");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("eur")]
    [InlineData("Eur")]
    public void Parse_ValidCode_ShouldSucceed(string code)
    {
        // Act
        var currency = CurrencyCode.Parse(code);

        // Assert
        currency.Code.ShouldBe(code.ToUpperInvariant());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyCode_ShouldThrow(string? code)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CurrencyCode.Parse(code!));
    }

    [Theory]
    [InlineData("EU")]
    [InlineData("EURO")]
    [InlineData("E1R")]
    [InlineData("123")]
    public void Parse_InvalidCode_ShouldThrow(string code)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CurrencyCode.Parse(code));
    }

    [Fact]
    public void TryParse_ValidCode_ShouldReturnTrue()
    {
        // Act
        var success = CurrencyCode.TryParse("EUR", out var currency, out var error);

        // Assert
        success.ShouldBeTrue();
        currency.Code.ShouldBe("EUR");
        error.ShouldBeNull();
    }

    [Fact]
    public void TryParse_InvalidCode_ShouldReturnFalse()
    {
        // Act
        var success = CurrencyCode.TryParse("INVALID", out var currency, out var error);

        // Assert
        success.ShouldBeFalse();
        error.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("EUR", 2)]
    [InlineData("USD", 2)]
    [InlineData("JPY", 0)]
    [InlineData("KWD", 3)]
    [InlineData("BHD", 3)]
    public void DecimalPlaces_ShouldReturnCorrectValue(string code, int expectedDecimals)
    {
        // Arrange
        var currency = CurrencyCode.Parse(code);

        // Assert
        currency.DecimalPlaces.ShouldBe(expectedDecimals);
    }

    [Fact]
    public void Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var eur1 = CurrencyCode.EUR;
        var eur2 = CurrencyCode.Parse("EUR");
        var usd = CurrencyCode.USD;

        // Assert
        eur1.ShouldBe(eur2);
        (eur1 == eur2).ShouldBeTrue();
        (eur1 == usd).ShouldBeFalse();
        (eur1 != usd).ShouldBeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnCode()
    {
        // Assert
        CurrencyCode.EUR.ToString().ShouldBe("EUR");
        CurrencyCode.USD.ToString().ShouldBe("USD");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnCode()
    {
        // Arrange
        CurrencyCode currency = CurrencyCode.EUR;

        // Act
        string code = currency;

        // Assert
        code.ShouldBe("EUR");
    }

    [Fact]
    public void GetHashCode_SameCode_ShouldBeSame()
    {
        // Arrange
        var eur1 = CurrencyCode.EUR;
        var eur2 = CurrencyCode.Parse("EUR");

        // Assert
        eur1.GetHashCode().ShouldBe(eur2.GetHashCode());
    }
}
