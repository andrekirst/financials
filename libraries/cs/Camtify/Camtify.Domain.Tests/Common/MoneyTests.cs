using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldSetAmountAndCurrency()
    {
        // Arrange & Act
        var money = new Money(100.50m, CurrencyCode.EUR);

        // Assert
        money.Amount.ShouldBe(100.50m);
        money.Currency.ShouldBe(CurrencyCode.EUR);
    }

    [Fact]
    public void Parse_ShouldCreateMoneyFromStrings()
    {
        // Arrange & Act
        var money = Money.Parse("1234.56", "EUR");

        // Assert
        money.Amount.ShouldBe(1234.56m);
        money.Currency.ShouldBe(CurrencyCode.EUR);
    }

    [Fact]
    public void Abs_ShouldReturnAbsoluteValue()
    {
        // Arrange
        var negative = new Money(-100m, CurrencyCode.EUR);
        var positive = new Money(100m, CurrencyCode.EUR);

        // Act & Assert
        negative.Abs().Amount.ShouldBe(100m);
        positive.Abs().Amount.ShouldBe(100m);
    }

    [Fact]
    public void Negate_ShouldNegateAmount()
    {
        // Arrange
        var positive = new Money(100m, CurrencyCode.EUR);
        var negative = new Money(-100m, CurrencyCode.EUR);

        // Act & Assert
        positive.Negate().Amount.ShouldBe(-100m);
        negative.Negate().Amount.ShouldBe(100m);
    }

    [Theory]
    [InlineData(100, true, false, false)]
    [InlineData(-100, false, true, false)]
    [InlineData(0, false, false, true)]
    public void IsPositive_IsNegative_IsZero_ShouldReturnCorrectValues(
        decimal amount, bool isPositive, bool isNegative, bool isZero)
    {
        // Arrange
        var money = new Money(amount, CurrencyCode.EUR);

        // Assert
        money.IsPositive.ShouldBe(isPositive);
        money.IsNegative.ShouldBe(isNegative);
        money.IsZero.ShouldBe(isZero);
    }

    [Fact]
    public void Addition_SameCurrency_ShouldSucceed()
    {
        // Arrange
        var a = new Money(100m, CurrencyCode.EUR);
        var b = new Money(50m, CurrencyCode.EUR);

        // Act
        var result = a + b;

        // Assert
        result.Amount.ShouldBe(150m);
        result.Currency.ShouldBe(CurrencyCode.EUR);
    }

    [Fact]
    public void Addition_DifferentCurrency_ShouldThrow()
    {
        // Arrange
        var eur = new Money(100m, CurrencyCode.EUR);
        var usd = new Money(50m, CurrencyCode.USD);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => eur + usd);
    }

    [Fact]
    public void Subtraction_SameCurrency_ShouldSucceed()
    {
        // Arrange
        var a = new Money(100m, CurrencyCode.EUR);
        var b = new Money(30m, CurrencyCode.EUR);

        // Act
        var result = a - b;

        // Assert
        result.Amount.ShouldBe(70m);
        result.Currency.ShouldBe(CurrencyCode.EUR);
    }

    [Fact]
    public void Subtraction_DifferentCurrency_ShouldThrow()
    {
        // Arrange
        var eur = new Money(100m, CurrencyCode.EUR);
        var usd = new Money(50m, CurrencyCode.USD);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => eur - usd);
    }

    [Fact]
    public void Multiplication_ShouldMultiplyAmount()
    {
        // Arrange
        var money = new Money(100m, CurrencyCode.EUR);

        // Act
        var result1 = money * 2.5m;
        var result2 = 2.5m * money;

        // Assert
        result1.Amount.ShouldBe(250m);
        result2.Amount.ShouldBe(250m);
        result1.Currency.ShouldBe(CurrencyCode.EUR);
    }

    [Fact]
    public void Division_ShouldDivideAmount()
    {
        // Arrange
        var money = new Money(100m, CurrencyCode.EUR);

        // Act
        var result = money / 4m;

        // Assert
        result.Amount.ShouldBe(25m);
        result.Currency.ShouldBe(CurrencyCode.EUR);
    }

    [Fact]
    public void Division_ByZero_ShouldThrow()
    {
        // Arrange
        var money = new Money(100m, CurrencyCode.EUR);

        // Act & Assert
        Should.Throw<DivideByZeroException>(() => money / 0m);
    }

    [Fact]
    public void CompareTo_SameCurrency_ShouldCompareCorrectly()
    {
        // Arrange
        var small = new Money(50m, CurrencyCode.EUR);
        var large = new Money(100m, CurrencyCode.EUR);
        var equal = new Money(100m, CurrencyCode.EUR);

        // Assert
        (small.CompareTo(large) < 0).ShouldBeTrue();
        (large.CompareTo(small) > 0).ShouldBeTrue();
        large.CompareTo(equal).ShouldBe(0);
    }

    [Fact]
    public void CompareTo_DifferentCurrency_ShouldThrow()
    {
        // Arrange
        var eur = new Money(100m, CurrencyCode.EUR);
        var usd = new Money(100m, CurrencyCode.USD);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => eur.CompareTo(usd));
    }

    [Fact]
    public void ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var small = new Money(50m, CurrencyCode.EUR);
        var large = new Money(100m, CurrencyCode.EUR);

        // Assert
        (small < large).ShouldBeTrue();
        (large > small).ShouldBeTrue();
        (small <= large).ShouldBeTrue();
        (large >= small).ShouldBeTrue();
        (small <= new Money(50m, CurrencyCode.EUR)).ShouldBeTrue();
        (large >= new Money(100m, CurrencyCode.EUR)).ShouldBeTrue();
    }

    [Fact]
    public void Zero_ShouldCreateZeroAmount()
    {
        // Act
        var zero = Money.Zero(CurrencyCode.EUR);

        // Assert
        zero.Amount.ShouldBe(0m);
        zero.Currency.ShouldBe(CurrencyCode.EUR);
        zero.IsZero.ShouldBeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = new Money(1234.56m, CurrencyCode.EUR);

        // Act
        var result = money.ToString();

        // Assert
        result.ShouldContain("1");
        result.ShouldContain("234");
        result.ShouldContain("56");
        result.ShouldContain("EUR");
    }

    [Fact]
    public void Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var a = new Money(100m, CurrencyCode.EUR);
        var b = new Money(100m, CurrencyCode.EUR);
        var c = new Money(100m, CurrencyCode.USD);
        var d = new Money(50m, CurrencyCode.EUR);

        // Assert
        a.ShouldBe(b);
        a.ShouldNotBe(c);
        a.ShouldNotBe(d);
    }

    [Fact]
    public void Precision_ShouldMaintainDecimalPrecision()
    {
        // Arrange - Testing decimal precision (no floating point errors)
        var a = new Money(0.1m, CurrencyCode.EUR);
        var b = new Money(0.2m, CurrencyCode.EUR);

        // Act
        var result = a + b;

        // Assert - This would fail with double due to floating point errors
        result.Amount.ShouldBe(0.3m);
    }

    [Fact]
    public void LargeAmounts_ShouldHandleCorrectly()
    {
        // Arrange - Testing large amounts
        var large = new Money(999_999_999_999.99m, CurrencyCode.EUR);
        var small = new Money(0.01m, CurrencyCode.EUR);

        // Act
        var result = large + small;

        // Assert
        result.Amount.ShouldBe(1_000_000_000_000.00m);
    }
}
