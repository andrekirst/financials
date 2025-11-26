using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class ExchangeRateTests
{
    [Fact]
    public void Convert_ShouldConvertCorrectly()
    {
        // Arrange
        var rate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 1.10m
        };
        var eurAmount = new Money(100m, CurrencyCode.EUR);

        // Act
        var usdAmount = rate.Convert(eurAmount);

        // Assert
        usdAmount.Amount.ShouldBe(110m);
        usdAmount.Currency.ShouldBe(CurrencyCode.USD);
    }

    [Fact]
    public void Convert_WrongSourceCurrency_ShouldThrow()
    {
        // Arrange
        var rate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 1.10m
        };
        var gbpAmount = new Money(100m, CurrencyCode.GBP);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => rate.Convert(gbpAmount));
    }

    [Fact]
    public void Invert_ShouldSwapCurrenciesAndInvertRate()
    {
        // Arrange
        var rate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 2m,
            RateType = ExchangeRateType.Spot,
            QuotationDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            ContractIdentification = "CONTRACT123"
        };

        // Act
        var inverted = rate.Invert();

        // Assert
        inverted.SourceCurrency.ShouldBe(CurrencyCode.USD);
        inverted.TargetCurrency.ShouldBe(CurrencyCode.EUR);
        inverted.Rate.ShouldBe(0.5m);
        inverted.RateType.ShouldBe(ExchangeRateType.Spot);
        inverted.QuotationDate.ShouldBe(rate.QuotationDate);
        inverted.ContractIdentification.ShouldBe(rate.ContractIdentification);
    }

    [Fact]
    public void Convert_WithInvertedRate_ShouldRoundTrip()
    {
        // Arrange
        var rate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 1.10m
        };
        var originalAmount = new Money(100m, CurrencyCode.EUR);

        // Act
        var usdAmount = rate.Convert(originalAmount);
        var backToEur = rate.Invert().Convert(usdAmount);

        // Assert
        backToEur.Amount.ShouldBe(originalAmount.Amount, 0.0000000001m); // Allow small precision difference
        backToEur.Currency.ShouldBe(originalAmount.Currency);
    }

    [Fact]
    public void RateType_ShouldBeStoredCorrectly()
    {
        // Arrange
        var spotRate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 1.10m,
            RateType = ExchangeRateType.Spot
        };

        var forwardRate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 1.12m,
            RateType = ExchangeRateType.Forward
        };

        // Assert
        spotRate.RateType.ShouldBe(ExchangeRateType.Spot);
        forwardRate.RateType.ShouldBe(ExchangeRateType.Forward);
    }

    [Fact]
    public void QuotationDate_ShouldBeStoredCorrectly()
    {
        // Arrange
        var quotationDate = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var rate = new ExchangeRate
        {
            SourceCurrency = CurrencyCode.EUR,
            TargetCurrency = CurrencyCode.USD,
            Rate = 1.10m,
            QuotationDate = quotationDate
        };

        // Assert
        rate.QuotationDate.ShouldBe(quotationDate);
    }
}
