using Camtify.Domain.Common;
using Camtify.Infrastructure.Extensions;

namespace Camtify.Domain.Tests.Common;

public class TransactionStatusTests
{
    [Theory]
    [InlineData(TransactionStatus.AcceptedCustomerProfile, "ACCP")]
    [InlineData(TransactionStatus.AcceptedSettlementCompleted, "ACSC")]
    [InlineData(TransactionStatus.AcceptedSettlementInProcess, "ACSP")]
    [InlineData(TransactionStatus.AcceptedTechnicalValidation, "ACTC")]
    [InlineData(TransactionStatus.AcceptedWithChange, "ACWC")]
    [InlineData(TransactionStatus.AcceptedWithoutPosting, "ACWP")]
    [InlineData(TransactionStatus.Received, "RCVD")]
    [InlineData(TransactionStatus.Pending, "PDNG")]
    [InlineData(TransactionStatus.Rejected, "RJCT")]
    [InlineData(TransactionStatus.Cancelled, "CANC")]
    [InlineData(TransactionStatus.AcceptedFundsChecked, "ACFC")]
    [InlineData(TransactionStatus.PartiallyAccepted, "PART")]
    public void ToIso20022Code_ShouldReturnCorrectCode(TransactionStatus status, string expectedCode)
    {
        // Act
        var code = status.ToIso20022Code();

        // Assert
        code.ShouldBe(expectedCode);
    }

    [Theory]
    [InlineData("ACCP", TransactionStatus.AcceptedCustomerProfile)]
    [InlineData("ACSC", TransactionStatus.AcceptedSettlementCompleted)]
    [InlineData("ACSP", TransactionStatus.AcceptedSettlementInProcess)]
    [InlineData("ACTC", TransactionStatus.AcceptedTechnicalValidation)]
    [InlineData("ACWC", TransactionStatus.AcceptedWithChange)]
    [InlineData("ACWP", TransactionStatus.AcceptedWithoutPosting)]
    [InlineData("RCVD", TransactionStatus.Received)]
    [InlineData("PDNG", TransactionStatus.Pending)]
    [InlineData("RJCT", TransactionStatus.Rejected)]
    [InlineData("CANC", TransactionStatus.Cancelled)]
    [InlineData("ACFC", TransactionStatus.AcceptedFundsChecked)]
    [InlineData("PART", TransactionStatus.PartiallyAccepted)]
    public void ParseTransactionStatus_ValidCode_ShouldReturnCorrectEnum(string code, TransactionStatus expected)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseTransactionStatus(code);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("accp")]
    [InlineData("Accp")]
    [InlineData("ACCP")]
    public void ParseTransactionStatus_CaseInsensitive_ShouldWork(string code)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseTransactionStatus(code);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(TransactionStatus.AcceptedCustomerProfile);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseTransactionStatus_EmptyCode_ShouldReturnNull(string? code)
    {
        // Act
        var result = Iso20022EnumExtensions.ParseTransactionStatus(code);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void ParseTransactionStatus_UnknownCode_ShouldReturnNull()
    {
        // Act
        var result = Iso20022EnumExtensions.ParseTransactionStatus("UNKNOWN");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void TransactionStatus_AllValues_ShouldHaveDescriptionAttribute()
    {
        // Arrange
        var allStatuses = Enum.GetValues<TransactionStatus>();

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var code = status.ToIso20022Code();
            code.ShouldNotBeNullOrWhiteSpace();
            code.Length.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public void TransactionStatus_RoundTrip_ShouldWork()
    {
        // Arrange
        var original = TransactionStatus.AcceptedSettlementCompleted;

        // Act
        var code = original.ToIso20022Code();
        var parsed = Iso20022EnumExtensions.ParseTransactionStatus(code);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.Value.ShouldBe(original);
    }
}
