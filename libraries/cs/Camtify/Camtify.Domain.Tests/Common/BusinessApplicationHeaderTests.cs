using Camtify.Domain.Common;
using BusinessApplicationHeader = Camtify.Domain.Common.BusinessApplicationHeader;
using Party = Camtify.Domain.Common.Party;

namespace Camtify.Domain.Tests.Common;

public class BusinessApplicationHeaderTests
{
    [Fact]
    public void Constructor_RequiredProperties_ShouldBeSet()
    {
        // Arrange
        var from = new Party
        {
            FinancialInstitutionId = new FinancialInstitutionIdentification { Bic = "DEUTDEFF" }
        };
        var to = new Party
        {
            FinancialInstitutionId = new FinancialInstitutionIdentification { Bic = "COBADEFF" }
        };
        var creationDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var header = new BusinessApplicationHeader
        {
            Version = new Camtify.Core.MessageIdentifier("head.001.001.02"),
            From = from,
            To = to,
            BusinessMessageIdentifier = "MSG123456789",
            MessageDefinitionIdentifier = "pain.001.001.09",
            CreationDate = creationDate
        };

        // Assert
        header.Version.Value.ShouldBe("head.001.001.02");
        header.From.ShouldBe(from);
        header.To.ShouldBe(to);
        header.BusinessMessageIdentifier.ShouldBe("MSG123456789");
        header.MessageDefinitionIdentifier.ShouldBe("pain.001.001.09");
        header.CreationDate.ShouldBe(creationDate);
    }

    [Fact]
    public void OptionalProperties_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var header = CreateMinimalHeader();

        // Assert
        header.BusinessService.ShouldBeNull();
        header.CharacterSet.ShouldBeNull();
        header.CopyDuplicate.ShouldBeNull();
        header.PossibleDuplicate.ShouldBeNull();
        header.Priority.ShouldBeNull();
        header.Signature.ShouldBeNull();
        header.Related.ShouldBeNull();
    }

    [Fact]
    public void BusinessService_ShouldStoreValue()
    {
        // Arrange & Act
        var header = CreateMinimalHeader() with
        {
            BusinessService = "swift.cbprplus.02"
        };

        // Assert
        header.BusinessService.ShouldBe("swift.cbprplus.02");
    }

    [Fact]
    public void CopyDuplicate_ShouldStoreValue()
    {
        // Arrange & Act
        var copyHeader = CreateMinimalHeader() with
        {
            CopyDuplicate = Camtify.Domain.Common.CopyDuplicate.Copy
        };
        var duplicateHeader = CreateMinimalHeader() with
        {
            CopyDuplicate = Camtify.Domain.Common.CopyDuplicate.Duplicate
        };

        // Assert
        copyHeader.CopyDuplicate.ShouldBe(Camtify.Domain.Common.CopyDuplicate.Copy);
        duplicateHeader.CopyDuplicate.ShouldBe(Camtify.Domain.Common.CopyDuplicate.Duplicate);
    }

    [Fact]
    public void PossibleDuplicate_ShouldStoreValue()
    {
        // Arrange & Act
        var header = CreateMinimalHeader() with
        {
            PossibleDuplicate = true
        };

        // Assert
        header.PossibleDuplicate.ShouldBe(true);
    }

    [Fact]
    public void Priority_ShouldStoreValue()
    {
        // Arrange & Act
        var normalHeader = CreateMinimalHeader() with
        {
            Priority = Camtify.Domain.Common.Priority.Normal
        };
        var highHeader = CreateMinimalHeader() with
        {
            Priority = Camtify.Domain.Common.Priority.High
        };

        // Assert
        normalHeader.Priority.ShouldBe(Camtify.Domain.Common.Priority.Normal);
        highHeader.Priority.ShouldBe(Camtify.Domain.Common.Priority.High);
    }

    [Fact]
    public void Related_ShouldStoreRelatedHeader()
    {
        // Arrange
        var originalHeader = CreateMinimalHeader();

        // Act
        var responseHeader = CreateMinimalHeader() with
        {
            BusinessMessageIdentifier = "RESPONSE123",
            Related = originalHeader
        };

        // Assert
        responseHeader.Related.ShouldNotBeNull();
        responseHeader.Related.BusinessMessageIdentifier.ShouldBe("MSG123456789");
    }

    [Fact]
    public void CharacterSet_ShouldDefaultToNull()
    {
        // Arrange & Act
        var header = CreateMinimalHeader();

        // Assert
        header.CharacterSet.ShouldBeNull();
    }

    [Fact]
    public void CharacterSet_WhenSet_ShouldStoreValue()
    {
        // Arrange & Act
        var header = CreateMinimalHeader() with
        {
            CharacterSet = "UTF-8"
        };

        // Assert
        header.CharacterSet.ShouldBe("UTF-8");
    }

    [Fact]
    public void Version_ShouldBeValidMessageIdentifier()
    {
        // Arrange & Act
        var header = CreateMinimalHeader();

        // Assert
        header.Version.BusinessArea.ShouldBe("head");
        header.Version.MessageNumber.ShouldBe("001");
        header.Version.Variant.ShouldBe("001");
        header.Version.Version.ShouldBe("02");
    }

    [Theory]
    [InlineData("head.001.001.01")]
    [InlineData("head.001.001.02")]
    public void Version_DifferentVersions_ShouldBeSupported(string version)
    {
        // Arrange & Act
        var header = CreateMinimalHeader() with
        {
            Version = new Camtify.Core.MessageIdentifier(version)
        };

        // Assert
        header.Version.Value.ShouldBe(version);
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        // Arrange
        var header1 = CreateMinimalHeader();
        var header2 = CreateMinimalHeader();

        // Assert
        header1.ShouldBe(header2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var header1 = CreateMinimalHeader();
        var header2 = CreateMinimalHeader() with
        {
            BusinessMessageIdentifier = "DIFFERENT123"
        };

        // Assert
        header1.ShouldNotBe(header2);
    }

    private static BusinessApplicationHeader CreateMinimalHeader()
    {
        return new BusinessApplicationHeader
        {
            Version = new Camtify.Core.MessageIdentifier("head.001.001.02"),
            From = new Party
            {
                FinancialInstitutionId = new FinancialInstitutionIdentification { Bic = "DEUTDEFF" }
            },
            To = new Party
            {
                FinancialInstitutionId = new FinancialInstitutionIdentification { Bic = "COBADEFF" }
            },
            BusinessMessageIdentifier = "MSG123456789",
            MessageDefinitionIdentifier = "pain.001.001.09",
            CreationDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };
    }
}
