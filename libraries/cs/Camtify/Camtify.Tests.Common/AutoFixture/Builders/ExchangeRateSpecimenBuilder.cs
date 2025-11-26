using Camtify.Domain.Common;

namespace Camtify.Tests.Common.AutoFixture.Builders;

/// <summary>
/// Specimen builder for creating valid ExchangeRate instances.
/// Ensures SourceCurrency and TargetCurrency are different.
/// </summary>
public class ExchangeRateSpecimenBuilder : ISpecimenBuilder
{
    private static readonly (CurrencyCode Source, CurrencyCode Target, decimal Rate)[] CurrencyPairs =
    [
        (CurrencyCode.EUR, CurrencyCode.USD, 1.10m),
        (CurrencyCode.EUR, CurrencyCode.GBP, 0.85m),
        (CurrencyCode.EUR, CurrencyCode.CHF, 0.95m),
        (CurrencyCode.USD, CurrencyCode.EUR, 0.91m),
        (CurrencyCode.USD, CurrencyCode.GBP, 0.77m),
        (CurrencyCode.GBP, CurrencyCode.EUR, 1.18m),
        (CurrencyCode.GBP, CurrencyCode.USD, 1.30m),
        (CurrencyCode.CHF, CurrencyCode.EUR, 1.05m)
    ];

    private int _currentIndex;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(ExchangeRate))
        {
            return new NoSpecimen();
        }

        var pair = CurrencyPairs[_currentIndex % CurrencyPairs.Length];
        _currentIndex++;

        return new ExchangeRate
        {
            SourceCurrency = pair.Source,
            TargetCurrency = pair.Target,
            Rate = pair.Rate,
            RateType = ExchangeRateType.Spot
        };
    }
}
