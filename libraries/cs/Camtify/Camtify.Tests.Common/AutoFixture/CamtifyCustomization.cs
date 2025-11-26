using Camtify.Domain.Common;
using Camtify.Tests.Common.AutoFixture.Builders;

namespace Camtify.Tests.Common.AutoFixture;

/// <summary>
/// Master customization for all Camtify domain types.
/// Registers all specimen builders for types with validation requirements.
/// </summary>
public class CamtifyCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // Register specimen builders for validated value objects
        fixture.Customizations.Add(new IbanSpecimenBuilder());
        fixture.Customizations.Add(new BicSpecimenBuilder());
        fixture.Customizations.Add(new CurrencyCodeSpecimenBuilder());
        fixture.Customizations.Add(new MessageIdentifierSpecimenBuilder());
        fixture.Customizations.Add(new MoneySpecimenBuilder());
        fixture.Customizations.Add(new ExchangeRateSpecimenBuilder());

        // Register concrete implementations for abstract types
        fixture.Register<AccountIdentification>(() =>
            new IbanAccountIdentification
            {
                Iban = fixture.Create<Iban>()
            });

        fixture.Register<PartyId>(() =>
            new OrganisationPartyId
            {
                Organisation = new OrganisationIdentification
                {
                    Lei = "529900T8BM49AURSDO55",
                    AnyBic = fixture.Create<Bic>().ToString()
                }
            });

        // Customize CashAccount to use generated Iban
        fixture.Customize<CashAccount>(c => c
            .With(x => x.Identification, () => new IbanAccountIdentification
            {
                Iban = fixture.Create<Iban>()
            })
            .With(x => x.Currency, () => fixture.Create<CurrencyCode>())
            .Without(x => x.Owner)
            .Without(x => x.Servicer)
            .Without(x => x.Proxy));

        // Customize Party to use generated Bic
        fixture.Customize<Party>(c => c
            .With(x => x.FinancialInstitutionId, () => new FinancialInstitutionIdentification
            {
                Bic = fixture.Create<Bic>().ToString()
            })
            .Without(x => x.OrganisationId));

        // Customize BusinessApplicationHeader
        fixture.Customize<BusinessApplicationHeader>(c => c
            .With(x => x.Version, () => fixture.Create<Camtify.Core.MessageIdentifier>())
            .With(x => x.From, () => fixture.Create<Party>())
            .With(x => x.To, () => fixture.Create<Party>())
            .Without(x => x.Related)
            .Without(x => x.Signature));

        // Customize DateAndDateTime
        fixture.Customize<DateAndDateTime>(c => c.FromFactory(() =>
            DateAndDateTime.FromDateTime(DateTime.UtcNow.AddDays(Random.Shared.Next(-365, 365)))));

        // Customize PostalAddress with reasonable values
        fixture.Customize<PostalAddress>(c => c
            .With(x => x.Country, "DE")
            .With(x => x.TownName, "Berlin")
            .With(x => x.PostCode, "10115")
            .Without(x => x.AddressLines));
    }
}
