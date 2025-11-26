using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class CashAccountTests
{
    [Fact]
    public void CashAccount_WithIbanIdentification_ShouldStoreAllProperties()
    {
        // Arrange & Act
        var account = new CashAccount
        {
            Identification = new IbanAccountIdentification
            {
                Iban = Iban.Parse("DE89370400440532013000")
            },
            Type = new AccountType { Code = "CACC" },
            Currency = CurrencyCode.EUR,
            Name = "Current Account"
        };

        // Assert
        account.Identification.ShouldNotBeNull();
        account.Identification.ShouldBeOfType<IbanAccountIdentification>();
        var ibanId = (IbanAccountIdentification)account.Identification;
        ibanId.Iban.Electronic.ShouldBe("DE89370400440532013000");
        account.Type?.Code.ShouldBe("CACC");
        account.Currency.ShouldBe(CurrencyCode.EUR);
        account.Name.ShouldBe("Current Account");
    }

    [Fact]
    public void CashAccount_WithOtherIdentification_ShouldStoreAllProperties()
    {
        // Arrange & Act
        var account = new CashAccount
        {
            Identification = new OtherAccountIdentification
            {
                Other = new GenericAccountIdentification
                {
                    Identification = "123456789",
                    SchemeName = new AccountSchemeName { Code = "BBAN" },
                    Issuer = "Local Bank"
                }
            },
            Type = new AccountType { Code = "SVGS" },
            Currency = CurrencyCode.USD
        };

        // Assert
        account.Identification.ShouldNotBeNull();
        account.Identification.ShouldBeOfType<OtherAccountIdentification>();
        var otherId = (OtherAccountIdentification)account.Identification;
        otherId.Other.Identification.ShouldBe("123456789");
        otherId.Other.SchemeName?.Code.ShouldBe("BBAN");
        otherId.Other.Issuer.ShouldBe("Local Bank");
    }

    [Fact]
    public void CashAccount_WithProxy_ShouldStoreProxyDetails()
    {
        // Arrange & Act
        var account = new CashAccount
        {
            Identification = new IbanAccountIdentification
            {
                Iban = Iban.Parse("DE89370400440532013000")
            },
            Proxy = new ProxyAccountIdentification
            {
                Type = new ProxyAccountType { Code = "TELE" },
                Identification = "+49170123456789"
            }
        };

        // Assert
        account.Proxy.ShouldNotBeNull();
        account.Proxy.Type?.Code.ShouldBe("TELE");
        account.Proxy.Identification.ShouldBe("+49170123456789");
    }

    [Fact]
    public void CashAccount_WithOwnerAndServicer_ShouldStoreDetails()
    {
        // Arrange & Act
        var account = new CashAccount
        {
            Identification = new IbanAccountIdentification
            {
                Iban = Iban.Parse("DE89370400440532013000")
            },
            Owner = new PartyIdentification
            {
                Name = "Max Mustermann"
            },
            Servicer = new BranchAndFinancialInstitutionIdentification
            {
                FinancialInstitutionIdentification = new FinancialInstitutionIdentification
                {
                    Bic = "DEUTDEFF"
                }
            }
        };

        // Assert
        account.Owner.ShouldNotBeNull();
        account.Owner.Name.ShouldBe("Max Mustermann");
        account.Servicer.ShouldNotBeNull();
        account.Servicer.FinancialInstitutionIdentification?.Bic.ShouldBe("DEUTDEFF");
    }

    [Fact]
    public void CashAccount_OptionalProperties_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var account = new CashAccount
        {
            Identification = new IbanAccountIdentification
            {
                Iban = Iban.Parse("DE89370400440532013000")
            }
        };

        // Assert
        account.Type.ShouldBeNull();
        account.Currency.ShouldBeNull();
        account.Name.ShouldBeNull();
        account.Proxy.ShouldBeNull();
        account.Owner.ShouldBeNull();
        account.Servicer.ShouldBeNull();
    }
}

public class AccountIdentificationTests
{
    [Fact]
    public void AccountIdentification_IsAbstract_CanBeEitherIbanOrOther()
    {
        // Arrange
        AccountIdentification ibanId = new IbanAccountIdentification
        {
            Iban = Iban.Parse("DE89370400440532013000")
        };

        AccountIdentification otherId = new OtherAccountIdentification
        {
            Other = new GenericAccountIdentification
            {
                Identification = "123456789"
            }
        };

        // Assert
        ibanId.ShouldBeOfType<IbanAccountIdentification>();
        otherId.ShouldBeOfType<OtherAccountIdentification>();
    }

    [Fact]
    public void IbanAccountIdentification_ShouldContainIban()
    {
        // Arrange & Act
        var ibanId = new IbanAccountIdentification
        {
            Iban = Iban.Parse("FR7630006000011234567890189")
        };

        // Assert
        ibanId.Iban.CountryCode.ShouldBe("FR");
        ibanId.Iban.CheckDigits.ShouldBe("76");
    }

    [Fact]
    public void OtherAccountIdentification_ShouldContainGenericIdentification()
    {
        // Arrange & Act
        var otherId = new OtherAccountIdentification
        {
            Other = new GenericAccountIdentification
            {
                Identification = "ACCT-12345-XYZ",
                SchemeName = new AccountSchemeName { Proprietary = "CUSTOM" },
                Issuer = "Bank of Test"
            }
        };

        // Assert
        otherId.Other.Identification.ShouldBe("ACCT-12345-XYZ");
        otherId.Other.SchemeName?.Proprietary.ShouldBe("CUSTOM");
        otherId.Other.Issuer.ShouldBe("Bank of Test");
    }
}

public class AccountTypeTests
{
    [Theory]
    [InlineData("CACC", "Current Account")]
    [InlineData("SVGS", "Savings Account")]
    [InlineData("LOAN", "Loan Account")]
    [InlineData("CASH", "Cash Payment")]
    [InlineData("CHAR", "Charges Account")]
    [InlineData("SACC", "Settlement Account")]
    public void AccountType_WithCode_ShouldStoreValue(string code, string description)
    {
        // Arrange & Act
        var accountType = new AccountType { Code = code };

        // Assert
        accountType.Code.ShouldBe(code);
        accountType.Proprietary.ShouldBeNull();
        _ = description; // For documentation
    }

    [Fact]
    public void AccountType_WithProprietary_ShouldStoreValue()
    {
        // Arrange & Act
        var accountType = new AccountType { Proprietary = "CUSTOM_ACCOUNT_TYPE" };

        // Assert
        accountType.Code.ShouldBeNull();
        accountType.Proprietary.ShouldBe("CUSTOM_ACCOUNT_TYPE");
    }

    [Fact]
    public void AccountType_Default_ShouldHaveNullProperties()
    {
        // Arrange & Act
        var accountType = new AccountType();

        // Assert
        accountType.Code.ShouldBeNull();
        accountType.Proprietary.ShouldBeNull();
    }
}

public class AccountSchemeNameTests
{
    [Theory]
    [InlineData("BBAN")]
    [InlineData("CUID")]
    [InlineData("UPIC")]
    public void AccountSchemeName_WithCode_ShouldStoreValue(string code)
    {
        // Arrange & Act
        var schemeName = new AccountSchemeName { Code = code };

        // Assert
        schemeName.Code.ShouldBe(code);
        schemeName.Proprietary.ShouldBeNull();
    }

    [Fact]
    public void AccountSchemeName_WithProprietary_ShouldStoreValue()
    {
        // Arrange & Act
        var schemeName = new AccountSchemeName { Proprietary = "INTERNAL_ID" };

        // Assert
        schemeName.Code.ShouldBeNull();
        schemeName.Proprietary.ShouldBe("INTERNAL_ID");
    }
}

public class GenericAccountIdentificationTests
{
    [Fact]
    public void GenericAccountIdentification_AllProperties_ShouldBeSet()
    {
        // Arrange & Act
        var genericId = new GenericAccountIdentification
        {
            Identification = "123-456-789",
            SchemeName = new AccountSchemeName { Code = "BBAN" },
            Issuer = "Central Bank"
        };

        // Assert
        genericId.Identification.ShouldBe("123-456-789");
        genericId.SchemeName.ShouldNotBeNull();
        genericId.SchemeName.Code.ShouldBe("BBAN");
        genericId.Issuer.ShouldBe("Central Bank");
    }

    [Fact]
    public void GenericAccountIdentification_MinimalProperties_ShouldWork()
    {
        // Arrange & Act
        var genericId = new GenericAccountIdentification
        {
            Identification = "ACCT123"
        };

        // Assert
        genericId.Identification.ShouldBe("ACCT123");
        genericId.SchemeName.ShouldBeNull();
        genericId.Issuer.ShouldBeNull();
    }
}

public class ProxyAccountIdentificationTests
{
    [Theory]
    [InlineData("TELE", "+49170123456789")]
    [InlineData("EMAL", "max@example.com")]
    [InlineData("DNAM", "maxmustermann")]
    public void ProxyAccountIdentification_WithTypeAndId_ShouldStoreValues(string typeCode, string identification)
    {
        // Arrange & Act
        var proxy = new ProxyAccountIdentification
        {
            Type = new ProxyAccountType { Code = typeCode },
            Identification = identification
        };

        // Assert
        proxy.Type?.Code.ShouldBe(typeCode);
        proxy.Identification.ShouldBe(identification);
    }

    [Fact]
    public void ProxyAccountIdentification_WithoutType_ShouldWork()
    {
        // Arrange & Act
        var proxy = new ProxyAccountIdentification
        {
            Identification = "some-alias-123"
        };

        // Assert
        proxy.Type.ShouldBeNull();
        proxy.Identification.ShouldBe("some-alias-123");
    }

    [Fact]
    public void ProxyAccountType_WithProprietary_ShouldStoreValue()
    {
        // Arrange & Act
        var proxy = new ProxyAccountIdentification
        {
            Type = new ProxyAccountType { Proprietary = "CUSTOM_PROXY" },
            Identification = "custom-value"
        };

        // Assert
        proxy.Type.Code.ShouldBeNull();
        proxy.Type.Proprietary.ShouldBe("CUSTOM_PROXY");
    }
}
