using System.Xml;
using Camtify.Core;
using Camtify.Core.Parsing;
using Microsoft.Extensions.Logging;

namespace Camtify.Parsing.Examples;

/// <summary>
/// Example streaming parser for camt.053 Bank Statements.
/// </summary>
/// <remarks>
/// This is a simplified example to demonstrate the streaming parser pattern using pure XmlReader.
/// Production implementations should use full domain models.
/// </remarks>
public sealed class Camt053StreamingParser : StreamingParserBase<Camt053Document, CamtEntry>
{
    /// <inheritdoc />
    public override IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; } = new[]
    {
        MessageIdentifier.Camt.V053_08,
        MessageIdentifier.Camt.V053_10
    };

    /// <inheritdoc />
    protected override string EntryElementName => "Ntry";

    /// <summary>
    /// Initializes a new instance of the <see cref="Camt053StreamingParser"/> class.
    /// </summary>
    /// <param name="xmlReaderFactory">Factory for creating XML readers.</param>
    /// <param name="messageDetector">Detector for identifying message types.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public Camt053StreamingParser(
        IXmlReaderFactory xmlReaderFactory,
        IMessageDetector messageDetector,
        ILogger<Camt053StreamingParser>? logger = null)
        : base(xmlReaderFactory, messageDetector, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<CamtEntry> ParseEntryAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        // Reader is positioned at <Ntry> element
        var entry = new CamtEntry();

        if (reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            return entry;
        }

        var entryDepth = reader.Depth;
        await reader.ReadAsync(); // Enter <Ntry>

        while (reader.Depth > entryDepth)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case "NtryRef":
                        entry.EntryReference = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
                        break;
                    case "Amt":
                        var (amount, currency) = await reader.ReadAmountWithCurrencyAsync(cancellationToken);
                        entry.Amount = amount ?? 0m;
                        entry.Currency = currency;
                        break;
                    case "CdtDbtInd":
                        entry.CreditDebitIndicator = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
                        break;
                    case "Sts":
                        entry.Status = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
                        break;
                    case "BookgDt":
                        entry.BookingDate = await reader.ReadDateElementAsync(cancellationToken);
                        break;
                    case "ValDt":
                        entry.ValueDate = await reader.ReadDateElementAsync(cancellationToken);
                        break;
                    default:
                        await reader.SkipElementAsync(cancellationToken);
                        break;
                }
            }
            else
            {
                await reader.ReadAsync();
            }
        }

        return entry;
    }

    /// <inheritdoc />
    protected override async Task<object> ParseHeaderAsync(
        Stream stream,
        MessageIdentifier messageId,
        CancellationToken cancellationToken)
    {
        var settings = new XmlReaderSettings { Async = true };
        using var reader = XmlReader.Create(stream, settings);

        // Navigate to Stmt element
        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Stmt")
            {
                return await ParseStatementHeaderAsync(reader, cancellationToken);
            }
        }

        throw new Iso20022ParsingException("Stmt element not found");
    }

    /// <inheritdoc />
    protected override int? GetExpectedEntryCount(object header)
    {
        if (header is Camt053Document doc)
        {
            return doc.NumberOfEntries;
        }
        return null;
    }

    #region Private Helper Methods

    private static async Task<Camt053Document> ParseStatementHeaderAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        var doc = new Camt053Document();

        if (reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            return doc;
        }

        var stmtDepth = reader.Depth;
        await reader.ReadAsync(); // Enter <Stmt>

        while (reader.Depth > stmtDepth)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case "Id":
                        doc.StatementId = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
                        break;
                    case "Acct":
                        doc.AccountIban = await ParseAccountIbanAsync(reader, cancellationToken);
                        break;
                    case "CreDtTm":
                        doc.CreationDateTime = await reader.ReadElementContentAsDateTimeAsync(cancellationToken);
                        break;
                    case "TxsSummry":
                        doc.NumberOfEntries = await ParseTotalEntriesAsync(reader, cancellationToken);
                        break;
                    default:
                        await reader.SkipElementAsync(cancellationToken);
                        break;
                }
            }
            else
            {
                await reader.ReadAsync();
            }
        }

        return doc;
    }

    private static async Task<string?> ParseAccountIbanAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        if (reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            return null;
        }

        var acctDepth = reader.Depth;
        await reader.ReadAsync(); // Enter <Acct>

        while (reader.Depth > acctDepth)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Id")
            {
                // Enter Id element
                if (reader.IsEmptyElement)
                {
                    await reader.ReadAsync();
                    continue;
                }

                var idDepth = reader.Depth;
                await reader.ReadAsync();

                while (reader.Depth > idDepth)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "IBAN")
                    {
                        return await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
                    }
                    await reader.ReadAsync();
                }
            }
            else
            {
                await reader.ReadAsync();
            }
        }

        return null;
    }

    private static async Task<int?> ParseTotalEntriesAsync(
        XmlReader reader,
        CancellationToken cancellationToken)
    {
        if (reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            return null;
        }

        var summaryDepth = reader.Depth;
        await reader.ReadAsync(); // Enter <TxsSummry>

        while (reader.Depth > summaryDepth)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "TtlNtries")
            {
                // Enter TtlNtries element
                if (reader.IsEmptyElement)
                {
                    await reader.ReadAsync();
                    continue;
                }

                var entriesDepth = reader.Depth;
                await reader.ReadAsync();

                while (reader.Depth > entriesDepth)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "NbOfNtries")
                    {
                        return await reader.ReadElementContentAsIntAsync(cancellationToken);
                    }
                    await reader.ReadAsync();
                }
            }
            else
            {
                await reader.ReadAsync();
            }
        }

        return null;
    }

    #endregion
}
