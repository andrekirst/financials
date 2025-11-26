using Camtify.Core;

namespace Camtify.Core.Tests;

public class MessageIdentifierTests
{
    [Theory]
    [InlineData("pain.001.001.09", "pain", "001", "001", "09")]
    [InlineData("camt.053.001.08", "camt", "053", "001", "08")]
    [InlineData("pacs.008.001.10", "pacs", "008", "001", "10")]
    [InlineData("head.001.001.02", "head", "001", "001", "02")]
    public void Constructor_ValidFormat_ShouldParseCorrectly(
        string value, string expectedArea, string expectedMessage, string expectedVariant, string expectedVersion)
    {
        // Act
        var identifier = new MessageIdentifier(value);

        // Assert
        identifier.Value.ShouldBe(value);
        identifier.BusinessArea.ShouldBe(expectedArea);
        identifier.MessageNumber.ShouldBe(expectedMessage);
        identifier.Variant.ShouldBe(expectedVariant);
        identifier.Version.ShouldBe(expectedVersion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhitespace_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new MessageIdentifier(value));
    }

    [Fact]
    public void Constructor_Null_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new MessageIdentifier(null!));
    }

    [Theory]
    [InlineData("pain")]
    [InlineData("pain.001")]
    [InlineData("pain.001.001")]
    [InlineData("pain.001.001.09.extra")]
    public void Constructor_InvalidFormat_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new MessageIdentifier(value));
        ex.Message.ShouldContain("Invalid message identifier format");
    }

    [Fact]
    public void Create_ShouldBuildCorrectIdentifier()
    {
        // Act
        var identifier = MessageIdentifier.Create("pain", "001", "001", "09");

        // Assert
        identifier.Value.ShouldBe("pain.001.001.09");
        identifier.BusinessArea.ShouldBe("pain");
        identifier.MessageNumber.ShouldBe("001");
        identifier.Variant.ShouldBe("001");
        identifier.Version.ShouldBe("09");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        string value = "camt.053.001.08";

        // Act
        MessageIdentifier identifier = value;

        // Assert
        identifier.Value.ShouldBe("camt.053.001.08");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var identifier = new MessageIdentifier("camt.053.001.08");

        // Act
        string value = identifier;

        // Assert
        value.ShouldBe("camt.053.001.08");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var identifier = new MessageIdentifier("pain.001.001.09");

        // Act
        var result = identifier.ToString();

        // Assert
        result.ShouldBe("pain.001.001.09");
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var id1 = new MessageIdentifier("pain.001.001.09");
        var id2 = new MessageIdentifier("pain.001.001.09");

        // Assert
        id1.ShouldBe(id2);
        (id1 == id2).ShouldBeTrue();
        (id1 != id2).ShouldBeFalse();
    }

    [Fact]
    public void Equality_DifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var id1 = new MessageIdentifier("pain.001.001.09");
        var id2 = new MessageIdentifier("pain.001.001.10");

        // Assert
        id1.ShouldNotBe(id2);
        (id1 == id2).ShouldBeFalse();
        (id1 != id2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValue_ShouldBeSame()
    {
        // Arrange
        var id1 = new MessageIdentifier("pain.001.001.09");
        var id2 = new MessageIdentifier("pain.001.001.09");

        // Assert
        id1.GetHashCode().ShouldBe(id2.GetHashCode());
    }

    [Theory]
    [InlineData("pain", "Customer Credit Transfer")]
    [InlineData("camt", "Cash Management")]
    [InlineData("pacs", "Payments Clearing and Settlement")]
    [InlineData("head", "Business Application Header")]
    public void BusinessArea_CommonAreas_ShouldParse(string area, string description)
    {
        // Arrange & Act
        var identifier = new MessageIdentifier($"{area}.001.001.01");

        // Assert
        identifier.BusinessArea.ShouldBe(area);
        _ = description; // Used for documentation in test output
    }
}
