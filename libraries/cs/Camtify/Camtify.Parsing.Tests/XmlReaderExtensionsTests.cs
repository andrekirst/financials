using System.Text;
using System.Xml;
using Camtify.Core.Parsing;
using Camtify.Parsing;

namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for <see cref="XmlReaderExtensions"/>.
/// </summary>
public class XmlReaderExtensionsTests
{
    private readonly IXmlReaderFactory _factory;
    private readonly string _tempDirectory;

    public XmlReaderExtensionsTests()
    {
        _factory = new Iso20022XmlReaderFactory();
        _tempDirectory = Path.Combine(Path.GetTempPath(), "CamtifyTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void CreateSecureReader_WithValidFile_ReturnsReader()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";
        var filePath = Path.Combine(_tempDirectory, "test.xml");
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            using var reader = _factory.CreateSecureReader(filePath);

            // Assert
            reader.ShouldNotBeNull();
            reader.Read().ShouldBeTrue();
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void CreateSecureReader_WithNullFactory_ThrowsArgumentNullException()
    {
        // Arrange
        IXmlReaderFactory? factory = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => factory!.CreateSecureReader("test.xml"));
    }

    [Fact]
    public void CreateSecureReader_WithNullFilePath_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.CreateSecureReader(null!));
    }

    [Fact]
    public void CreateSecureReader_WithEmptyFilePath_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.CreateSecureReader(string.Empty));
    }

    [Fact]
    public void CreateSecureReader_WithWhitespaceFilePath_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.CreateSecureReader("   "));
    }

    [Fact]
    public void CreateSecureReader_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "nonexistent.xml");

        // Act & Assert
        Should.Throw<FileNotFoundException>(() => _factory.CreateSecureReader(filePath));
    }

    [Fact]
    public void CreateSecureReader_CanReadXmlContent()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn>
        <GrpHdr>
            <MsgId>MSG123</MsgId>
        </GrpHdr>
    </CstmrCdtTrfInitn>
</Document>";
        var filePath = Path.Combine(_tempDirectory, "test2.xml");
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            using var reader = _factory.CreateSecureReader(filePath);
            var content = new List<string>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    content.Add(reader.LocalName);
                }
            }

            // Assert
            content.ShouldContain("Document");
            content.ShouldContain("CstmrCdtTrfInitn");
            content.ShouldContain("GrpHdr");
            content.ShouldContain("MsgId");
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void CreateFromString_WithValidXml_ReturnsReader()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";

        // Act
        using var reader = _factory.CreateFromString(xml);

        // Assert
        reader.ShouldNotBeNull();
        reader.Read().ShouldBeTrue();
    }

    [Fact]
    public void CreateFromString_WithNullFactory_ThrowsArgumentNullException()
    {
        // Arrange
        IXmlReaderFactory? factory = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => factory!.CreateFromString("<Document/>"));
    }

    [Fact]
    public void CreateFromString_WithNullXml_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.CreateFromString(null!));
    }

    [Fact]
    public void CreateFromString_WithEmptyXml_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.CreateFromString(string.Empty));
    }

    [Fact]
    public void CreateFromString_WithWhitespaceXml_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.CreateFromString("   "));
    }

    [Fact]
    public void CreateFromString_CanReadXmlContent()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn>
        <GrpHdr>
            <MsgId>MSG123</MsgId>
        </GrpHdr>
    </CstmrCdtTrfInitn>
</Document>";

        // Act
        using var reader = _factory.CreateFromString(xml);
        var content = new List<string>();

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                content.Add(reader.LocalName);
            }
        }

        // Assert
        content.ShouldContain("Document");
        content.ShouldContain("CstmrCdtTrfInitn");
        content.ShouldContain("GrpHdr");
        content.ShouldContain("MsgId");
    }

    [Fact]
    public void CreateFromString_WithMalformedXml_ThrowsXmlException()
    {
        // Arrange
        var xml = "<Document><Unclosed>";

        // Act
        using var reader = _factory.CreateFromString(xml);

        // Assert
        Should.Throw<XmlException>(() =>
        {
            while (reader.Read())
            {
                // Read until exception
            }
        });
    }

    [Fact]
    public async Task CreateFromString_SupportsAsyncRead()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";

        // Act
        using var reader = _factory.CreateFromString(xml);
        var result = await reader.ReadAsync();

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void CreateFromString_WithXxePayload_IsProtected()
    {
        // Arrange
        var xxePayload = @"<?xml version=""1.0""?>
<!DOCTYPE foo [
    <!ENTITY xxe SYSTEM ""file:///etc/passwd"">
]>
<Document>&xxe;</Document>";

        // Act
        using var reader = _factory.CreateFromString(xxePayload);

        // Assert
        Should.Throw<XmlException>(() =>
        {
            while (reader.Read())
            {
                // Read until exception
            }
        });
    }

    [Fact]
    public void CreateSecureReader_WithLargeFile_CanRead()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "large.xml");

        // Create a large XML file
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine(@"<?xml version=""1.0""?>");
            writer.WriteLine(@"<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">");

            // Write many elements
            for (int i = 0; i < 1000; i++)
            {
                writer.WriteLine($"    <Entry id=\"{i}\">Data {i}</Entry>");
            }

            writer.WriteLine("</Document>");
        }

        try
        {
            // Act
            using var reader = _factory.CreateSecureReader(filePath);
            var count = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Entry")
                {
                    count++;
                }
            }

            // Assert
            count.ShouldBe(1000);
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}
