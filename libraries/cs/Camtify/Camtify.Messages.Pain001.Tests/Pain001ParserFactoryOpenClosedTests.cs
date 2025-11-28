using System.Reflection;
using Camtify.Messages.Pain001.Parsers;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for Pain001ParserFactory Open-Closed-Principle compliance.
/// </summary>
/// <remarks>
/// These tests verify that the factory uses reflection-based parser discovery,
/// enabling new parsers to be added without modifying the factory code.
/// </remarks>
public sealed class Pain001ParserFactoryOpenClosedTests
{
    [Fact]
    public void ParserDiscovery_FindsAllParserImplementations()
    {
        // Arrange
        var expectedParserTypes = new[]
        {
            typeof(Pain001v03Parser),
            typeof(Pain001v08Parser),
            typeof(Pain001v09Parser),
            typeof(Pain001v10Parser),
            typeof(Pain001v11Parser)
        };

        // Act
        var supportedVersions = Pain001ParserFactory.GetSupportedVersions();

        // Assert
        supportedVersions.ShouldNotBeNull();
        supportedVersions.Count.ShouldBe(expectedParserTypes.Length);

        foreach (var parserType in expectedParserTypes)
        {
            var attribute = parserType.GetCustomAttribute<Pain001VersionAttribute>();
            attribute.ShouldNotBeNull($"{parserType.Name} should have Pain001VersionAttribute");
            supportedVersions.ShouldContain(attribute.Version, $"Factory should discover {parserType.Name}");
        }
    }

    [Theory]
    [InlineData(typeof(Pain001v03Parser), "003", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03")]
    [InlineData(typeof(Pain001v08Parser), "008", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.08")]
    [InlineData(typeof(Pain001v09Parser), "009", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData(typeof(Pain001v10Parser), "010", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.10")]
    [InlineData(typeof(Pain001v11Parser), "011", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.11")]
    public void ParserAttributes_ContainCorrectMetadata(Type parserType, string expectedVersion, string expectedNamespace)
    {
        // Act
        var attribute = parserType.GetCustomAttribute<Pain001VersionAttribute>();

        // Assert
        attribute.ShouldNotBeNull($"{parserType.Name} should have Pain001VersionAttribute");
        attribute.Version.ShouldBe(expectedVersion, $"{parserType.Name} version should match");
        attribute.Namespace.ShouldBe(expectedNamespace, $"{parserType.Name} namespace should match");
    }

    [Theory]
    [InlineData("003", typeof(Pain001v03Parser))]
    [InlineData("008", typeof(Pain001v08Parser))]
    [InlineData("009", typeof(Pain001v09Parser))]
    [InlineData("010", typeof(Pain001v10Parser))]
    [InlineData("011", typeof(Pain001v11Parser))]
    public void CreateForVersion_CreatesCorrectParserType(string version, Type expectedParserType)
    {
        // Arrange
        var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.{version}"">
  <CstmrCdtTrfInitn>
    <GrpHdr>
      <MsgId>TEST</MsgId>
      <CreDtTm>2023-11-27T14:30:00Z</CreDtTm>
      <NbOfTxs>0</NbOfTxs>
      <InitgPty><Nm>Test</Nm></InitgPty>
    </GrpHdr>
  </CstmrCdtTrfInitn>
</Document>";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act
        var parser = Pain001ParserFactory.CreateForVersion(version, stream);

        // Assert
        parser.ShouldNotBeNull();
        parser.ShouldBeOfType(expectedParserType);
        parser.Version.ShouldBe(version);
    }

    [Fact]
    public void CreateForVersion_UnsupportedVersion_ThrowsNotSupportedException()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.99"">
  <CstmrCdtTrfInitn>
    <GrpHdr>
      <MsgId>TEST</MsgId>
      <CreDtTm>2023-11-27T14:30:00Z</CreDtTm>
      <NbOfTxs>0</NbOfTxs>
      <InitgPty><Nm>Test</Nm></InitgPty>
    </GrpHdr>
  </CstmrCdtTrfInitn>
</Document>";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act & Assert
        Should.Throw<NotSupportedException>(() => Pain001ParserFactory.CreateForVersion("999", stream));
    }

    [Fact]
    public void CreateForVersion_NullVersion_ThrowsArgumentNullException()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Pain001ParserFactory.CreateForVersion(null!, stream));
    }

    [Fact]
    public void CreateForVersion_NullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Pain001ParserFactory.CreateForVersion("009", null!));
    }

    [Fact]
    public void GetSupportedNamespaces_ReturnsAllNamespaces()
    {
        // Arrange
        var expectedNamespaces = new[]
        {
            "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03",
            "urn:iso:std:iso:20022:tech:xsd:pain.001.001.08",
            "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09",
            "urn:iso:std:iso:20022:tech:xsd:pain.001.001.10",
            "urn:iso:std:iso:20022:tech:xsd:pain.001.001.11"
        };

        // Act
        var namespaces = Pain001ParserFactory.GetSupportedNamespaces();

        // Assert
        namespaces.ShouldNotBeNull();
        namespaces.Count.ShouldBe(expectedNamespaces.Length);

        foreach (var expectedNamespace in expectedNamespaces)
        {
            namespaces.ShouldContain(expectedNamespace, $"Factory should support namespace {expectedNamespace}");
        }
    }

    [Fact]
    public void GetSupportedVersions_ReturnsReadOnlyCollection()
    {
        // Act
        var versions = Pain001ParserFactory.GetSupportedVersions();

        // Assert
        versions.ShouldNotBeNull();
        versions.ShouldBeAssignableTo<IReadOnlyCollection<string>>();
    }

    [Fact]
    public void GetSupportedNamespaces_ReturnsReadOnlyCollection()
    {
        // Act
        var namespaces = Pain001ParserFactory.GetSupportedNamespaces();

        // Assert
        namespaces.ShouldNotBeNull();
        namespaces.ShouldBeAssignableTo<IReadOnlyCollection<string>>();
    }

    [Fact]
    public void AllParsers_ImplementIPain001Parser()
    {
        // Arrange
        var assembly = typeof(Pain001ParserFactory).Assembly;
        var parserTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<Pain001VersionAttribute>() != null)
            .ToList();

        // Assert
        parserTypes.ShouldNotBeEmpty("At least one parser with Pain001VersionAttribute should exist");

        foreach (var parserType in parserTypes)
        {
            typeof(IPain001Parser).IsAssignableFrom(parserType)
                .ShouldBeTrue($"{parserType.Name} should implement IPain001Parser");
        }
    }

    [Fact]
    public void AllParsers_AreSealed()
    {
        // Arrange
        var assembly = typeof(Pain001ParserFactory).Assembly;
        var parserTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<Pain001VersionAttribute>() != null)
            .ToList();

        // Assert
        parserTypes.ShouldNotBeEmpty();

        foreach (var parserType in parserTypes)
        {
            parserType.IsSealed.ShouldBeTrue($"{parserType.Name} should be sealed");
        }
    }

    [Fact]
    public void AllParsers_HaveRequiredConstructor()
    {
        // Arrange
        var assembly = typeof(Pain001ParserFactory).Assembly;
        var parserTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<Pain001VersionAttribute>() != null)
            .ToList();

        var requiredParameterTypes = new[] { typeof(Stream), typeof(bool), typeof(bool) };

        // Assert
        parserTypes.ShouldNotBeEmpty();

        foreach (var parserType in parserTypes)
        {
            var constructor = parserType.GetConstructor(requiredParameterTypes);
            constructor.ShouldNotBeNull(
                $"{parserType.Name} should have constructor with parameters (Stream, bool, bool)");
        }
    }

    [Fact]
    public void VersionNamespaceMapping_IsConsistent()
    {
        // Arrange
        var supportedVersions = Pain001ParserFactory.GetSupportedVersions();
        var supportedNamespaces = Pain001ParserFactory.GetSupportedNamespaces();

        // Assert
        supportedVersions.Count.ShouldBe(supportedNamespaces.Count,
            "Number of versions should match number of namespaces");

        foreach (var version in supportedVersions)
        {
            var ns = Pain001ParserFactory.GetNamespaceFromVersion(version);
            ns.ShouldNotBeNull($"Version {version} should have a corresponding namespace");
            supportedNamespaces.ShouldContain(ns!, $"Namespace for version {version} should be in supported namespaces");

            var versionFromNs = Pain001ParserFactory.GetVersionFromNamespace(ns!);
            versionFromNs.ShouldBe(version, "Mapping should be bidirectional");
        }
    }

    [Fact]
    public void ParserDiscovery_IsDeterministic()
    {
        // Act
        var versions1 = Pain001ParserFactory.GetSupportedVersions().OrderBy(v => v).ToList();
        var versions2 = Pain001ParserFactory.GetSupportedVersions().OrderBy(v => v).ToList();

        var namespaces1 = Pain001ParserFactory.GetSupportedNamespaces().OrderBy(n => n).ToList();
        var namespaces2 = Pain001ParserFactory.GetSupportedNamespaces().OrderBy(n => n).ToList();

        // Assert
        versions1.ShouldBe(versions2, "GetSupportedVersions should return consistent results");
        namespaces1.ShouldBe(namespaces2, "GetSupportedNamespaces should return consistent results");
    }

    [Theory]
    [InlineData("003")]
    [InlineData("008")]
    [InlineData("009")]
    [InlineData("010")]
    [InlineData("011")]
    public void CreateForVersion_WithLeaveOpen_CreatesParser(string version)
    {
        // Arrange
        var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.{version}"">
  <CstmrCdtTrfInitn>
    <GrpHdr>
      <MsgId>TEST</MsgId>
      <CreDtTm>2023-11-27T14:30:00Z</CreDtTm>
      <NbOfTxs>0</NbOfTxs>
      <InitgPty><Nm>Test</Nm></InitgPty>
    </GrpHdr>
  </CstmrCdtTrfInitn>
</Document>";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act
        var parser = Pain001ParserFactory.CreateForVersion(version, stream, leaveOpen: true, cacheGroupHeader: false);

        // Assert
        parser.ShouldNotBeNull();
        parser.Version.ShouldBe(version);
    }

    [Fact]
    public void Pain001VersionAttribute_IsNotInheritable()
    {
        // Arrange
        var attributeType = typeof(Pain001VersionAttribute);

        // Act
        var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.Inherited.ShouldBeFalse("Pain001VersionAttribute should not be inheritable");
        attributeUsage.AllowMultiple.ShouldBeFalse("Pain001VersionAttribute should not allow multiple instances");
    }
}
