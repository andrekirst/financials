using System.Text.Json;
using Camtify.Core;

namespace Camtify.Core.Tests;

public class ConfigurableNamespaceRegistryTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldInheritBuiltInNamespaces()
    {
        // Act
        var registry = new ConfigurableNamespaceRegistry();

        // Assert
        registry.KnownMessages.Count.ShouldBeGreaterThan(0);
        registry.IsKnownMessage(MessageIdentifier.Parse("pain.001.001.09")).ShouldBeTrue();
    }

    #endregion

    #region LoadFromJson Tests

    [Fact]
    public void LoadFromJson_ValidConfiguration_ShouldLoadNamespaces()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:custom:namespace:pain.999.001.01", MessageId = "pain.999.001.01" },
                new { Namespace = "urn:custom:namespace:camt.999.001.01", MessageId = "camt.999.001.01" }
            }
        };

        File.WriteAllText(jsonPath, JsonSerializer.Serialize(config));

        try
        {
            // Act
            registry.LoadFromJson(jsonPath);

            // Assert
            registry.IsKnownNamespace("urn:custom:namespace:pain.999.001.01").ShouldBeTrue();
            registry.IsKnownNamespace("urn:custom:namespace:camt.999.001.01").ShouldBeTrue();

            var messageId1 = registry.GetMessageIdentifier("urn:custom:namespace:pain.999.001.01");
            messageId1.ShouldNotBeNull();
            messageId1!.Value.Value.ShouldBe("pain.999.001.01");
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void LoadFromJson_EmptyMappings_ShouldNotThrow()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new { Mappings = Array.Empty<object>() };
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(config));

        try
        {
            // Act & Assert
            Should.NotThrow(() => registry.LoadFromJson(jsonPath));
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void LoadFromJson_NullMappings_ShouldNotThrow()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        File.WriteAllText(jsonPath, "{}");

        try
        {
            // Act & Assert
            Should.NotThrow(() => registry.LoadFromJson(jsonPath));
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void LoadFromJson_InvalidMessageId_ShouldSkipInvalidEntries()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:custom:namespace:valid", MessageId = "pain.001.001.09" },
                new { Namespace = "urn:custom:namespace:invalid", MessageId = "invalid" }
            }
        };

        File.WriteAllText(jsonPath, JsonSerializer.Serialize(config));

        try
        {
            // Act
            registry.LoadFromJson(jsonPath);

            // Assert
            registry.IsKnownNamespace("urn:custom:namespace:valid").ShouldBeTrue();
            registry.IsKnownNamespace("urn:custom:namespace:invalid").ShouldBeFalse();
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void LoadFromJson_EmptyNamespaceOrMessageId_ShouldSkip()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new
        {
            Mappings = new[]
            {
                new { Namespace = "", MessageId = "pain.001.001.09" },
                new { Namespace = "urn:custom:namespace:pain.001.001.09", MessageId = "" }
            }
        };

        File.WriteAllText(jsonPath, JsonSerializer.Serialize(config));

        try
        {
            // Act & Assert
            Should.NotThrow(() => registry.LoadFromJson(jsonPath));
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void LoadFromJson_FileNotFound_ShouldThrow()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");

        // Act & Assert
        Should.Throw<FileNotFoundException>(() => registry.LoadFromJson(jsonPath));
    }

    [Fact]
    public void LoadFromJson_InvalidJson_ShouldThrow()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        File.WriteAllText(jsonPath, "{ invalid json }");

        try
        {
            // Act & Assert
            Should.Throw<JsonException>(() => registry.LoadFromJson(jsonPath));
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    #endregion

    #region LoadFromJsonAsync Tests

    [Fact]
    public async Task LoadFromJsonAsync_ValidConfiguration_ShouldLoadNamespaces()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:custom:namespace:async:pain.999.001.01", MessageId = "pain.999.001.01" },
                new { Namespace = "urn:custom:namespace:async:camt.999.001.01", MessageId = "camt.999.001.01" }
            }
        };

        await File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(config));

        try
        {
            // Act
            await registry.LoadFromJsonAsync(jsonPath);

            // Assert
            registry.IsKnownNamespace("urn:custom:namespace:async:pain.999.001.01").ShouldBeTrue();
            registry.IsKnownNamespace("urn:custom:namespace:async:camt.999.001.01").ShouldBeTrue();

            var messageId1 = registry.GetMessageIdentifier("urn:custom:namespace:async:pain.999.001.01");
            messageId1.ShouldNotBeNull();
            messageId1!.Value.Value.ShouldBe("pain.999.001.01");
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public async Task LoadFromJsonAsync_FileNotFound_ShouldThrow()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");

        // Act & Assert
        await Should.ThrowAsync<FileNotFoundException>(async () => await registry.LoadFromJsonAsync(jsonPath));
    }

    [Fact]
    public async Task LoadFromJsonAsync_CancellationRequested_ShouldThrow()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:custom:namespace:pain.001.001.09", MessageId = "pain.001.001.09" }
            }
        };

        await File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(config));

        var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            // Act & Assert
            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await registry.LoadFromJsonAsync(jsonPath, cts.Token));
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Integration_LoadJsonAndUseRegistry_ShouldWork()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath = Path.Combine(Path.GetTempPath(), $"namespaces_{Guid.NewGuid()}.json");

        var config = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:bank:sepa:pain.001.001.09", MessageId = "pain.001.001.09" },
                new { Namespace = "urn:bank:sepa:camt.053.001.08", MessageId = "camt.053.001.08" }
            }
        };

        File.WriteAllText(jsonPath, JsonSerializer.Serialize(config));

        try
        {
            // Act
            registry.LoadFromJson(jsonPath);

            // Assert - Custom namespaces should work
            var painMessageId = registry.GetMessageIdentifier("urn:bank:sepa:pain.001.001.09");
            painMessageId.ShouldNotBeNull();
            painMessageId!.Value.Value.ShouldBe("pain.001.001.09");

            // Assert - Built-in namespaces should still work
            var builtInMessageId = registry.GetMessageIdentifier("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
            builtInMessageId.ShouldNotBeNull();
            builtInMessageId!.Value.Value.ShouldBe("pain.001.001.09");

            // Assert - Both custom and built-in namespaces should be listed
            var allNamespaces = registry.GetAllNamespaces(painMessageId.Value);
            allNamespaces.ShouldContain("urn:bank:sepa:pain.001.001.09");
            allNamespaces.ShouldContain("urn:iso:std:iso:20022:tech:xsd:pain.001.001.09");
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void Integration_MultipleLoadCalls_ShouldAccumulate()
    {
        // Arrange
        var registry = new ConfigurableNamespaceRegistry();
        var jsonPath1 = Path.Combine(Path.GetTempPath(), $"namespaces1_{Guid.NewGuid()}.json");
        var jsonPath2 = Path.Combine(Path.GetTempPath(), $"namespaces2_{Guid.NewGuid()}.json");

        var config1 = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:bank1:pain.001.001.09", MessageId = "pain.001.001.09" }
            }
        };

        var config2 = new
        {
            Mappings = new[]
            {
                new { Namespace = "urn:bank2:camt.053.001.08", MessageId = "camt.053.001.08" }
            }
        };

        File.WriteAllText(jsonPath1, JsonSerializer.Serialize(config1));
        File.WriteAllText(jsonPath2, JsonSerializer.Serialize(config2));

        try
        {
            // Act
            registry.LoadFromJson(jsonPath1);
            registry.LoadFromJson(jsonPath2);

            // Assert
            registry.IsKnownNamespace("urn:bank1:pain.001.001.09").ShouldBeTrue();
            registry.IsKnownNamespace("urn:bank2:camt.053.001.08").ShouldBeTrue();
        }
        finally
        {
            if (File.Exists(jsonPath1))
                File.Delete(jsonPath1);
            if (File.Exists(jsonPath2))
                File.Delete(jsonPath2);
        }
    }

    #endregion
}
