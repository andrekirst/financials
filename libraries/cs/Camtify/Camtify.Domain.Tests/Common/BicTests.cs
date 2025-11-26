using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class BicTests
{
    [Theory]
    [InlineData("DEUTDEFF", "DEUT", "DE", "FF")]
    [InlineData("COBADEFF", "COBA", "DE", "FF")]
    [InlineData("BNPAFRPP", "BNPA", "FR", "PP")]
    public void Parse_ValidBic8_ShouldExtractComponents(
        string value, string expectedInstitution, string expectedCountry, string expectedLocation)
    {
        // Act
        var bic = Bic.Parse(value);

        // Assert
        bic.InstitutionCode.ShouldBe(expectedInstitution);
        bic.CountryCode.ShouldBe(expectedCountry);
        bic.LocationCode.ShouldBe(expectedLocation);
        bic.BranchCode.ShouldBeNull();
        bic.IsBic8.ShouldBeTrue();
        bic.IsBic11.ShouldBeFalse();
    }

    [Theory]
    [InlineData("DEUTDEFFXXX", "DEUT", "DE", "FF", "XXX")]
    [InlineData("COBADEFF001", "COBA", "DE", "FF", "001")]
    [InlineData("BNPAFRPP123", "BNPA", "FR", "PP", "123")]
    public void Parse_ValidBic11_ShouldExtractComponents(
        string value, string expectedInstitution, string expectedCountry, string expectedLocation, string expectedBranch)
    {
        // Act
        var bic = Bic.Parse(value);

        // Assert
        bic.InstitutionCode.ShouldBe(expectedInstitution);
        bic.CountryCode.ShouldBe(expectedCountry);
        bic.LocationCode.ShouldBe(expectedLocation);
        bic.BranchCode.ShouldBe(expectedBranch);
        bic.IsBic8.ShouldBeFalse();
        bic.IsBic11.ShouldBeTrue();
    }

    [Theory]
    [InlineData("deut de ff")]
    [InlineData("DEUT DE FF")]
    [InlineData("  DEUTDEFF  ")]
    public void Parse_WithSpacesOrLowerCase_ShouldNormalize(string value)
    {
        // Act
        var bic = Bic.Parse(value);

        // Assert
        bic.ToString().ShouldBe("DEUTDEFF");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Parse_EmptyOrNull_ShouldThrow(string? value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Bic.Parse(value!));
        ex.Message.ToLower().ShouldContain("empty");
    }

    [Theory]
    [InlineData("DEUT")] // Too short (4 chars)
    [InlineData("DEUTDE")] // Too short (6 chars)
    [InlineData("DEUTDEF")] // Too short (7 chars)
    [InlineData("DEUTDEFFXX")] // Invalid length (10 chars)
    [InlineData("DEUTDEFFXXXX")] // Too long (12 chars)
    public void Parse_InvalidLength_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Bic.Parse(value));
        ex.Message.ToLower().ShouldContain("length");
    }

    [Theory]
    [InlineData("1234DEFF")] // Institution code not letters
    [InlineData("DEUT12FF")] // Country code not letters
    public void Parse_InvalidFormat_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Bic.Parse(value));
        ex.Message.ToLower().ShouldContain("format");
    }

    [Fact]
    public void AsBic11_FromBic8_ShouldAppendXXX()
    {
        // Arrange
        var bic = Bic.Parse("DEUTDEFF");

        // Act
        var asBic11 = bic.AsBic11;

        // Assert
        asBic11.ShouldBe("DEUTDEFFXXX");
    }

    [Fact]
    public void AsBic11_FromBic11_ShouldReturnSame()
    {
        // Arrange
        var bic = Bic.Parse("DEUTDEFF001");

        // Act
        var asBic11 = bic.AsBic11;

        // Assert
        asBic11.ShouldBe("DEUTDEFF001");
    }

    [Fact]
    public void AsBic8_FromBic11WithXXX_ShouldRemoveXXX()
    {
        // Arrange
        var bic = Bic.Parse("DEUTDEFFXXX");

        // Act
        var asBic8 = bic.AsBic8;

        // Assert
        asBic8.ShouldBe("DEUTDEFF");
    }

    [Fact]
    public void AsBic8_FromBic11WithBranch_ShouldReturnFull()
    {
        // Arrange
        var bic = Bic.Parse("DEUTDEFF001");

        // Act
        var asBic8 = bic.AsBic8;

        // Assert
        asBic8.ShouldBe("DEUTDEFF001"); // Cannot reduce, has specific branch
    }

    [Fact]
    public void TryParse_ValidBic_ShouldSucceed()
    {
        // Act
        var success = Bic.TryParse("DEUTDEFF", out var bic, out var error);

        // Assert
        success.ShouldBeTrue();
        bic.ShouldNotBeNull();
        error.ShouldBeNull();
    }

    [Fact]
    public void TryParse_InvalidBic_ShouldFail()
    {
        // Act
        var success = Bic.TryParse("INVALID", out var bic, out var error);

        // Assert
        success.ShouldBeFalse();
        bic.ShouldBeNull();
        error.ShouldNotBeNull();
    }

    [Fact]
    public void TryParse_WithoutErrorOutput_ShouldWork()
    {
        // Act
        var success = Bic.TryParse("DEUTDEFF", out var bic);

        // Assert
        success.ShouldBeTrue();
        bic.ShouldNotBeNull();
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var bic = Bic.Parse("DEUTDEFF");

        // Act
        var result = bic.ToString();

        // Assert
        result.ShouldBe("DEUTDEFF");
    }

    [Fact]
    public void Equality_Bic8AndBic11WithXXX_ShouldBeEqual()
    {
        // Arrange
        var bic8 = Bic.Parse("DEUTDEFF");
        var bic11 = Bic.Parse("DEUTDEFFXXX");

        // Assert - They should be equal because XXX means head office
        bic8.ShouldBe(bic11);
        (bic8 == bic11).ShouldBeTrue();
        (bic8 != bic11).ShouldBeFalse();
    }

    [Fact]
    public void Equality_DifferentBranch_ShouldNotBeEqual()
    {
        // Arrange
        var bic1 = Bic.Parse("DEUTDEFFXXX");
        var bic2 = Bic.Parse("DEUTDEFF001");

        // Assert
        bic1.ShouldNotBe(bic2);
        (bic1 == bic2).ShouldBeFalse();
        (bic1 != bic2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentInstitution_ShouldNotBeEqual()
    {
        // Arrange
        var bic1 = Bic.Parse("DEUTDEFF");
        var bic2 = Bic.Parse("COBADEFF");

        // Assert
        bic1.ShouldNotBe(bic2);
    }

    [Fact]
    public void GetHashCode_Bic8AndBic11XXX_ShouldBeSame()
    {
        // Arrange
        var bic8 = Bic.Parse("DEUTDEFF");
        var bic11 = Bic.Parse("DEUTDEFFXXX");

        // Assert - Should be same because they compare as equal
        bic8.GetHashCode().ShouldBe(bic11.GetHashCode());
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var bic = Bic.Parse("DEUTDEFF");

        // Act
        string value = bic;

        // Assert
        value.ShouldBe("DEUTDEFF");
    }

    [Theory]
    [InlineData("DEUTDEFF")] // Deutsche Bank Germany
    [InlineData("COBADEFF")] // Commerzbank Germany
    [InlineData("BNPAFRPP")] // BNP Paribas France
    [InlineData("BARCGB22")] // Barclays UK
    [InlineData("CHASUS33")] // JP Morgan Chase USA
    [InlineData("UBSWCHZH")] // UBS Switzerland
    public void Parse_RealWorldBics_ShouldValidateCorrectly(string value)
    {
        // Act
        var bic = Bic.Parse(value);

        // Assert
        bic.ShouldNotBeNull();
        bic.InstitutionCode.Length.ShouldBe(4);
        bic.CountryCode.Length.ShouldBe(2);
        bic.LocationCode.Length.ShouldBe(2);
    }
}
