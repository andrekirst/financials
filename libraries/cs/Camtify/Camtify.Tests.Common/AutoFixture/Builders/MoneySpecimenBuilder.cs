using Camtify.Domain.Common;

namespace Camtify.Tests.Common.AutoFixture.Builders;

/// <summary>
/// Specimen builder for creating valid Money instances.
/// Generates realistic amounts with valid CurrencyCode.
/// </summary>
public class MoneySpecimenBuilder : ISpecimenBuilder
{
    private static readonly Random Random = new();

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(Money))
        {
            return new NoSpecimen();
        }

        var currency = (CurrencyCode)context.Resolve(typeof(CurrencyCode));
        var amount = Math.Round((decimal)(Random.NextDouble() * 10000), currency.DecimalPlaces);

        return new Money(amount, currency);
    }
}
