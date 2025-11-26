using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class PartyIdentificationTests
{
    [Fact]
    public void PartyIdentification_AllProperties_ShouldBeSet()
    {
        // Arrange & Act
        var party = new PartyIdentification
        {
            Name = "Max Mustermann GmbH",
            PostalAddress = new PostalAddress
            {
                StreetName = "Musterstraße",
                BuildingNumber = "123",
                PostCode = "12345",
                TownName = "Berlin",
                Country = "DE"
            },
            Identification = new OrganisationPartyId
            {
                Organisation = new OrganisationIdentification
                {
                    Lei = "529900T8BM49AURSDO55"
                }
            },
            CountryOfResidence = "DE",
            ContactDetails = new ContactDetails
            {
                EmailAddress = "info@mustermann.de",
                PhoneNumber = "+49 30 12345678"
            }
        };

        // Assert
        party.Name.ShouldBe("Max Mustermann GmbH");
        party.PostalAddress.ShouldNotBeNull();
        party.PostalAddress.TownName.ShouldBe("Berlin");
        party.Identification.ShouldNotBeNull();
        party.Identification.ShouldBeOfType<OrganisationPartyId>();
        party.CountryOfResidence.ShouldBe("DE");
        party.ContactDetails.ShouldNotBeNull();
        party.ContactDetails.EmailAddress.ShouldBe("info@mustermann.de");
    }

    [Fact]
    public void PartyIdentification_OptionalProperties_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var party = new PartyIdentification();

        // Assert
        party.Name.ShouldBeNull();
        party.PostalAddress.ShouldBeNull();
        party.Identification.ShouldBeNull();
        party.CountryOfResidence.ShouldBeNull();
        party.ContactDetails.ShouldBeNull();
    }

    [Fact]
    public void OrganisationPartyId_ShouldContainOrganisationIdentification()
    {
        // Arrange & Act
        var orgId = new OrganisationPartyId
        {
            Organisation = new OrganisationIdentification
            {
                AnyBic = "DEUTDEFF",
                Lei = "529900T8BM49AURSDO55"
            }
        };

        // Assert
        orgId.Organisation.AnyBic.ShouldBe("DEUTDEFF");
        orgId.Organisation.Lei.ShouldBe("529900T8BM49AURSDO55");
    }

    [Fact]
    public void PersonPartyId_ShouldContainPersonIdentification()
    {
        // Arrange & Act
        var personId = new PersonPartyId
        {
            Person = new PersonIdentification
            {
                DateAndPlaceOfBirth = new DateAndPlaceOfBirth
                {
                    BirthDate = new DateOnly(1980, 5, 15),
                    CityOfBirth = "Berlin",
                    CountryOfBirth = "DE"
                }
            }
        };

        // Assert
        personId.Person.DateAndPlaceOfBirth.ShouldNotBeNull();
        personId.Person.DateAndPlaceOfBirth.BirthDate.ShouldBe(new DateOnly(1980, 5, 15));
        personId.Person.DateAndPlaceOfBirth.CityOfBirth.ShouldBe("Berlin");
    }

    [Fact]
    public void PartyId_IsAbstract_CanBeEitherOrgOrPerson()
    {
        // Arrange
        PartyId orgPartyId = new OrganisationPartyId
        {
            Organisation = new OrganisationIdentification { Lei = "12345" }
        };

        PartyId personPartyId = new PersonPartyId
        {
            Person = new PersonIdentification()
        };

        // Assert
        orgPartyId.ShouldBeOfType<OrganisationPartyId>();
        personPartyId.ShouldBeOfType<PersonPartyId>();
    }
}

public class PostalAddressTests
{
    [Fact]
    public void PostalAddress_StructuredFormat_ShouldStoreAllFields()
    {
        // Arrange & Act
        var address = new PostalAddress
        {
            AddressType = AddressType.Business,
            Department = "IT Abteilung",
            SubDepartment = "Entwicklung",
            StreetName = "Hauptstraße",
            BuildingNumber = "42",
            BuildingName = "Tech Tower",
            Floor = "5",
            PostBox = "PO 12345",
            Room = "501",
            PostCode = "10115",
            TownName = "Berlin",
            TownLocationName = "Mitte",
            DistrictName = "Bezirk Mitte",
            CountrySubDivision = "BE",
            Country = "DE"
        };

        // Assert
        address.AddressType.ShouldBe(AddressType.Business);
        address.Department.ShouldBe("IT Abteilung");
        address.SubDepartment.ShouldBe("Entwicklung");
        address.StreetName.ShouldBe("Hauptstraße");
        address.BuildingNumber.ShouldBe("42");
        address.BuildingName.ShouldBe("Tech Tower");
        address.Floor.ShouldBe("5");
        address.PostBox.ShouldBe("PO 12345");
        address.Room.ShouldBe("501");
        address.PostCode.ShouldBe("10115");
        address.TownName.ShouldBe("Berlin");
        address.TownLocationName.ShouldBe("Mitte");
        address.DistrictName.ShouldBe("Bezirk Mitte");
        address.CountrySubDivision.ShouldBe("BE");
        address.Country.ShouldBe("DE");
    }

    [Fact]
    public void PostalAddress_UnstructuredFormat_ShouldStoreAddressLines()
    {
        // Arrange & Act
        var address = new PostalAddress
        {
            AddressLines = new[]
            {
                "Max Mustermann GmbH",
                "Hauptstraße 42",
                "10115 Berlin",
                "Germany"
            },
            Country = "DE"
        };

        // Assert
        address.AddressLines.ShouldNotBeNull();
        address.AddressLines.Count.ShouldBe(4);
        address.AddressLines[0].ShouldBe("Max Mustermann GmbH");
        address.AddressLines[2].ShouldBe("10115 Berlin");
    }

    [Theory]
    [InlineData(AddressType.Postal)]
    [InlineData(AddressType.POBox)]
    [InlineData(AddressType.Home)]
    [InlineData(AddressType.Business)]
    [InlineData(AddressType.MailTo)]
    [InlineData(AddressType.Delivery)]
    public void AddressType_AllValues_ShouldBeValid(AddressType addressType)
    {
        // Arrange & Act
        var address = new PostalAddress
        {
            AddressType = addressType
        };

        // Assert
        address.AddressType.ShouldBe(addressType);
    }
}

public class ContactDetailsTests
{
    [Fact]
    public void ContactDetails_AllFields_ShouldBeSet()
    {
        // Arrange & Act
        var contact = new ContactDetails
        {
            NamePrefix = NamePrefix.Mister,
            Name = "Max Mustermann",
            PhoneNumber = "+49 30 12345678",
            MobileNumber = "+49 170 9876543",
            FaxNumber = "+49 30 12345679",
            EmailAddress = "max@mustermann.de",
            EmailPurpose = "Business",
            JobTitle = "CEO",
            Responsibility = "Management",
            Department = "Executive",
            PreferredMethod = PreferredContactMethod.Email
        };

        // Assert
        contact.NamePrefix.ShouldBe(NamePrefix.Mister);
        contact.Name.ShouldBe("Max Mustermann");
        contact.PhoneNumber.ShouldBe("+49 30 12345678");
        contact.MobileNumber.ShouldBe("+49 170 9876543");
        contact.FaxNumber.ShouldBe("+49 30 12345679");
        contact.EmailAddress.ShouldBe("max@mustermann.de");
        contact.EmailPurpose.ShouldBe("Business");
        contact.JobTitle.ShouldBe("CEO");
        contact.Responsibility.ShouldBe("Management");
        contact.Department.ShouldBe("Executive");
        contact.PreferredMethod.ShouldBe(PreferredContactMethod.Email);
    }

    [Fact]
    public void ContactDetails_OtherChannels_ShouldStoreMultipleChannels()
    {
        // Arrange & Act
        var contact = new ContactDetails
        {
            Other = new[]
            {
                new OtherContact { ChannelType = "SKYPE", Identification = "max.mustermann" },
                new OtherContact { ChannelType = "TEAMS", Identification = "max@company.com" }
            }
        };

        // Assert
        contact.Other.ShouldNotBeNull();
        contact.Other.Count.ShouldBe(2);
        contact.Other[0].ChannelType.ShouldBe("SKYPE");
        contact.Other[0].Identification.ShouldBe("max.mustermann");
    }

    [Theory]
    [InlineData(NamePrefix.Doctor)]
    [InlineData(NamePrefix.Madame)]
    [InlineData(NamePrefix.Miss)]
    [InlineData(NamePrefix.Mister)]
    [InlineData(NamePrefix.Mx)]
    public void NamePrefix_AllValues_ShouldBeValid(NamePrefix prefix)
    {
        // Arrange & Act
        var contact = new ContactDetails { NamePrefix = prefix };

        // Assert
        contact.NamePrefix.ShouldBe(prefix);
    }

    [Theory]
    [InlineData(PreferredContactMethod.Letter)]
    [InlineData(PreferredContactMethod.Email)]
    [InlineData(PreferredContactMethod.Phone)]
    [InlineData(PreferredContactMethod.Fax)]
    [InlineData(PreferredContactMethod.Mobile)]
    public void PreferredContactMethod_AllValues_ShouldBeValid(PreferredContactMethod method)
    {
        // Arrange & Act
        var contact = new ContactDetails { PreferredMethod = method };

        // Assert
        contact.PreferredMethod.ShouldBe(method);
    }
}

public class PersonIdentificationTests
{
    [Fact]
    public void DateAndPlaceOfBirth_AllFields_ShouldBeSet()
    {
        // Arrange & Act
        var birthInfo = new DateAndPlaceOfBirth
        {
            BirthDate = new DateOnly(1985, 3, 20),
            ProvinceOfBirth = "Bavaria",
            CityOfBirth = "Munich",
            CountryOfBirth = "DE"
        };

        // Assert
        birthInfo.BirthDate.ShouldBe(new DateOnly(1985, 3, 20));
        birthInfo.ProvinceOfBirth.ShouldBe("Bavaria");
        birthInfo.CityOfBirth.ShouldBe("Munich");
        birthInfo.CountryOfBirth.ShouldBe("DE");
    }

    [Fact]
    public void GenericPersonIdentification_ShouldStoreIdAndScheme()
    {
        // Arrange & Act
        var personId = new GenericPersonIdentification
        {
            Identification = "DE123456789",
            SchemeName = new PersonIdentificationSchemeName
            {
                Code = "NIDN" // National Identity Number
            },
            Issuer = "German Government"
        };

        // Assert
        personId.Identification.ShouldBe("DE123456789");
        personId.SchemeName.ShouldNotBeNull();
        personId.SchemeName.Code.ShouldBe("NIDN");
        personId.Issuer.ShouldBe("German Government");
    }

    [Fact]
    public void PersonIdentification_WithDateAndOther_ShouldStoreAll()
    {
        // Arrange & Act
        var person = new PersonIdentification
        {
            DateAndPlaceOfBirth = new DateAndPlaceOfBirth
            {
                BirthDate = new DateOnly(1990, 1, 1),
                CityOfBirth = "Hamburg",
                CountryOfBirth = "DE"
            },
            Other = new[]
            {
                new GenericPersonIdentification
                {
                    Identification = "PASS123456",
                    SchemeName = new PersonIdentificationSchemeName { Code = "CCPT" } // Passport
                }
            }
        };

        // Assert
        person.DateAndPlaceOfBirth.ShouldNotBeNull();
        person.Other.ShouldNotBeNull();
        person.Other.ShouldHaveSingleItem();
        person.Other[0].Identification.ShouldBe("PASS123456");
    }
}
