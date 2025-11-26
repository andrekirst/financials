using Camtify.Domain.Common;

namespace Camtify.Tests.Common.AutoFixture.Builders;

/// <summary>
/// Specimen builder for creating valid IBAN instances.
/// Uses a pool of pre-validated IBANs from different countries.
/// </summary>
public class IbanSpecimenBuilder : ISpecimenBuilder
{
    private static readonly string[] ValidIbans =
    [
        "DE89370400440532013000",       // Germany
        "FR7630006000011234567890189",  // France
        "GB82WEST12345698765432",       // United Kingdom
        "AT611904300234573201",         // Austria
        "CH9300762011623852957",        // Switzerland
        "ES9121000418450200051332",     // Spain
        "IT60X0542811101000000123456",  // Italy
        "NL91ABNA0417164300",           // Netherlands
        "BE68539007547034",             // Belgium
        "LU280019400644750000"          // Luxembourg
    ];

    private int _currentIndex;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(Iban))
        {
            return new NoSpecimen();
        }

        var iban = ValidIbans[_currentIndex % ValidIbans.Length];
        _currentIndex++;
        return Iban.Parse(iban);
    }
}
