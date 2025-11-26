using Camtify.Core;

namespace Camtify.Core.Tests;

public class MessageIdentifierTests
{
    #region Constructor Tests

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
    [InlineData("PAIN.001.001.09", "pain")] // Upper case business area
    [InlineData("Pain.001.001.09", "pain")] // Mixed case business area
    [InlineData("CAMT.053.001.08", "camt")] // Upper case
    public void Constructor_CaseInsensitive_ShouldNormalize(string value, string expectedArea)
    {
        // Act
        var identifier = new MessageIdentifier(value);

        // Assert
        identifier.BusinessArea.ShouldBe(expectedArea);
        identifier.Value.ShouldStartWith(expectedArea + ".");
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
    [InlineData("abc.001.001.09")] // 3 letter business area
    [InlineData("abcde.001.001.09")] // 5 letter business area
    public void Constructor_InvalidFormat_ShouldThrow(string value)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new MessageIdentifier(value));
        ex.Message.ShouldContain("Invalid format");
    }

    #endregion

    #region Create Factory Method

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
    public void Create_UpperCaseBusinessArea_ShouldNormalize()
    {
        // Act
        var identifier = MessageIdentifier.Create("PAIN", "001", "001", "09");

        // Assert
        identifier.BusinessArea.ShouldBe("pain");
        identifier.Value.ShouldBe("pain.001.001.09");
    }

    #endregion

    #region Parse Method

    [Theory]
    [InlineData("pain.001.001.09")]
    [InlineData("PAIN.001.001.09")]
    [InlineData("Pain.001.001.09")]
    [InlineData("camt.053.001.08")]
    public void Parse_ValidFormat_ShouldSucceed(string value)
    {
        // Act
        var identifier = MessageIdentifier.Parse(value);

        // Assert
        identifier.Value.ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("pain.001")]
    public void Parse_InvalidFormat_ShouldThrowFormatException(string? value)
    {
        // Act & Assert
        Should.Throw<FormatException>(() => MessageIdentifier.Parse(value!));
    }

    #endregion

    #region TryParse Method

    [Theory]
    [InlineData("pain.001.001.09", true)]
    [InlineData("PAIN.001.001.09", true)]
    [InlineData("camt.053.001.08", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("invalid", false)]
    [InlineData("pain.001", false)]
    public void TryParse_ShouldReturnExpectedResult(string? value, bool expectedSuccess)
    {
        // Act
        var success = MessageIdentifier.TryParse(value, out var result, out var error);

        // Assert
        success.ShouldBe(expectedSuccess);
        if (expectedSuccess)
        {
            result.ShouldNotBeNull();
            error.ShouldBeNull();
        }
        else
        {
            result.ShouldBeNull();
            error.ShouldNotBeNull();
        }
    }

    [Fact]
    public void TryParse_InvalidFormat_ShouldReturnMeaningfulError()
    {
        // Act
        MessageIdentifier.TryParse("invalid", out _, out var error);

        // Assert
        error.ShouldContain("Invalid format");
        error.ShouldContain("xxxx.nnn.nnn.nn");
    }

    #endregion

    #region FromNamespace Method

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09", "pain.001.001.09")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:camt.053.001.08", "camt.053.001.08")]
    [InlineData("urn:swift:xsd:pain.001.001.09", "pain.001.001.09")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pacs.008.001.10$cbpr_plus", "pacs.008.001.10")]
    public void FromNamespace_ValidNamespace_ShouldExtractIdentifier(string @namespace, string expectedValue)
    {
        // Act
        var identifier = MessageIdentifier.FromNamespace(@namespace);

        // Assert
        identifier.Value.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid:namespace")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:invalid")]
    public void FromNamespace_InvalidNamespace_ShouldThrowFormatException(string? @namespace)
    {
        // Act & Assert
        Should.Throw<FormatException>(() => MessageIdentifier.FromNamespace(@namespace!));
    }

    #endregion

    #region TryFromNamespace Method

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09", true, "pain.001.001.09")]
    [InlineData("urn:swift:xsd:camt.053.001.08", true, "camt.053.001.08")]
    [InlineData("invalid", false, null)]
    [InlineData(null, false, null)]
    public void TryFromNamespace_ShouldReturnExpectedResult(string? @namespace, bool expectedSuccess, string? expectedValue)
    {
        // Act
        var success = MessageIdentifier.TryFromNamespace(@namespace, out var result, out var error);

        // Assert
        success.ShouldBe(expectedSuccess);
        if (expectedSuccess)
        {
            result.ShouldNotBeNull();
            result!.Value.Value.ShouldBe(expectedValue);
            error.ShouldBeNull();
        }
        else
        {
            result.ShouldBeNull();
            error.ShouldNotBeNull();
        }
    }

    #endregion

    #region ToNamespace Method

    [Theory]
    [InlineData("pain.001.001.09", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData("camt.053.001.08", "urn:iso:std:iso:20022:tech:xsd:camt.053.001.08")]
    public void ToNamespace_ShouldGenerateCorrectNamespace(string value, string expectedNamespace)
    {
        // Arrange
        var identifier = new MessageIdentifier(value);

        // Act
        var @namespace = identifier.ToNamespace();

        // Assert
        @namespace.ShouldBe(expectedNamespace);
    }

    [Fact]
    public void ToNamespace_FromNamespace_ShouldRoundTrip()
    {
        // Arrange
        var original = new MessageIdentifier("pain.001.001.09");

        // Act
        var @namespace = original.ToNamespace();
        var restored = MessageIdentifier.FromNamespace(@namespace);

        // Assert
        restored.ShouldBe(original);
    }

    #endregion

    #region VersionNumber Property

    [Theory]
    [InlineData("pain.001.001.09", 9)]
    [InlineData("pain.001.001.01", 1)]
    [InlineData("camt.053.001.10", 10)]
    [InlineData("pacs.008.001.12", 12)]
    public void VersionNumber_ShouldReturnCorrectInt(string value, int expectedVersion)
    {
        // Arrange
        var identifier = new MessageIdentifier(value);

        // Act & Assert
        identifier.VersionNumber.ShouldBe(expectedVersion);
    }

    #endregion

    #region ShortName Property

    [Theory]
    [InlineData("pain.001.001.09", "PAIN.001 v9")]
    [InlineData("camt.053.001.08", "CAMT.053 v8")]
    [InlineData("pacs.008.001.10", "PACS.008 v10")]
    public void ShortName_ShouldReturnCorrectFormat(string value, string expectedShortName)
    {
        // Arrange
        var identifier = new MessageIdentifier(value);

        // Act & Assert
        identifier.ShortName.ShouldBe(expectedShortName);
    }

    #endregion

    #region FullName Property

    [Fact]
    public void FullName_ShouldIncludeDescription()
    {
        // Arrange
        var identifier = new MessageIdentifier("pain.001.001.09");

        // Act
        var fullName = identifier.FullName;

        // Assert
        fullName.ShouldContain("PAIN.001 v9");
        fullName.ShouldContain("Customer Credit Transfer Initiation");
    }

    #endregion

    #region GetMessageDescription Method

    [Theory]
    [InlineData("pain.001.001.09", "Customer Credit Transfer Initiation")]
    [InlineData("pain.002.001.09", "Customer Payment Status Report")]
    [InlineData("pain.008.001.08", "Customer Direct Debit Initiation")]
    [InlineData("camt.052.001.08", "Bank To Customer Account Report")]
    [InlineData("camt.053.001.08", "Bank To Customer Statement")]
    [InlineData("camt.054.001.08", "Bank To Customer Debit Credit Notification")]
    [InlineData("pacs.002.001.10", "FI To FI Payment Status Report")]
    [InlineData("pacs.008.001.08", "FI To FI Customer Credit Transfer")]
    [InlineData("head.001.001.01", "Business Application Header")]
    public void GetMessageDescription_KnownMessages_ShouldReturnDescription(string value, string expectedDescription)
    {
        // Arrange
        var identifier = new MessageIdentifier(value);

        // Act
        var description = identifier.GetMessageDescription();

        // Assert
        description.ShouldBe(expectedDescription);
    }

    [Fact]
    public void GetMessageDescription_UnknownMessage_ShouldReturnUnknown()
    {
        // Arrange
        var identifier = new MessageIdentifier("xxxx.999.001.01");

        // Act
        var description = identifier.GetMessageDescription();

        // Assert
        description.ShouldBe("Unknown Message Type");
    }

    #endregion

    #region IsBusinessArea Method

    [Theory]
    [InlineData("pain.001.001.09", "pain", true)]
    [InlineData("pain.001.001.09", "PAIN", true)]
    [InlineData("pain.001.001.09", "Pain", true)]
    [InlineData("pain.001.001.09", "camt", false)]
    [InlineData("camt.053.001.08", "camt", true)]
    [InlineData("camt.053.001.08", "pain", false)]
    public void IsBusinessArea_ShouldReturnCorrectResult(string value, string area, bool expected)
    {
        // Arrange
        var identifier = new MessageIdentifier(value);

        // Act
        var result = identifier.IsBusinessArea(area);

        // Assert
        result.ShouldBe(expected);
    }

    #endregion

    #region IsNewerThan Method

    [Theory]
    [InlineData("pain.001.001.10", "pain.001.001.09", true)]
    [InlineData("pain.001.001.09", "pain.001.001.10", false)]
    [InlineData("pain.001.001.09", "pain.001.001.09", false)]
    public void IsNewerThan_SameMessageType_ShouldCompareVersions(string value1, string value2, bool expected)
    {
        // Arrange
        var identifier1 = new MessageIdentifier(value1);
        var identifier2 = new MessageIdentifier(value2);

        // Act
        var result = identifier1.IsNewerThan(identifier2);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void IsNewerThan_DifferentMessageTypes_ShouldThrow()
    {
        // Arrange
        var pain = new MessageIdentifier("pain.001.001.09");
        var camt = new MessageIdentifier("camt.053.001.08");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => pain.IsNewerThan(camt));
    }

    [Fact]
    public void IsNewerThan_DifferentMessageNumbers_ShouldThrow()
    {
        // Arrange
        var pain001 = new MessageIdentifier("pain.001.001.09");
        var pain002 = new MessageIdentifier("pain.002.001.09");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => pain001.IsNewerThan(pain002));
    }

    #endregion

    #region WithVersion Method

    [Theory]
    [InlineData("pain.001.001.09", 10, "pain.001.001.10")]
    [InlineData("camt.053.001.08", 12, "camt.053.001.12")]
    [InlineData("pacs.008.001.10", 1, "pacs.008.001.01")]
    public void WithVersion_ShouldCreateNewIdentifierWithVersion(string originalValue, int newVersion, string expectedValue)
    {
        // Arrange
        var original = new MessageIdentifier(originalValue);

        // Act
        var modified = original.WithVersion(newVersion);

        // Assert
        modified.Value.ShouldBe(expectedValue);
        modified.VersionNumber.ShouldBe(newVersion);
    }

    [Fact]
    public void WithVersion_ShouldNotModifyOriginal()
    {
        // Arrange
        var original = new MessageIdentifier("pain.001.001.09");

        // Act
        _ = original.WithVersion(10);

        // Assert
        original.Version.ShouldBe("09");
        original.VersionNumber.ShouldBe(9);
    }

    #endregion

    #region CompareTo Method

    [Fact]
    public void CompareTo_SameIdentifier_ShouldReturnZero()
    {
        // Arrange
        var id1 = new MessageIdentifier("pain.001.001.09");
        var id2 = new MessageIdentifier("pain.001.001.09");

        // Act & Assert
        id1.CompareTo(id2).ShouldBe(0);
    }

    [Fact]
    public void CompareTo_DifferentBusinessArea_ShouldSortAlphabetically()
    {
        // Arrange
        var camt = new MessageIdentifier("camt.053.001.08");
        var pain = new MessageIdentifier("pain.001.001.09");

        // Act & Assert
        camt.CompareTo(pain).ShouldBeLessThan(0);
        pain.CompareTo(camt).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void CompareTo_DifferentMessageNumber_ShouldSortNumerically()
    {
        // Arrange
        var pain001 = new MessageIdentifier("pain.001.001.09");
        var pain008 = new MessageIdentifier("pain.008.001.09");

        // Act & Assert
        pain001.CompareTo(pain008).ShouldBeLessThan(0);
        pain008.CompareTo(pain001).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void CompareTo_DifferentVersion_ShouldSortByVersion()
    {
        // Arrange
        var v9 = new MessageIdentifier("pain.001.001.09");
        var v10 = new MessageIdentifier("pain.001.001.10");

        // Act & Assert
        v9.CompareTo(v10).ShouldBeLessThan(0);
        v10.CompareTo(v9).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Sort_MultipleIdentifiers_ShouldSortCorrectly()
    {
        // Arrange
        var identifiers = new List<MessageIdentifier>
        {
            new("pacs.008.001.10"),
            new("camt.053.001.08"),
            new("pain.001.001.10"),
            new("pain.001.001.09"),
            new("camt.052.001.08")
        };

        // Act
        identifiers.Sort();

        // Assert
        identifiers[0].Value.ShouldBe("camt.052.001.08");
        identifiers[1].Value.ShouldBe("camt.053.001.08");
        identifiers[2].Value.ShouldBe("pacs.008.001.10");
        identifiers[3].Value.ShouldBe("pain.001.001.09");
        identifiers[4].Value.ShouldBe("pain.001.001.10");
    }

    #endregion

    #region Static Instances

    [Fact]
    public void Pain_StaticInstances_ShouldHaveCorrectValues()
    {
        MessageIdentifier.Pain.V001_09.Value.ShouldBe("pain.001.001.09");
        MessageIdentifier.Pain.V001_10.Value.ShouldBe("pain.001.001.10");
        MessageIdentifier.Pain.V001_11.Value.ShouldBe("pain.001.001.11");
        MessageIdentifier.Pain.V002_09.Value.ShouldBe("pain.002.001.09");
        MessageIdentifier.Pain.V002_10.Value.ShouldBe("pain.002.001.10");
        MessageIdentifier.Pain.V008_08.Value.ShouldBe("pain.008.001.08");
        MessageIdentifier.Pain.V008_09.Value.ShouldBe("pain.008.001.09");
    }

    [Fact]
    public void Camt_StaticInstances_ShouldHaveCorrectValues()
    {
        MessageIdentifier.Camt.V052_08.Value.ShouldBe("camt.052.001.08");
        MessageIdentifier.Camt.V052_10.Value.ShouldBe("camt.052.001.10");
        MessageIdentifier.Camt.V053_08.Value.ShouldBe("camt.053.001.08");
        MessageIdentifier.Camt.V053_10.Value.ShouldBe("camt.053.001.10");
        MessageIdentifier.Camt.V054_08.Value.ShouldBe("camt.054.001.08");
        MessageIdentifier.Camt.V054_10.Value.ShouldBe("camt.054.001.10");
    }

    [Fact]
    public void Pacs_StaticInstances_ShouldHaveCorrectValues()
    {
        MessageIdentifier.Pacs.V002_10.Value.ShouldBe("pacs.002.001.10");
        MessageIdentifier.Pacs.V002_11.Value.ShouldBe("pacs.002.001.11");
        MessageIdentifier.Pacs.V008_08.Value.ShouldBe("pacs.008.001.08");
        MessageIdentifier.Pacs.V008_10.Value.ShouldBe("pacs.008.001.10");
    }

    [Fact]
    public void Head_StaticInstances_ShouldHaveCorrectValues()
    {
        MessageIdentifier.Head.V001_01.Value.ShouldBe("head.001.001.01");
        MessageIdentifier.Head.V001_02.Value.ShouldBe("head.001.001.02");
        MessageIdentifier.Head.V001_03.Value.ShouldBe("head.001.001.03");
    }

    [Fact]
    public void StaticInstances_ShouldBeEqualToConstructedInstances()
    {
        // Arrange
        var constructed = new MessageIdentifier("pain.001.001.09");

        // Assert
        MessageIdentifier.Pain.V001_09.ShouldBe(constructed);
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectDescriptions()
    {
        MessageIdentifier.Pain.V001_09.GetMessageDescription().ShouldBe("Customer Credit Transfer Initiation");
        MessageIdentifier.Camt.V053_08.GetMessageDescription().ShouldBe("Bank To Customer Statement");
        MessageIdentifier.Pacs.V008_08.GetMessageDescription().ShouldBe("FI To FI Customer Credit Transfer");
        MessageIdentifier.Head.V001_01.GetMessageDescription().ShouldBe("Business Application Header");
    }

    #endregion

    #region Implicit Conversions

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

    #endregion

    #region Equality and GetHashCode

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
    public void Equality_DifferentCase_ShouldBeEqual()
    {
        // Arrange
        var lower = new MessageIdentifier("pain.001.001.09");
        var upper = new MessageIdentifier("PAIN.001.001.09");

        // Assert - business area should be normalized
        lower.ShouldBe(upper);
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

    [Fact]
    public void GetHashCode_DifferentCase_ShouldBeSame()
    {
        // Arrange
        var lower = new MessageIdentifier("pain.001.001.09");
        var upper = new MessageIdentifier("PAIN.001.001.09");

        // Assert
        lower.GetHashCode().ShouldBe(upper.GetHashCode());
    }

    #endregion

    #region ToString

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

    #endregion

    #region Immutability

    [Fact]
    public void MessageIdentifier_ShouldBeImmutable()
    {
        // Arrange
        var identifier = new MessageIdentifier("pain.001.001.09");

        // Act - try to get a modified version
        var withNewVersion = identifier.WithVersion(10);

        // Assert - original should be unchanged
        identifier.Version.ShouldBe("09");
        withNewVersion.Version.ShouldBe("10");
    }

    [Fact]
    public void MessageIdentifier_ShouldBeRecordStruct()
    {
        // Assert
        typeof(MessageIdentifier).IsValueType.ShouldBeTrue();
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void RealWorld_ParseSepaNamespace_ShouldWork()
    {
        // Arrange - typical SEPA payment namespace
        var @namespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09";

        // Act
        var identifier = MessageIdentifier.FromNamespace(@namespace);

        // Assert
        identifier.BusinessArea.ShouldBe("pain");
        identifier.GetMessageDescription().ShouldBe("Customer Credit Transfer Initiation");
        identifier.ShortName.ShouldBe("PAIN.001 v9");
    }

    [Fact]
    public void RealWorld_CompareBankStatementVersions_ShouldWork()
    {
        // Arrange - compare camt.053 versions
        var v8 = MessageIdentifier.Camt.V053_08;
        var v10 = MessageIdentifier.Camt.V053_10;

        // Assert
        v10.IsNewerThan(v8).ShouldBeTrue();
        v8.IsNewerThan(v10).ShouldBeFalse();
    }

    [Fact]
    public void RealWorld_GroupMessagesByBusinessArea_ShouldWork()
    {
        // Arrange
        var messages = new[]
        {
            MessageIdentifier.Pain.V001_09,
            MessageIdentifier.Camt.V053_08,
            MessageIdentifier.Pain.V008_08,
            MessageIdentifier.Pacs.V008_08,
            MessageIdentifier.Camt.V054_08
        };

        // Act
        var grouped = messages.GroupBy(m => m.BusinessArea).ToDictionary(g => g.Key, g => g.Count());

        // Assert
        grouped["pain"].ShouldBe(2);
        grouped["camt"].ShouldBe(2);
        grouped["pacs"].ShouldBe(1);
    }

    #endregion
}
