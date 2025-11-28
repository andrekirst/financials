using Camtify.Messages.Pain001.Parsers;

namespace Camtify.Messages.Pain001.Tests;

/// <summary>
/// Tests for Pain001ParserFactory.
/// </summary>
public sealed class Pain001ParserFactoryTests
{
    [Theory]
    [InlineData("pain001_v03_simple.xml", "003", typeof(Pain001v03Parser))]
    [InlineData("pain001_v08_simple.xml", "008", typeof(Pain001v08Parser))]
    [InlineData("pain001_v09_simple.xml", "009", typeof(Pain001v09Parser))]
    [InlineData("pain001_v10_simple.xml", "010", typeof(Pain001v10Parser))]
    [InlineData("pain001_v11_simple.xml", "011", typeof(Pain001v11Parser))]
    public async Task CreateAsync_DetectsVersionCorrectly(string fileName, string expectedVersion, Type expectedParserType)
    {
        // Arrange
        var xmlPath = Path.Combine("TestData", fileName);
        var xml = await File.ReadAllTextAsync(xmlPath);
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));

        // Act
        var parser = await Pain001ParserFactory.CreateAsync(stream);

        // Assert
        parser.ShouldNotBeNull();
        parser.ShouldBeOfType(expectedParserType);
        parser.Version.ShouldBe(expectedVersion);
    }

    [Fact]
    public async Task CreateAsync_UnsupportedVersion_ThrowsNotSupportedException()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.02"">
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
        await Should.ThrowAsync<NotSupportedException>(async () => await Pain001ParserFactory.CreateAsync(stream));
    }

    [Fact]
    public async Task CreateAsync_NonSeekableStream_ThrowsArgumentException()
    {
        // Arrange
        var nonSeekableStream = new NonSeekableStream();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await Pain001ParserFactory.CreateAsync(nonSeekableStream));
    }

    [Fact]
    public async Task CreateAsync_NullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await Pain001ParserFactory.CreateAsync(null!));
    }

    [Theory]
    [InlineData("003", true)]
    [InlineData("008", true)]
    [InlineData("009", true)]
    [InlineData("010", true)]
    [InlineData("011", true)]
    [InlineData("002", false)]
    [InlineData("012", false)]
    [InlineData("999", false)]
    public void IsVersionSupported_ReturnsCorrectValue(string version, bool expected)
    {
        // Act
        var result = Pain001ParserFactory.IsVersionSupported(version);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.03", true)]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.08", true)]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09", true)]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.10", true)]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.11", true)]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.02", false)]
    [InlineData("urn:invalid", false)]
    public void IsNamespaceSupported_ReturnsCorrectValue(string ns, bool expected)
    {
        // Act
        var result = Pain001ParserFactory.IsNamespaceSupported(ns);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.03", "003")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.08", "008")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09", "009")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.10", "010")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.11", "011")]
    public void GetVersionFromNamespace_ReturnsCorrectVersion(string ns, string expectedVersion)
    {
        // Act
        var result = Pain001ParserFactory.GetVersionFromNamespace(ns);

        // Assert
        result.ShouldBe(expectedVersion);
    }

    [Fact]
    public void GetVersionFromNamespace_UnsupportedNamespace_ReturnsNull()
    {
        // Act
        var result = Pain001ParserFactory.GetVersionFromNamespace("urn:invalid");

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("003", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03")]
    [InlineData("008", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.08")]
    [InlineData("009", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData("010", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.10")]
    [InlineData("011", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.11")]
    public void GetNamespaceFromVersion_ReturnsCorrectNamespace(string version, string expectedNamespace)
    {
        // Act
        var result = Pain001ParserFactory.GetNamespaceFromVersion(version);

        // Assert
        result.ShouldBe(expectedNamespace);
    }

    [Fact]
    public void GetNamespaceFromVersion_UnsupportedVersion_ReturnsNull()
    {
        // Act
        var result = Pain001ParserFactory.GetNamespaceFromVersion("999");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetSupportedVersions_ReturnsAllVersions()
    {
        // Act
        var versions = Pain001ParserFactory.GetSupportedVersions();

        // Assert
        versions.ShouldNotBeNull();
        versions.Count.ShouldBe(5);
        versions.ShouldContain("003");
        versions.ShouldContain("008");
        versions.ShouldContain("009");
        versions.ShouldContain("010");
        versions.ShouldContain("011");
    }

    [Fact]
    public async Task DetectVersionAsync_ReturnsCorrectVersion()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
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
        var version = await Pain001ParserFactory.DetectVersionAsync(stream);

        // Assert
        version.ShouldBe("009");
        stream.Position.ShouldBe(0); // Stream should be reset to beginning
    }

    [Fact]
    public async Task DetectVersionAsync_NonSeekableStream_ThrowsArgumentException()
    {
        // Arrange
        var nonSeekableStream = new NonSeekableStream();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await Pain001ParserFactory.DetectVersionAsync(nonSeekableStream));
    }

    /// <summary>
    /// Helper class to simulate a non-seekable stream for testing.
    /// </summary>
    private sealed class NonSeekableStream : MemoryStream
    {
        public override bool CanSeek => false;
    }
}
