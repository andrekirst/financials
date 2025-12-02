using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Camtify.Core;

/// <summary>
/// Loads additional namespaces from JSON configuration.
/// </summary>
public sealed class ConfigurableNamespaceRegistry : NamespaceRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableNamespaceRegistry"/> class.
    /// </summary>
    /// <param name="logger">Optional logger for debugging.</param>
    public ConfigurableNamespaceRegistry(ILogger<ConfigurableNamespaceRegistry>? logger = null)
        : base(logger)
    {
    }

    /// <summary>
    /// Loads additional namespaces from a JSON configuration file.
    /// </summary>
    /// <param name="jsonPath">Path to the JSON configuration file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file is not found.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public void LoadFromJson(string jsonPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {jsonPath}");
        }

        var json = File.ReadAllText(jsonPath);
        var config = JsonSerializer.Deserialize<NamespaceConfig>(json);

        if (config?.Mappings == null)
        {
            return;
        }

        foreach (var mapping in config.Mappings)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(mapping.Namespace) || string.IsNullOrEmpty(mapping.MessageId))
            {
                continue;
            }

            if (MessageIdentifier.TryParse(mapping.MessageId, out var messageId, out _))
            {
                Register(mapping.Namespace, messageId!.Value);
            }
        }
    }

    /// <summary>
    /// Loads additional namespaces from a JSON configuration file asynchronously.
    /// </summary>
    /// <param name="jsonPath">Path to the JSON configuration file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file is not found.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public async Task LoadFromJsonAsync(string jsonPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {jsonPath}");
        }

        await using var stream = File.OpenRead(jsonPath);
        var config = await JsonSerializer.DeserializeAsync<NamespaceConfig>(stream, cancellationToken: cancellationToken);

        if (config?.Mappings == null)
        {
            return;
        }

        foreach (var mapping in config.Mappings)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(mapping.Namespace) || string.IsNullOrEmpty(mapping.MessageId))
            {
                continue;
            }

            if (MessageIdentifier.TryParse(mapping.MessageId, out var messageId, out _))
            {
                Register(mapping.Namespace, messageId!.Value);
            }
        }
    }
}
