using System.Xml;
using Camtify.Core;
using Microsoft.Extensions.Logging;

namespace Camtify.Parsing;

/// <summary>
/// Standard implementation of the message detector for ISO 20022 messages.
/// </summary>
public sealed partial class Iso20022MessageDetector : IMessageDetector
{
    private readonly ILogger<Iso20022MessageDetector>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso20022MessageDetector"/> class.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    public Iso20022MessageDetector(ILogger<Iso20022MessageDetector>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<MessageIdentifier> DetectAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var result = await DetectWithDetailsAsync(stream, cancellationToken);
        return result.MessageId;
    }

    /// <inheritdoc/>
    public async Task<MessageDetectionResult> DetectWithDetailsAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var settings = new XmlReaderSettings
        {
            Async = true,
            IgnoreWhitespace = true,
            IgnoreComments = true,
            DtdProcessing = DtdProcessing.Prohibit, // XXE Prevention
            XmlResolver = null // XXE Prevention
        };

        using var reader = XmlReader.Create(stream, settings);

        string? rootElementName = null;
        string? rootNamespace = null;
        string? documentNamespace = null;
        string? messageElementName = null;
        string? msgDefIdr = null;
        bool hasAppHeader = false;
        MessageIdentifier? appHeaderMessageId = null;

        // Navigate to root element
        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element)
            {
                rootElementName = reader.LocalName;
                rootNamespace = reader.NamespaceURI;
                break;
            }
        }

        if (rootElementName is null)
        {
            throw new MessageDetectionException("XML does not contain a root element.");
        }

        _logger?.LogDebug("Root element: {RootElement}, Namespace: {Namespace}",
            rootElementName, rootNamespace);

        // Detect variant
        MessageVariant variant;

        if (rootElementName == "BizMsgEnvlp" || rootElementName == "RequestPayload")
        {
            // BAH-wrapped Document
            variant = MessageVariant.WithApplicationHeader;
            hasAppHeader = true;

            // Search for AppHdr and Document
            while (await reader.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (reader.NodeType != XmlNodeType.Element)
                    continue;

                if (reader.LocalName == "AppHdr")
                {
                    // Extract AppHdr namespace
                    var appHdrNs = reader.NamespaceURI;
                    if (MessageIdentifier.TryFromNamespace(appHdrNs, out var appMsgId, out _))
                    {
                        appHeaderMessageId = appMsgId;
                    }

                    // Search for MsgDefIdr
                    msgDefIdr = await FindMsgDefIdrAsync(reader, cancellationToken);
                }
                else if (reader.LocalName == "Document")
                {
                    documentNamespace = reader.NamespaceURI;

                    // Find message element name
                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Element)
                    {
                        messageElementName = reader.LocalName;
                    }
                    break;
                }
            }
        }
        else if (rootElementName == "Document")
        {
            // Standalone Document (may contain AppHdr)
            documentNamespace = rootNamespace;
            variant = DetermineVariant(rootNamespace);

            // Find message element name (check for AppHdr first)
            while (await reader.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "AppHdr")
                    {
                        // Found AppHdr inside Document
                        hasAppHeader = true;
                        variant = MessageVariant.WithApplicationHeader;

                        var appHdrNs = reader.NamespaceURI;
                        if (MessageIdentifier.TryFromNamespace(appHdrNs, out var appMsgId, out _))
                        {
                            appHeaderMessageId = appMsgId;
                        }

                        // Search for MsgDefIdr
                        msgDefIdr = await FindMsgDefIdrAsync(reader, cancellationToken);

                        // Continue to find message element
                        continue;
                    }

                    messageElementName = reader.LocalName;
                    break;
                }
            }
        }
        else
        {
            throw new MessageDetectionException(
                $"Unknown root element '{rootElementName}'. Expected: 'Document' or 'BizMsgEnvlp'.",
                rootNamespace,
                rootElementName);
        }

        // Extract message ID from namespace
        if (string.IsNullOrEmpty(documentNamespace))
        {
            throw new MessageDetectionException(
                "Document namespace not found.",
                rootNamespace,
                rootElementName);
        }

        if (!MessageIdentifier.TryFromNamespace(documentNamespace, out var messageId, out _))
        {
            // Fallback: Use MsgDefIdr from BAH
            if (!string.IsNullOrEmpty(msgDefIdr) &&
                MessageIdentifier.TryParse(msgDefIdr, out messageId, out _))
            {
                _logger?.LogDebug("Message ID from MsgDefIdr: {MessageId}", messageId);
            }
            else
            {
                throw new MessageDetectionException(
                    $"Could not extract message ID from namespace '{documentNamespace}'.",
                    documentNamespace,
                    rootElementName);
            }
        }

        return new MessageDetectionResult
        {
            MessageId = messageId!.Value,
            Namespace = documentNamespace,
            HasApplicationHeader = hasAppHeader,
            ApplicationHeaderMessageId = appHeaderMessageId,
            MessageDefinitionIdentifier = msgDefIdr,
            RootElementName = rootElementName,
            MessageElementName = messageElementName,
            Variant = variant
        };
    }

    /// <inheritdoc/>
    public async Task<(bool Success, MessageIdentifier? MessageId, string? Error)> TryDetectAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var messageId = await DetectAsync(stream, cancellationToken);
            return (true, messageId, null);
        }
        catch (MessageDetectionException ex)
        {
            return (false, null, ex.Message);
        }
        catch (XmlException ex)
        {
            return (false, null, $"XML error: {ex.Message}");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return (false, null, $"Unexpected error: {ex.Message}");
        }
    }

    private static MessageVariant DetermineVariant(string? namespaceUri)
    {
        if (string.IsNullOrEmpty(namespaceUri))
            return MessageVariant.Standalone;

        if (namespaceUri.Contains("urn:swift:xsd:", StringComparison.OrdinalIgnoreCase))
            return MessageVariant.Swift;

        if (namespaceUri.Contains("$", StringComparison.Ordinal))
            return MessageVariant.CbprPlus;

        return MessageVariant.Standalone;
    }

    private static async Task<string?> FindMsgDefIdrAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        // Search for MsgDefIdr in AppHdr
        var depth = reader.Depth;

        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Exit AppHdr
            if (reader.Depth <= depth && reader.NodeType == XmlNodeType.EndElement)
                break;

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "MsgDefIdr")
            {
                return await reader.ReadElementContentAsStringAsync();
            }
        }

        return null;
    }
}
