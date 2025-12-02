using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Camtify.Core;

/// <summary>
/// Standard implementation of the Namespace Registry.
/// Loads known namespaces from built-in configuration.
/// </summary>
public class NamespaceRegistry : INamespaceRegistry
{
    private readonly ConcurrentDictionary<string, MessageIdentifier> _namespaceToMessage = new();
    private readonly ConcurrentDictionary<MessageIdentifier, HashSet<string>> _messageToNamespaces = new();
    private readonly ILogger<NamespaceRegistry>? _logger;

    // Namespace prefixes
    private const string IsoPrefix = "urn:iso:std:iso:20022:tech:xsd:";
    private const string SwiftPrefix = "urn:swift:xsd:";
    private const string CbprPlusSuffix = "$cbpr_plus";

    /// <summary>
    /// Initializes a new instance of the <see cref="NamespaceRegistry"/> class.
    /// </summary>
    /// <param name="logger">Optional logger for debugging.</param>
    public NamespaceRegistry(ILogger<NamespaceRegistry>? logger = null)
    {
        _logger = logger;
        LoadBuiltInNamespaces();
    }

    /// <inheritdoc/>
    public MessageIdentifier? GetMessageIdentifier(string namespaceUri)
    {
        if (string.IsNullOrEmpty(namespaceUri))
        {
            return null;
        }

        // Direct lookup
        if (_namespaceToMessage.TryGetValue(namespaceUri, out var messageId))
        {

            return messageId;
        }

        // Try dynamic parsing
        if (TryParseNamespace(namespaceUri, out MessageIdentifier? parsedMessageId))
        {
            // Cache for future lookups
            Register(namespaceUri, parsedMessageId!.Value);
            return parsedMessageId;
        }

        return null;
    }

    /// <inheritdoc/>
    public string? GetNamespace(MessageIdentifier messageId)
    {
        // Generate standard ISO namespace
        return $"{IsoPrefix}{messageId}";
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<string> GetAllNamespaces(MessageIdentifier messageId)
    {
        if (_messageToNamespaces.TryGetValue(messageId, out var namespaces))
        {
            return [.. namespaces];
        }

        // Generate standard namespaces
        return
        [
            $"{IsoPrefix}{messageId}",
            $"{SwiftPrefix}{messageId}"
        ];
    }

    /// <inheritdoc/>
    public bool IsKnownNamespace(string namespaceUri)
    {
        return _namespaceToMessage.ContainsKey(namespaceUri) ||
               TryParseNamespace(namespaceUri, out _);
    }

    /// <inheritdoc/>
    public bool IsKnownMessage(MessageIdentifier messageId)
    {
        return _messageToNamespaces.ContainsKey(messageId);
    }

    /// <inheritdoc/>
    public void Register(string namespaceUri, MessageIdentifier messageId)
    {
        _namespaceToMessage.TryAdd(namespaceUri, messageId);
        _messageToNamespaces.AddOrUpdate(
            messageId,
            _ => [namespaceUri],
            (_, set) =>
            {
                lock (set)
                {
                    set.Add(namespaceUri);
                }

                return set;
            });

        _logger?.LogDebug("Namespace registered: {Namespace} -> {MessageId}", namespaceUri, messageId);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<MessageIdentifier> KnownMessages =>
        _messageToNamespaces.Keys.ToList();

    /// <inheritdoc/>
    public IReadOnlyCollection<string> KnownNamespaces =>
        _namespaceToMessage.Keys.ToList();

    private void LoadBuiltInNamespaces()
    {
        // PAIN Messages
        RegisterStandardVersions("pain", "001", new[] { "03", "08", "09", "10", "11" }); // Credit Transfer
        RegisterStandardVersions("pain", "002", new[] { "03", "09", "10" }); // Status Report
        RegisterStandardVersions("pain", "008", new[] { "02", "07", "08", "09" }); // Direct Debit
        RegisterStandardVersions("pain", "013", new[] { "07", "08" }); // Creditor Payment Activation Request
        RegisterStandardVersions("pain", "014", new[] { "07", "08" }); // Creditor Payment Activation Status

        // CAMT Messages
        RegisterStandardVersions("camt", "052", new[] { "02", "06", "08", "10" }); // Account Report
        RegisterStandardVersions("camt", "053", new[] { "02", "04", "06", "08", "10" }); // Statement
        RegisterStandardVersions("camt", "054", new[] { "02", "06", "08", "09" }); // Notification
        RegisterStandardVersions("camt", "055", new[] { "01", "06", "08" }); // Cancel Request
        RegisterStandardVersions("camt", "056", new[] { "01", "06", "08", "09" }); // FI Cancel Request
        RegisterStandardVersions("camt", "029", new[] { "03", "08", "09" }); // Resolution
        RegisterStandardVersions("camt", "026", new[] { "02", "07", "08" }); // Unable to Apply

        // PACS Messages
        RegisterStandardVersions("pacs", "002", new[] { "03", "08", "10", "11" }); // Status Report
        RegisterStandardVersions("pacs", "004", new[] { "02", "08", "09", "10" }); // Payment Return
        RegisterStandardVersions("pacs", "008", new[] { "02", "06", "08", "10" }); // FI Credit Transfer
        RegisterStandardVersions("pacs", "009", new[] { "02", "06", "08", "09" }); // FI to FI Transfer
        RegisterStandardVersions("pacs", "028", new[] { "01", "03", "04" }); // Status Request

        // HEAD (Business Application Header)
        RegisterStandardVersions("head", "001", new[] { "01", "02" });

        // ACMT Messages
        RegisterStandardVersions("acmt", "001", new[] { "02", "07", "08" }); // Account Opening
        RegisterStandardVersions("acmt", "002", new[] { "02", "07", "08" }); // Account Modification
        RegisterStandardVersions("acmt", "003", new[] { "02", "07", "08" }); // Account Closing

        // ADMI Messages
        RegisterStandardVersions("admi", "002", new[] { "01" }); // Message Reject
        RegisterStandardVersions("admi", "004", new[] { "01", "02" }); // System Event
        RegisterStandardVersions("admi", "007", new[] { "01" }); // Receipt Acknowledgement

        _logger?.LogInformation("Namespace registry initialized with {Count} messages",
            KnownMessages.Count);
    }

    private void RegisterStandardVersions(string area, string type, string[] versions)
    {
        foreach (var version in versions)
        {
            var messageId = MessageIdentifier.Parse($"{area}.{type}.001.{version}");

            // ISO namespace
            var isoNamespace = $"{IsoPrefix}{messageId}";
            Register(isoNamespace, messageId);

            // SWIFT namespace
            var swiftNamespace = $"{SwiftPrefix}{messageId}";
            Register(swiftNamespace, messageId);
        }
    }

    private static bool TryParseNamespace(string namespaceUri, out MessageIdentifier? messageId)
    {
        messageId = null;

        // ISO format
        if (namespaceUri.StartsWith(IsoPrefix))
        {
            var remainder = namespaceUri.Substring(IsoPrefix.Length);

            // Remove CBPR+ suffix
            if (remainder.EndsWith(CbprPlusSuffix))
            {
                remainder = remainder.Substring(0, remainder.Length - CbprPlusSuffix.Length);
            }

            return MessageIdentifier.TryParse(remainder, out messageId, out _);
        }

        // SWIFT format
        if (namespaceUri.StartsWith(SwiftPrefix))
        {
            var remainder = namespaceUri.Substring(SwiftPrefix.Length);
            return MessageIdentifier.TryParse(remainder, out messageId, out _);
        }

        return false;
    }
}
