using Camtify.Domain.Common;

namespace Camtify.Domain.Tests.Common;

public class CreditorReferenceInformationTests
{
    #region RF Reference Validation (ISO 11649)

    [Theory]
    [InlineData("RF18539007547034")] // Known valid Belgian RF reference
    public void ValidateRfReference_ValidRfReference_ShouldReturnTrue(string rfReference)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeTrue($"RF reference '{rfReference}' should be valid");
    }

    [Theory]
    [InlineData("RF01123456")] // Check digit 01 is invalid for this reference
    [InlineData("RF02123456")] // Check digit 02 is invalid
    [InlineData("RF55INVALID")] // Invalid checksum
    public void ValidateRfReference_InvalidCheckDigits_ShouldReturnFalse(string rfReference)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeFalse($"RF reference '{rfReference}' should be invalid");
    }

    [Theory]
    [InlineData("XX18539007547034")] // Doesn't start with RF
    [InlineData("ABC123")] // Not an RF reference
    [InlineData("123456")] // Just numbers
    public void ValidateRfReference_NotRfFormat_ShouldReturnFalse(string reference)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForReference(reference);

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void ValidateRfReference_NullReference_ShouldReturnFalse()
    {
        // Arrange
        var creditorRef = new CreditorReferenceInformation();

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Theory]
    [InlineData("RF")] // Too short
    [InlineData("RF18")] // Too short - only check digits, no reference
    public void ValidateRfReference_TooShort_ShouldReturnFalse(string rfReference)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Theory]
    [InlineData("RFAB123456")] // Non-numeric check digits
    [InlineData("RF1X123456")] // Mixed check digits
    public void ValidateRfReference_NonNumericCheckDigits_ShouldReturnFalse(string rfReference)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeFalse();
    }

    #endregion

    #region RF Reference Generation

    [Theory]
    [InlineData("539007547034")]
    [InlineData("ABCD")]
    [InlineData("N324")]
    [InlineData("INV2024001")]
    [InlineData("12345")]
    public void GenerateRfReference_ValidBaseReference_ShouldGenerateValidRf(string baseReference)
    {
        // Act
        var rfReference = CreditorReferenceInformation.GenerateRfReference(baseReference);

        // Assert - generated RF should start with RF and validate
        rfReference.ShouldStartWith("RF");
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);
        creditorRef.ValidateRfReference().ShouldBeTrue($"Generated RF '{rfReference}' should be valid");
    }

    [Fact]
    public void GenerateRfReference_GeneratedReference_ShouldBeValid()
    {
        // Arrange
        var baseReference = "INV2024001";

        // Act
        var rfReference = CreditorReferenceInformation.GenerateRfReference(baseReference);

        // Assert
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);
        creditorRef.ValidateRfReference().ShouldBeTrue();
        rfReference.ShouldStartWith("RF");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GenerateRfReference_EmptyBaseReference_ShouldThrow(string? baseReference)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            CreditorReferenceInformation.GenerateRfReference(baseReference!));
    }

    [Fact]
    public void GenerateRfReference_TooLongBaseReference_ShouldThrow()
    {
        // Arrange - RF reference max 25 chars total (RF + 2 check digits + max 21 chars)
        var tooLong = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 26 chars

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            CreditorReferenceInformation.GenerateRfReference(tooLong));
    }

    [Fact]
    public void GenerateRfReference_WithSpecialCharacters_ShouldStripThem()
    {
        // Arrange
        var baseWithSpecials = "INV-2024/001";

        // Act
        var rfReference = CreditorReferenceInformation.GenerateRfReference(baseWithSpecials);

        // Assert
        rfReference.ShouldNotContain("-");
        rfReference.ShouldNotContain("/");
        rfReference.ShouldStartWith("RF");

        // Verify the generated reference is valid
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);
        creditorRef.ValidateRfReference().ShouldBeTrue();
    }

    #endregion

    #region Validation Result

    [Fact]
    public void GetValidationResult_ValidRfReference_ShouldReturnSuccess()
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference("RF18539007547034");

        // Act
        var (isValid, errorMessage) = creditorRef.GetValidationResult();

        // Assert
        isValid.ShouldBeTrue();
        errorMessage.ShouldBeNull();
    }

    [Fact]
    public void GetValidationResult_InvalidChecksum_ShouldReturnError()
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference("RF12INVALID");

        // Act
        var (isValid, errorMessage) = creditorRef.GetValidationResult();

        // Assert
        isValid.ShouldBeFalse();
        errorMessage.ShouldNotBeNull();
        errorMessage.ShouldContain("checksum");
    }

    [Fact]
    public void GetValidationResult_NotStartingWithRF_ShouldReturnError()
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForReference("XX18539007547034");

        // Act
        var (isValid, errorMessage) = creditorRef.GetValidationResult();

        // Assert
        isValid.ShouldBeFalse();
        errorMessage.ShouldContain("RF");
    }

    [Fact]
    public void GetValidationResult_NullReference_ShouldReturnError()
    {
        // Arrange
        var creditorRef = new CreditorReferenceInformation();

        // Act
        var (isValid, errorMessage) = creditorRef.GetValidationResult();

        // Assert
        isValid.ShouldBeFalse();
        errorMessage.ShouldContain("null");
    }

    #endregion

    #region IsStructuredCreditorReference

    [Theory]
    [InlineData("RF18539007547034", true)]
    [InlineData("rf18539007547034", true)] // Case insensitive
    [InlineData("Rf18539007547034", true)]
    [InlineData("XX18539007547034", false)]
    [InlineData("123456", false)]
    public void IsStructuredCreditorReference_ShouldDetectRfFormat(string reference, bool expected)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForReference(reference);

        // Assert
        creditorRef.IsStructuredCreditorReference.ShouldBe(expected);
    }

    #endregion

    #region Factory Methods

    [Fact]
    public void ForRfReference_ShouldCreateWithScorType()
    {
        // Act
        var creditorRef = CreditorReferenceInformation.ForRfReference("RF18539007547034");

        // Assert
        creditorRef.Reference.ShouldBe("RF18539007547034");
        creditorRef.Type.ShouldNotBeNull();
        creditorRef.Type.Value.TypeCode.ShouldBe(ReferenceTypeCodes.SCOR);
    }

    [Fact]
    public void ForReference_ShouldCreateWithGivenReference()
    {
        // Act
        var creditorRef = CreditorReferenceInformation.ForReference("INV-2024-001");

        // Assert
        creditorRef.Reference.ShouldBe("INV-2024-001");
        creditorRef.Type.ShouldBeNull();
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_ValidRfReference_ShouldShowValid()
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference("RF18539007547034");

        // Act
        var result = creditorRef.ToString();

        // Assert
        result.ShouldContain("RF18539007547034");
        result.ShouldContain("Valid");
    }

    [Fact]
    public void ToString_NonRfReference_ShouldShowStandard()
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForReference("INV-2024-001");

        // Act
        var result = creditorRef.ToString();

        // Assert
        result.ShouldContain("INV-2024-001");
        result.ShouldContain("Standard");
    }

    #endregion

    #region Real-World RF Reference Examples

    [Theory]
    [InlineData("RF18539007547034")] // Belgian structured reference converted
    public void ValidateRfReference_RealWorldExamples_ShouldBeValid(string rfReference)
    {
        // Arrange
        var creditorRef = CreditorReferenceInformation.ForRfReference(rfReference);

        // Act
        var isValid = creditorRef.ValidateRfReference();

        // Assert
        isValid.ShouldBeTrue($"Real-world RF reference '{rfReference}' should be valid");
    }

    #endregion
}
