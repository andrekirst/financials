using Camtify.Domain.Common;

namespace Camtify.Tests.Common.AutoFixture.Builders;

/// <summary>
/// Specimen builder for creating valid BIC instances.
/// Uses a pool of pre-validated BICs (SWIFT codes) from major banks.
/// </summary>
public class BicSpecimenBuilder : ISpecimenBuilder
{
    private static readonly string[] ValidBics =
    [
        "DEUTDEFF",     // Deutsche Bank, Germany
        "COBADEFF",     // Commerzbank, Germany
        "BNPAFRPP",     // BNP Paribas, France
        "BARCGB22",     // Barclays, UK
        "CHASUS33",     // JP Morgan Chase, USA
        "UBSWCHZH",     // UBS, Switzerland
        "INGBNL2A",     // ING Bank, Netherlands
        "KREDBEBB",     // KBC Bank, Belgium
        "BABORUHM",     // Bank Austria, Austria
        "BCITITMM"      // Intesa Sanpaolo, Italy
    ];

    private int _currentIndex;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(Bic))
        {
            return new NoSpecimen();
        }

        var bic = ValidBics[_currentIndex % ValidBics.Length];
        _currentIndex++;
        return Bic.Parse(bic);
    }
}
