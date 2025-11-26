using Camtify.Domain.Common;
using Camtify.Infrastructure.Extensions;

namespace Camtify.Domain.Tests.Common;

public class CreditDebitIndicatorTests
{
    [Theory]
    [InlineData(CreditDebitIndicator.Credit, "CRDT")]
    [InlineData(CreditDebitIndicator.Debit, "DBIT")]
    public void ToIso20022Code_ShouldReturnCorrectCode(CreditDebitIndicator indicator, string expectedCode)
    {
        // Act
        var code = indicator.ToIso20022Code();

        // Assert
        code.ShouldBe(expectedCode);
    }

    [Theory]
    [InlineData("CRDT", CreditDebitIndicator.Credit)]
    [InlineData("DBIT", CreditDebitIndicator.Debit)]
    public void ParseCreditDebitIndicator_ValidCode_ShouldReturnCorrectEnum(string code, CreditDebitIndicator expected)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseCreditDebitIndicator(code);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("crdt")]
    [InlineData("Crdt")]
    [InlineData("CRDT")]
    public void ParseCreditDebitIndicator_CaseInsensitive_ShouldWork(string code)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseCreditDebitIndicator(code);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(CreditDebitIndicator.Credit);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseCreditDebitIndicator_EmptyCode_ShouldReturnNull(string? code)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseCreditDebitIndicator(code);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void ParseCreditDebitIndicator_UnknownCode_ShouldReturnNull()
    {
        // Act
        var result = Iso20022EnumExtensions.ParseCreditDebitIndicator("UNKNOWN");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void CreditDebitIndicator_AllValues_ShouldHaveDescriptionAttribute()
    {
        // Arrange
        var allIndicators = Enum.GetValues<CreditDebitIndicator>();

        // Act & Assert
        foreach (var indicator in allIndicators)
        {
            var code = indicator.ToIso20022Code();
            code.ShouldNotBeNullOrWhiteSpace();
            code.Length.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public void CreditDebitIndicator_RoundTrip_ShouldWork()
    {
        // Arrange
        var original = CreditDebitIndicator.Credit;

        // Act
        var code = original.ToIso20022Code();
        var parsed = Iso20022EnumExtensions.ParseCreditDebitIndicator(code);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.Value.ShouldBe(original);
    }

    [Fact]
    public void GetDescription_ShouldReturnIsoCode()
    {
        // Act
        var creditDesc = CreditDebitIndicator.Credit.GetDescription();
        var debitDesc = CreditDebitIndicator.Debit.GetDescription();

        // Assert
        creditDesc.ShouldBe("CRDT");
        debitDesc.ShouldBe("DBIT");
    }
}
