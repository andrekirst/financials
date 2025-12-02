using Camtify.Core;

namespace Camtify.Core.Tests;

public class NamespaceRegistryTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldLoadBuiltInNamespaces()
    {
        // Act
        var registry = new NamespaceRegistry();

        // Assert
        registry.KnownMessages.Count.ShouldBeGreaterThan(0);
        registry.KnownNamespaces.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Constructor_ShouldLoadPainMessages()
    {
        // Act
        var registry = new NamespaceRegistry();

        // Assert
        var painMessages = registry.KnownMessages.Where(m => m.BusinessArea == "pain").ToList();
        painMessages.ShouldNotBeEmpty();
        painMessages.Count.ShouldBeGreaterThan(10); // Multiple versions of PAIN messages
    }

    [Fact]
    public void Constructor_ShouldLoadCamtMessages()
    {
        // Act
        var registry = new NamespaceRegistry();

        // Assert
        var camtMessages = registry.KnownMessages.Where(m => m.BusinessArea == "camt").ToList();
        camtMessages.ShouldNotBeEmpty();
        camtMessages.Count.ShouldBeGreaterThan(10); // Multiple versions of CAMT messages
    }

    [Fact]
    public void Constructor_ShouldLoadPacsMessages()
    {
        // Act
        var registry = new NamespaceRegistry();

        // Assert
        var pacsMessages = registry.KnownMessages.Where(m => m.BusinessArea == "pacs").ToList();
        pacsMessages.ShouldNotBeEmpty();
    }

    #endregion

    #region GetMessageIdentifier Tests

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09", "pain.001.001.09")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:camt.053.001.08", "camt.053.001.08")]
    [InlineData("urn:swift:xsd:pain.001.001.09", "pain.001.001.09")]
    [InlineData("urn:swift:xsd:camt.053.001.08", "camt.053.001.08")]
    public void GetMessageIdentifier_ValidIsoOrSwiftNamespace_ShouldReturnMessageId(string namespaceUri, string expectedMessageId)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var result = registry.GetMessageIdentifier(namespaceUri);

        // Assert
        result.ShouldNotBeNull();
        result!.Value.Value.ShouldBe(expectedMessageId);
    }

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09$cbpr_plus", "pain.001.001.09")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pacs.008.001.10$cbpr_plus", "pacs.008.001.10")]
    public void GetMessageIdentifier_CbprPlusNamespace_ShouldReturnMessageId(string namespaceUri, string expectedMessageId)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var result = registry.GetMessageIdentifier(namespaceUri);

        // Assert
        result.ShouldNotBeNull();
        result!.Value.Value.ShouldBe(expectedMessageId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetMessageIdentifier_NullOrEmpty_ShouldReturnNull(string? namespaceUri)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var result = registry.GetMessageIdentifier(namespaceUri!);

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("invalid:namespace")]
    [InlineData("http://example.com/namespace")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:invalid")]
    public void GetMessageIdentifier_InvalidNamespace_ShouldReturnNull(string namespaceUri)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var result = registry.GetMessageIdentifier(namespaceUri);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetMessageIdentifier_UnknownButValidNamespace_ShouldParseAndCache()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var unknownNamespace = "urn:iso:std:iso:20022:tech:xsd:pain.999.001.99";

        // Act
        var result = registry.GetMessageIdentifier(unknownNamespace);

        // Assert
        result.ShouldNotBeNull();
        result!.Value.Value.ShouldBe("pain.999.001.99");

        // Verify it was cached
        registry.IsKnownNamespace(unknownNamespace).ShouldBeTrue();
    }

    #endregion

    #region GetNamespace Tests

    [Theory]
    [InlineData("pain.001.001.09", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData("camt.053.001.08", "urn:iso:std:iso:20022:tech:xsd:camt.053.001.08")]
    [InlineData("pacs.008.001.10", "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.10")]
    public void GetNamespace_ValidMessageId_ShouldReturnIsoNamespace(string messageIdValue, string expectedNamespace)
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse(messageIdValue);

        // Act
        var result = registry.GetNamespace(messageId);

        // Assert
        result.ShouldBe(expectedNamespace);
    }

    [Fact]
    public void GetNamespace_ShouldAlwaysReturnIsoFormat()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse("pain.001.001.09");

        // Act
        var result = registry.GetNamespace(messageId);

        // Assert
        result.ShouldStartWith("urn:iso:std:iso:20022:tech:xsd:");
    }

    #endregion

    #region GetAllNamespaces Tests

    [Fact]
    public void GetAllNamespaces_RegisteredMessage_ShouldReturnAllVariants()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse("pain.001.001.09");

        // Act
        var namespaces = registry.GetAllNamespaces(messageId);

        // Assert
        namespaces.ShouldNotBeEmpty();
        namespaces.Count.ShouldBeGreaterThanOrEqualTo(2); // At least ISO and SWIFT
        namespaces.ShouldContain("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
        namespaces.ShouldContain("urn:swift:xsd:pain.001.001.09");
    }

    [Fact]
    public void GetAllNamespaces_UnregisteredMessage_ShouldReturnStandardNamespaces()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse("pain.999.001.99");

        // Act
        var namespaces = registry.GetAllNamespaces(messageId);

        // Assert
        namespaces.Count.ShouldBe(2);
        namespaces.ShouldContain("urn:iso:std:iso:20022:tech:xsd:pain.999.001.99");
        namespaces.ShouldContain("urn:swift:xsd:pain.999.001.99");
    }

    #endregion

    #region IsKnownNamespace Tests

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData("urn:swift:xsd:pain.001.001.09")]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:camt.053.001.08")]
    [InlineData("urn:swift:xsd:camt.053.001.08")]
    public void IsKnownNamespace_RegisteredNamespace_ShouldReturnTrue(string namespaceUri)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var result = registry.IsKnownNamespace(namespaceUri);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("invalid:namespace")]
    [InlineData("http://example.com/namespace")]
    public void IsKnownNamespace_InvalidNamespace_ShouldReturnFalse(string namespaceUri)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var result = registry.IsKnownNamespace(namespaceUri);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsKnownNamespace_ValidButUnknownNamespace_ShouldReturnTrue()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var unknownNamespace = "urn:iso:std:iso:20022:tech:xsd:pain.999.001.99";

        // Act
        var result = registry.IsKnownNamespace(unknownNamespace);

        // Assert
        result.ShouldBeTrue(); // Dynamic parsing makes it "known"
    }

    #endregion

    #region IsKnownMessage Tests

    [Theory]
    [InlineData("pain.001.001.09")]
    [InlineData("camt.053.001.08")]
    [InlineData("pacs.008.001.10")]
    public void IsKnownMessage_RegisteredMessage_ShouldReturnTrue(string messageIdValue)
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse(messageIdValue);

        // Act
        var result = registry.IsKnownMessage(messageId);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsKnownMessage_UnregisteredMessage_ShouldReturnFalse()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse("pain.999.001.99");

        // Act
        var result = registry.IsKnownMessage(messageId);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region Register Tests

    [Fact]
    public void Register_NewMapping_ShouldAddToRegistry()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse("pain.999.001.99");
        var namespaceUri = "urn:custom:namespace:pain.999.001.99";

        // Act
        registry.Register(namespaceUri, messageId);

        // Assert
        registry.IsKnownNamespace(namespaceUri).ShouldBeTrue();
        registry.IsKnownMessage(messageId).ShouldBeTrue();

        var retrievedMessageId = registry.GetMessageIdentifier(namespaceUri);
        retrievedMessageId.ShouldBe(messageId);
    }

    [Fact]
    public void Register_DuplicateNamespace_ShouldNotOverwrite()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId1 = MessageIdentifier.Parse("pain.001.001.09");
        var messageId2 = MessageIdentifier.Parse("pain.001.001.10");
        var namespaceUri = "urn:custom:namespace:pain.001.001.09";

        // Act
        registry.Register(namespaceUri, messageId1);
        registry.Register(namespaceUri, messageId2); // Try to overwrite

        // Assert
        var retrievedMessageId = registry.GetMessageIdentifier(namespaceUri);
        retrievedMessageId.ShouldBe(messageId1); // Should keep first registration
    }

    [Fact]
    public void Register_MultipleNamespacesForSameMessage_ShouldAllBeRetrievable()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messageId = MessageIdentifier.Parse("pain.001.001.09");
        var namespace1 = "urn:custom:namespace1:pain.001.001.09";
        var namespace2 = "urn:custom:namespace2:pain.001.001.09";

        // Act
        registry.Register(namespace1, messageId);
        registry.Register(namespace2, messageId);

        // Assert
        var namespaces = registry.GetAllNamespaces(messageId);
        namespaces.ShouldContain(namespace1);
        namespaces.ShouldContain(namespace2);
    }

    #endregion

    #region KnownMessages Tests

    [Fact]
    public void KnownMessages_ShouldContainAllRegisteredMessages()
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var messages = registry.KnownMessages;

        // Assert
        messages.ShouldNotBeEmpty();
        messages.ShouldContain(m => m.Value == "pain.001.001.09");
        messages.ShouldContain(m => m.Value == "camt.053.001.08");
    }

    [Fact]
    public void KnownMessages_ShouldBeReadOnly()
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var messages = registry.KnownMessages;

        // Assert
        messages.ShouldBeAssignableTo<IReadOnlyCollection<MessageIdentifier>>();
    }

    #endregion

    #region KnownNamespaces Tests

    [Fact]
    public void KnownNamespaces_ShouldContainAllRegisteredNamespaces()
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var namespaces = registry.KnownNamespaces;

        // Assert
        namespaces.ShouldNotBeEmpty();
        namespaces.ShouldContain("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
        namespaces.ShouldContain("urn:swift:xsd:pain.001.001.09");
    }

    [Fact]
    public void KnownNamespaces_ShouldBeReadOnly()
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var namespaces = registry.KnownNamespaces;

        // Assert
        namespaces.ShouldBeAssignableTo<IReadOnlyCollection<string>>();
    }

    #endregion

    #region Bidirectional Mapping Tests

    [Theory]
    [InlineData("pain.001.001.09")]
    [InlineData("camt.053.001.08")]
    [InlineData("pacs.008.001.10")]
    public void BidirectionalMapping_MessageToNamespaceAndBack_ShouldRoundTrip(string messageIdValue)
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var originalMessageId = MessageIdentifier.Parse(messageIdValue);

        // Act
        var @namespace = registry.GetNamespace(originalMessageId);
        var retrievedMessageId = registry.GetMessageIdentifier(@namespace!);

        // Assert
        retrievedMessageId.ShouldBe(originalMessageId);
    }

    [Theory]
    [InlineData("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")]
    [InlineData("urn:swift:xsd:camt.053.001.08")]
    public void BidirectionalMapping_NamespaceToMessageAndBack_ShouldContainOriginal(string originalNamespace)
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act
        var messageId = registry.GetMessageIdentifier(originalNamespace);
        var namespaces = registry.GetAllNamespaces(messageId!.Value);

        // Assert
        namespaces.ShouldContain(originalNamespace);
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public void Register_ConcurrentRegistrations_ShouldBeThreadSafe()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                var messageId = MessageIdentifier.Parse($"pain.{index:D3}.001.01");
                var namespaceUri = $"urn:test:namespace:pain.{index:D3}.001.01";
                registry.Register(namespaceUri, messageId);
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        registry.KnownMessages.Count.ShouldBeGreaterThanOrEqualTo(100);
    }

    [Fact]
    public void GetMessageIdentifier_ConcurrentReads_ShouldBeThreadSafe()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var tasks = new List<Task<MessageIdentifier?>>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() =>
                registry.GetMessageIdentifier("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09")));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        tasks.ShouldAllBe(t => t.Result != null);
        tasks.ShouldAllBe(t => t.Result!.Value.Value == "pain.001.001.09");
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void RealWorld_SepaPaymentInitiation_ShouldWork()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var sepaNamespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09";

        // Act
        var messageId = registry.GetMessageIdentifier(sepaNamespace);
        var allNamespaces = registry.GetAllNamespaces(messageId!.Value);

        // Assert
        messageId.ShouldNotBeNull();
        messageId!.Value.BusinessArea.ShouldBe("pain");
        messageId.Value.MessageNumber.ShouldBe("001");
        allNamespaces.ShouldContain(sepaNamespace);
    }

    [Fact]
    public void RealWorld_BankStatement_ShouldSupportMultipleVersions()
    {
        // Arrange
        var registry = new NamespaceRegistry();

        // Act & Assert - Check for multiple CAMT.053 versions
        registry.IsKnownMessage(MessageIdentifier.Parse("camt.053.001.02")).ShouldBeTrue();
        registry.IsKnownMessage(MessageIdentifier.Parse("camt.053.001.08")).ShouldBeTrue();
        registry.IsKnownMessage(MessageIdentifier.Parse("camt.053.001.10")).ShouldBeTrue();
    }

    [Fact]
    public void RealWorld_CbprPlusNamespace_ShouldBeSupported()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var cbprPlusNamespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09$cbpr_plus";

        // Act
        var messageId = registry.GetMessageIdentifier(cbprPlusNamespace);

        // Assert
        messageId.ShouldNotBeNull();
        messageId!.Value.Value.ShouldBe("pain.001.001.09");
    }

    [Fact]
    public void RealWorld_SupportedBusinessAreas_ShouldIncludeAll()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var messages = registry.KnownMessages.ToList();

        // Act
        var businessAreas = messages.Select(m => m.BusinessArea).Distinct().ToList();

        // Assert
        businessAreas.ShouldContain("pain");
        businessAreas.ShouldContain("camt");
        businessAreas.ShouldContain("pacs");
        businessAreas.ShouldContain("head");
        businessAreas.ShouldContain("acmt");
        businessAreas.ShouldContain("admi");
    }

    #endregion

    #region Lazy Parsing Tests

    [Fact]
    public void LazyParsing_UnknownButValidNamespace_ShouldParseOnDemand()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var unknownNamespace = "urn:iso:std:iso:20022:tech:xsd:reda.001.001.05";

        // Act
        var messageId = registry.GetMessageIdentifier(unknownNamespace);

        // Assert
        messageId.ShouldNotBeNull();
        messageId!.Value.BusinessArea.ShouldBe("reda");
        messageId.Value.MessageNumber.ShouldBe("001");
        messageId.Value.Version.ShouldBe("05");
    }

    [Fact]
    public void LazyParsing_AfterFirstParse_ShouldBeCached()
    {
        // Arrange
        var registry = new NamespaceRegistry();
        var unknownNamespace = "urn:iso:std:iso:20022:tech:xsd:reda.001.001.05";

        // Act
        var messageId1 = registry.GetMessageIdentifier(unknownNamespace);
        var messageId2 = registry.GetMessageIdentifier(unknownNamespace);

        // Assert
        messageId1.ShouldBe(messageId2);
        registry.IsKnownNamespace(unknownNamespace).ShouldBeTrue();
    }

    #endregion
}
