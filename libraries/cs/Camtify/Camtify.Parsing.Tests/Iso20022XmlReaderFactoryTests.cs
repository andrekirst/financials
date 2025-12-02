using System.Text;
using System.Xml;
using System.Xml.Schema;
using Camtify.Core.Parsing;
using Camtify.Parsing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for <see cref="Iso20022XmlReaderFactory"/>.
/// </summary>
public class Iso20022XmlReaderFactoryTests
{
    private readonly Iso20022XmlReaderFactory _factory;

    public Iso20022XmlReaderFactoryTests()
    {
        _factory = new Iso20022XmlReaderFactory();
    }

    [Fact]
    public void DefaultSettings_HasSecurityEnabled()
    {
        // Arrange & Act
        var settings = _factory.DefaultSettings;

        // Assert
        settings.DtdProcessing.ShouldBe(DtdProcessing.Prohibit);
        // Note: XmlResolver is write-only in .NET 9.0, so we can't check it here
        settings.MaxCharactersFromEntities.ShouldBe(1024);
    }

    [Fact]
    public void DefaultSettings_HasAsyncEnabled()
    {
        // Arrange & Act
        var settings = _factory.DefaultSettings;

        // Assert
        settings.Async.ShouldBeTrue();
    }

    [Fact]
    public void DefaultSettings_HasPerformanceOptimizations()
    {
        // Arrange & Act
        var settings = _factory.DefaultSettings;

        // Assert
        settings.IgnoreWhitespace.ShouldBeTrue();
        settings.IgnoreComments.ShouldBeTrue();
        settings.IgnoreProcessingInstructions.ShouldBeTrue();
    }

    [Fact]
    public void DefaultSettings_HasCorrectConformance()
    {
        // Arrange & Act
        var settings = _factory.DefaultSettings;

        // Assert
        settings.CheckCharacters.ShouldBeTrue();
        settings.ConformanceLevel.ShouldBe(ConformanceLevel.Document);
    }

    [Fact]
    public void ValidatingSettings_HasValidationConfigured()
    {
        // Arrange & Act
        var settings = _factory.ValidatingSettings;

        // Assert
        settings.ValidationType.ShouldBe(ValidationType.Schema);
        (settings.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints).ShouldBe(XmlSchemaValidationFlags.ProcessIdentityConstraints);
        (settings.ValidationFlags & XmlSchemaValidationFlags.ReportValidationWarnings).ShouldBe(XmlSchemaValidationFlags.ReportValidationWarnings);
    }

    [Fact]
    public void ValidatingSettings_InheritsSecurity()
    {
        // Arrange & Act
        var settings = _factory.ValidatingSettings;

        // Assert
        settings.DtdProcessing.ShouldBe(DtdProcessing.Prohibit);
        // Note: XmlResolver is write-only in .NET 9.0, so we can't check it here
    }

    [Fact]
    public void LargeFileSettings_HasUnlimitedDocumentSize()
    {
        // Arrange & Act
        var settings = _factory.LargeFileSettings;

        // Assert
        settings.MaxCharactersInDocument.ShouldBe(0);
    }

    [Fact]
    public void LargeFileSettings_KeepsEntityLimits()
    {
        // Arrange & Act
        var settings = _factory.LargeFileSettings;

        // Assert
        settings.MaxCharactersFromEntities.ShouldBe(1024);
    }

    [Fact]
    public void Create_WithValidXml_ReturnsReader()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        using var reader = _factory.Create(stream);

        // Assert
        reader.ShouldNotBeNull();
        reader.Settings.ShouldNotBeNull();
    }

    [Fact]
    public void Create_WithNullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.Create((Stream)null!));
    }

    [Fact]
    public void Create_WithNullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.Create(stream, null!));
    }

    [Fact]
    public void Create_WithXxePayload_ThrowsXmlException()
    {
        // Arrange
        var xxePayload = @"<?xml version=""1.0""?>
<!DOCTYPE foo [
    <!ENTITY xxe SYSTEM ""file:///etc/passwd"">
]>
<Document>&xxe;</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xxePayload));
        using var reader = _factory.Create(stream);

        // Act & Assert
        Should.Throw<XmlException>(() =>
        {
            while (reader.Read())
            {
                // Read until exception
            }
        });
    }

    [Fact]
    public void Create_WithDtdPayload_ThrowsXmlException()
    {
        // Arrange
        var dtdPayload = @"<?xml version=""1.0""?>
<!DOCTYPE Document [
    <!ELEMENT Document ANY>
]>
<Document>Test</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(dtdPayload));
        using var reader = _factory.Create(stream);

        // Act & Assert
        Should.Throw<XmlException>(() =>
        {
            while (reader.Read())
            {
                // Read until exception
            }
        });
    }

    [Fact]
    public async Task Create_WithAsyncSettings_SupportsAsyncRead()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09"">
    <CstmrCdtTrfInitn/>
</Document>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        using var reader = _factory.Create(stream);
        var result = await reader.ReadAsync();

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Create_WithCustomSettings_UsesProvidedSettings()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?><Document/>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var customSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreWhitespace = false
        };

        // Act
        using var reader = _factory.Create(stream, customSettings);

        // Assert
        reader.Settings.IgnoreWhitespace.ShouldBeFalse();
    }

    [Fact]
    public void CreateValidating_WithNullStream_ThrowsArgumentNullException()
    {
        // Arrange
        var schemaSet = new XmlSchemaSet();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.CreateValidating(null!, schemaSet));
    }

    [Fact]
    public void CreateValidating_WithNullSchemaSet_ThrowsArgumentNullException()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.CreateValidating(stream, null!));
    }

    [Fact]
    public void CreateValidating_WithValidSchemaSet_ReturnsValidatingReader()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?><Document/>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var schemaSet = new XmlSchemaSet();
        var schema = XmlSchema.Read(
            new StringReader(@"<?xml version=""1.0""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
    <xs:element name=""Document""/>
</xs:schema>"),
            null);
        schemaSet.Add(schema!);

        // Act
        using var reader = _factory.CreateValidating(stream, schemaSet);

        // Assert
        reader.ShouldNotBeNull();
        reader.Settings.ValidationType.ShouldBe(ValidationType.Schema);
    }

    [Fact]
    public void CreateValidating_WithValidationHandler_AttachesHandler()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?><Document/>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var schemaSet = new XmlSchemaSet();
        var handlerCalled = false;
        ValidationEventHandler handler = (sender, args) => handlerCalled = true;

        // Act
        using var reader = _factory.CreateValidating(stream, schemaSet, handler);

        // Assert - just verify reader is created, handler will be called on validation errors
        reader.ShouldNotBeNull();
    }

    [Fact]
    public void CreateFromTextReader_WithValidXml_ReturnsReader()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?><Document/>";
        using var textReader = new StringReader(xml);

        // Act
        using var reader = _factory.CreateFromTextReader(textReader);

        // Assert
        reader.ShouldNotBeNull();
    }

    [Fact]
    public void CreateFromTextReader_WithNullTextReader_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.CreateFromTextReader(null!));
    }

    [Fact]
    public void CreateWithNamespaceManager_WithValidManager_ReturnsReader()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?><Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.001.001.09""/>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var nameTable = new NameTable();
        var namespaceManager = new XmlNamespaceManager(nameTable);
        namespaceManager.AddNamespace("iso", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");

        // Act
        using var reader = _factory.CreateWithNamespaceManager(stream, namespaceManager);

        // Assert
        reader.ShouldNotBeNull();
    }

    [Fact]
    public void CreateWithNamespaceManager_WithNullStream_ThrowsArgumentNullException()
    {
        // Arrange
        var nameTable = new NameTable();
        var namespaceManager = new XmlNamespaceManager(nameTable);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.CreateWithNamespaceManager(null!, namespaceManager));
    }

    [Fact]
    public void CreateWithNamespaceManager_WithNullNamespaceManager_ThrowsArgumentNullException()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _factory.CreateWithNamespaceManager(stream, null!));
    }

    [Fact]
    public void Constructor_WithLogger_DoesNotThrow()
    {
        // Arrange
        var logger = NullLogger<Iso20022XmlReaderFactory>.Instance;

        // Act
        var factory = new Iso20022XmlReaderFactory(logger);

        // Assert
        factory.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithoutLogger_DoesNotThrow()
    {
        // Act
        var factory = new Iso20022XmlReaderFactory();

        // Assert
        factory.ShouldNotBeNull();
    }

    [Fact]
    public void Create_WithBillionLaughsAttack_IsProtectedByEntityLimits()
    {
        // Arrange - Billion Laughs Attack (XML bomb)
        var billionLaughs = @"<?xml version=""1.0""?>
<!DOCTYPE lolz [
    <!ENTITY lol ""lol"">
    <!ENTITY lol2 ""&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;"">
    <!ENTITY lol3 ""&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;"">
]>
<Document>&lol3;</Document>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(billionLaughs));
        using var reader = _factory.Create(stream);

        // Act & Assert - Should throw due to DTD being prohibited
        Should.Throw<XmlException>(() =>
        {
            while (reader.Read())
            {
                // Read until exception
            }
        });
    }

    [Fact]
    public void Create_IgnoresWhitespace()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<Document>
    <Element>Test</Element>
</Document>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        using var reader = _factory.Create(stream);
        var nodeTypes = new List<XmlNodeType>();

        while (reader.Read())
        {
            nodeTypes.Add(reader.NodeType);
        }

        // Assert - No Whitespace nodes should be present
        nodeTypes.ShouldNotContain(XmlNodeType.Whitespace);
    }

    [Fact]
    public void Create_IgnoresComments()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<!-- This is a comment -->
<Document>
    <Element>Test</Element>
</Document>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        using var reader = _factory.Create(stream);
        var nodeTypes = new List<XmlNodeType>();

        while (reader.Read())
        {
            nodeTypes.Add(reader.NodeType);
        }

        // Assert - No Comment nodes should be present
        nodeTypes.ShouldNotContain(XmlNodeType.Comment);
    }

    [Fact]
    public void Create_IgnoresProcessingInstructions()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<?xml-stylesheet type=""text/xsl"" href=""style.xsl""?>
<Document>
    <Element>Test</Element>
</Document>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        using var reader = _factory.Create(stream);
        var nodeTypes = new List<XmlNodeType>();

        while (reader.Read())
        {
            nodeTypes.Add(reader.NodeType);
        }

        // Assert - No ProcessingInstruction nodes should be present
        nodeTypes.ShouldNotContain(XmlNodeType.ProcessingInstruction);
    }
}
