using Camtify.Domain.Common;

namespace Camtify.Tests.Common.AutoFixture.Builders;

/// <summary>
/// Specimen builder for creating valid CurrencyCode instances.
/// Cycles through common ISO 4217 currency codes.
/// </summary>
public class CurrencyCodeSpecimenBuilder : ISpecimenBuilder
{
    private static readonly CurrencyCode[] ValidCurrencies =
    [
        CurrencyCode.EUR,
        CurrencyCode.USD,
        CurrencyCode.GBP,
        CurrencyCode.CHF,
        CurrencyCode.JPY
    ];

    private int _currentIndex;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(CurrencyCode))
        {
            return new NoSpecimen();
        }

        var currency = ValidCurrencies[_currentIndex % ValidCurrencies.Length];
        _currentIndex++;
        return currency;
    }
}
