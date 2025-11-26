using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class IbanTests
{
    [Theory]
    [InlineData("DE89370400440532013000", "DE", "89", "370400440532013000")]
    [InlineData("FR7630006000011234567890189", "FR", "76", "30006000011234567890189")]
    [InlineData("GB82WEST12345698765432", "GB", "82", "WEST12345698765432")]
    [InlineData("AT611904300234573201", "AT", "61", "1904300234573201")]
    public void Parse_ValidIban_ShouldExtractComponents(
        string value, string expectedCountry, string expectedCheckDigits, string expectedBban)
    {
        // Act
        var iban = Iban.Parse(value);

        // Assert
        iban.CountryCode.ShouldBe(expectedCountry);
        iban.CheckDigits.ShouldBe(expectedCheckDigits);
        iban.Bban.ShouldBe(expectedBban);
    }

    [Theory]
    [InlineData("DE89 3704 0044 0532 0130 00")]
    [InlineData("DE89-3704-0044-0532-0130-00")]
    [InlineData("de89370400440532013000")]
    [InlineData("  DE89370400440532013000  ")]
    public void Parse_WithSpacesOrDashesOrLowerCase_ShouldNormalize(string value)
    {
        // Act
        var iban = Iban.Parse(value);

        // Assert
        iban.Electronic.ShouldBe("DE89370400440532013000");
        iban.CountryCode.ShouldBe("DE");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Parse_EmptyOrNull_ShouldThrow(string? value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Iban.Parse(value!));
        ex.Message.ShouldContain("empty");
    }

    [Theory]
    [InlineData("DE12345")] // Too short
    [InlineData("DE1234567890123456789012345678901234567")] // Too long (35+ chars)
    public void Parse_InvalidLength_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Iban.Parse(value));
        ex.Message.ShouldContain("length");
    }

    [Theory]
    [InlineData("12DE370400440532013000")] // Country code not at start
    [InlineData("DEDE370400440532013000")] // Check digits not numeric
    [InlineData("D189370400440532013000")] // Single letter country
    public void Parse_InvalidFormat_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Iban.Parse(value));
        ex.Message.ToLower().ShouldContain("format");
    }

    [Theory]
    [InlineData("DE00370400440532013000")] // Invalid check digits
    [InlineData("DE12370400440532013000")] // Invalid check digits
    public void Parse_InvalidChecksum_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Iban.Parse(value));
        ex.Message.ToLower().ShouldContain("checksum");
    }

    [Fact]
    public void Formatted_ShouldReturnGroupsOfFour()
    {
        // Arrange
        var iban = Iban.Parse("DE89370400440532013000");

        // Act
        var formatted = iban.Formatted;

        // Assert
        formatted.ShouldBe("DE89 3704 0044 0532 0130 00");
    }

    [Fact]
    public void Electronic_ShouldReturnWithoutSpaces()
    {
        // Arrange
        var iban = Iban.Parse("DE89 3704 0044 0532 0130 00");

        // Act
        var electronic = iban.Electronic;

        // Assert
        electronic.ShouldBe("DE89370400440532013000");
    }

    [Fact]
    public void Length_ShouldReturnCorrectValue()
    {
        // Arrange
        var germanIban = Iban.Parse("DE89370400440532013000");
        var frenchIban = Iban.Parse("FR7630006000011234567890189");

        // Assert
        germanIban.Length.ShouldBe(22);
        frenchIban.Length.ShouldBe(27);
    }

    [Fact]
    public void TryParse_ValidIban_ShouldSucceed()
    {
        // Act
        var success = Iban.TryParse("DE89370400440532013000", out var iban, out var error);

        // Assert
        success.ShouldBeTrue();
        iban.ShouldNotBeNull();
        error.ShouldBeNull();
    }

    [Fact]
    public void TryParse_InvalidIban_ShouldFail()
    {
        // Act
        var success = Iban.TryParse("INVALID", out var iban, out var error);

        // Assert
        success.ShouldBeFalse();
        iban.ShouldBeNull();
        error.ShouldNotBeNull();
    }

    [Fact]
    public void TryParse_WithoutErrorOutput_ShouldWork()
    {
        // Act
        var success = Iban.TryParse("DE89370400440532013000", out var iban);

        // Assert
        success.ShouldBeTrue();
        iban.ShouldNotBeNull();
    }

    [Fact]
    public void ToString_ShouldReturnElectronicFormat()
    {
        // Arrange
        var iban = Iban.Parse("DE89 3704 0044 0532 0130 00");

        // Act
        var result = iban.ToString();

        // Assert
        result.ShouldBe("DE89370400440532013000");
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var iban1 = Iban.Parse("DE89370400440532013000");
        var iban2 = Iban.Parse("DE89 3704 0044 0532 0130 00");

        // Assert
        iban1.ShouldBe(iban2);
        (iban1 == iban2).ShouldBeTrue();
        (iban1 != iban2).ShouldBeFalse();
    }

    [Fact]
    public void Equality_DifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var iban1 = Iban.Parse("DE89370400440532013000");
        var iban2 = Iban.Parse("FR7630006000011234567890189");

        // Assert
        iban1.ShouldNotBe(iban2);
        (iban1 == iban2).ShouldBeFalse();
        (iban1 != iban2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValue_ShouldBeSame()
    {
        // Arrange
        var iban1 = Iban.Parse("DE89370400440532013000");
        var iban2 = Iban.Parse("DE89370400440532013000");

        // Assert
        iban1.GetHashCode().ShouldBe(iban2.GetHashCode());
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var iban = Iban.Parse("DE89370400440532013000");

        // Act
        string value = iban;

        // Assert
        value.ShouldBe("DE89370400440532013000");
    }

    [Theory]
    [InlineData("DE89370400440532013000")] // Germany
    [InlineData("AT611904300234573201")] // Austria
    [InlineData("CH9300762011623852957")] // Switzerland
    [InlineData("FR7630006000011234567890189")] // France
    [InlineData("GB82WEST12345698765432")] // UK
    [InlineData("ES9121000418450200051332")] // Spain
    [InlineData("IT60X0542811101000000123456")] // Italy
    [InlineData("NL91ABNA0417164300")] // Netherlands
    [InlineData("BE68539007547034")] // Belgium
    [InlineData("LU280019400644750000")] // Luxembourg
    public void Parse_VariousCountries_ShouldValidateCorrectly(string value)
    {
        // Act
        var iban = Iban.Parse(value);

        // Assert
        iban.ShouldNotBeNull();
        iban.CountryCode.Length.ShouldBe(2);
    }
}
