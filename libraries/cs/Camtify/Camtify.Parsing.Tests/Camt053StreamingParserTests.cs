using System.Text;
using Camtify.Core;
using Camtify.Core.Parsing;
using Camtify.Parsing.Examples;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for the <see cref="Camt053StreamingParser"/> class.
/// </summary>
public class Camt053StreamingParserTests
{
    private const string Camt053V08Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.053.001.08";
    private const string Camt053V10Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.053.001.10";

    private static string GenerateCamt053Xml(
        int entryCount,
        string ns = Camt053V08Namespace,
        string? statementId = null,
        string? iban = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine($"<Document xmlns=\"{ns}\">");
        sb.AppendLine("  <BkToCstmrStmt>");
        sb.AppendLine("    <Stmt>");
        sb.AppendLine($"      <Id>{statementId ?? "STMT-2024-001"}</Id>");
        sb.AppendLine("      <CreDtTm>");
        sb.AppendLine("        <Dt>2024-01-15</Dt>");
        sb.AppendLine("      </CreDtTm>");
        sb.AppendLine("      <Acct>");
        sb.AppendLine("        <Id>");
        sb.AppendLine($"          <IBAN>{iban ?? "DE89370400440532013000"}</IBAN>");
        sb.AppendLine("        </Id>");
        sb.AppendLine("      </Acct>");
        sb.AppendLine("      <TxsSummry>");
        sb.AppendLine("        <TtlNtries>");
        sb.AppendLine($"          <NbOfNtries>{entryCount}</NbOfNtries>");
        sb.AppendLine("        </TtlNtries>");
        sb.AppendLine("      </TxsSummry>");

        for (var i = 1; i <= entryCount; i++)
        {
            var amount = 100.00m + i;
            var cdtDbt = i % 2 == 0 ? "CRDT" : "DBIT";
            
            sb.AppendLine("      <Ntry>");
            sb.AppendLine($"        <NtryRef>ENTRY-{i:D6}</NtryRef>");
            sb.AppendLine($"        <Amt Ccy=\"EUR\">{amount:F2}</Amt>");
            sb.AppendLine($"        <CdtDbtInd>{cdtDbt}</CdtDbtInd>");
            sb.AppendLine("        <Sts>BOOK</Sts>");
            sb.AppendLine("        <BookgDt>");
            sb.AppendLine($"          <Dt>2024-01-{(15 + (i % 10)):D2}</Dt>");
            sb.AppendLine("        </BookgDt>");
            sb.AppendLine("        <ValDt>");
            sb.AppendLine($"          <Dt>2024-01-{(16 + (i % 10)):D2}</Dt>");
            sb.AppendLine("        </ValDt>");
            sb.AppendLine("      </Ntry>");
        }

        sb.AppendLine("    </Stmt>");
        sb.AppendLine("  </BkToCstmrStmt>");
        sb.AppendLine("</Document>");
        return sb.ToString();
    }

    [Fact]
    public async Task ParseEntriesAsync_WithCamt053V08_ShouldParseAllEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(5, Camt053V08Namespace);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var entries = new List<CamtEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(5);
        
        // Check first entry
        entries[0].EntryReference.ShouldBe("ENTRY-000001");
        entries[0].Amount.ShouldBe(101.00m);
        entries[0].Currency.ShouldBe("EUR");
        entries[0].CreditDebitIndicator.ShouldBe("DBIT");
        entries[0].Status.ShouldBe("BOOK");
        entries[0].BookingDate.ShouldNotBeNull();
        entries[0].ValueDate.ShouldNotBeNull();

        // Check last entry
        entries[4].EntryReference.ShouldBe("ENTRY-000005");
        entries[4].Amount.ShouldBe(105.00m);
        entries[4].CreditDebitIndicator.ShouldBe("DBIT");
    }

    [Fact]
    public async Task ParseEntriesAsync_WithCamt053V10_ShouldParseAllEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(3, Camt053V10Namespace);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var entries = new List<CamtEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(3);
        entries[0].EntryReference.ShouldBe("ENTRY-000001");
        entries[2].EntryReference.ShouldBe("ENTRY-000003");
    }

    [Fact]
    public async Task ParseWithContextAsync_ShouldParseHeaderCorrectly()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(
            entryCount: 10,
            statementId: "STMT-TEST-001",
            iban: "GB82WEST12345698765432");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await parser.ParseWithContextAsync(stream);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe(MessageIdentifier.Camt.V053_08);
        result.Header.ShouldBeOfType<Camt053Document>();

        var header = (Camt053Document)result.Header;
        header.StatementId.ShouldBe("STMT-TEST-001");
        header.AccountIban.ShouldBe("GB82WEST12345698765432");
        header.CreationDateTime.ShouldNotBeNull();
        header.NumberOfEntries.ShouldBe(10);

        result.ExpectedEntryCount.ShouldBe(10);

        // Verify entries can be enumerated
        var entries = new List<CamtEntry>();
        await foreach (var entry in result.Entries)
        {
            entries.Add(entry);
        }
        entries.Count.ShouldBe(10);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithProgressReporting_ShouldReportProgress()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var progressReports = new List<ParseProgress>();
        var progress = new Progress<ParseProgress>(p => progressReports.Add(p));

        var xml = GenerateCamt053Xml(2500);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var options = new ParseOptions { Progress = progress };

        // Act
        var count = 0;
        await foreach (var entry in parser.ParseEntriesAsync(stream, options))
        {
            count++;
        }

        // Assert
        count.ShouldBe(2500);
        progressReports.ShouldNotBeEmpty();
        progressReports.ShouldContain(p => p.Status == ParseStatus.Starting);
        progressReports.ShouldContain(p => p.Status == ParseStatus.Completed);
        
        // Should report progress at 1000, 2000 entries
        progressReports.ShouldContain(p => p.EntriesParsed == 1000);
        progressReports.ShouldContain(p => p.EntriesParsed == 2000);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithMaxEntries_ShouldLimitResults()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(100);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var options = new ParseOptions { MaxEntries = 25 };

        // Act
        var entries = new List<CamtEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream, options))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(25);
        entries[24].EntryReference.ShouldBe("ENTRY-000025");
    }

    [Fact]
    public async Task CountEntriesAsync_ShouldCountWithoutFullParsing()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(250);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var count = await parser.CountEntriesAsync(stream);

        // Assert
        count.ShouldBe(250);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithCancellation_ShouldStopParsing()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(1000);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        using var cts = new CancellationTokenSource();

        // Act
        var entries = new List<CamtEntry>();
        var exception = await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await foreach (var entry in parser.ParseEntriesAsync(stream, cancellationToken: cts.Token))
            {
                entries.Add(entry);
                if (entries.Count == 50)
                {
                    cts.Cancel();
                }
            }
        });

        // Assert
        entries.Count.ShouldBe(50);
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task ParseEntriesAsync_WithBatchProcessing_ShouldGroupEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(105);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var batches = new List<IReadOnlyList<CamtEntry>>();
        await foreach (var batch in parser.ParseEntriesAsync(stream).BatchAsync(50))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(3);
        batches[0].Count.ShouldBe(50);
        batches[1].Count.ShouldBe(50);
        batches[2].Count.ShouldBe(5);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithMemoryConstraint_ShouldMaintainConstantMemory()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        // Generate large XML with many entries
        var xml = GenerateCamt053Xml(5000);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var memoryBefore = GC.GetTotalMemory(true);

        // Act
        var processedCount = 0;
        decimal totalAmount = 0m;
        
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            processedCount++;
            totalAmount += entry.Amount;
            // Process entry without storing it
        }

        var memoryAfter = GC.GetTotalMemory(true);
        var memoryGrowth = memoryAfter - memoryBefore;

        // Assert
        processedCount.ShouldBe(5000);
        totalAmount.ShouldBeGreaterThan(0);
        
        // Memory growth should be minimal (< 15MB for 5k entries)
        // This is a soft assertion as GC behavior can vary
        memoryGrowth.ShouldBeLessThan(15 * 1024 * 1024);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithEmptyStatement_ShouldReturnNoEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(0);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var entries = new List<CamtEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            entries.Add(entry);
        }

        // Assert
        entries.ShouldBeEmpty();
    }

    [Fact]
    public void SupportedMessages_ShouldContainCamt053Versions()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        // Act
        var supportedMessages = parser.SupportedMessages;

        // Assert
        supportedMessages.ShouldNotBeEmpty();
        supportedMessages.ShouldContain(MessageIdentifier.Camt.V053_08);
        supportedMessages.ShouldContain(MessageIdentifier.Camt.V053_10);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithTakeExtension_ShouldPreviewEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new Camt053MessageDetector();
        var parser = new Camt053StreamingParser(
            xmlReaderFactory,
            messageDetector,
            NullLogger<Camt053StreamingParser>.Instance);

        var xml = GenerateCamt053Xml(1000);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act - Preview first 10 entries
        var entries = new List<CamtEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream).TakeAsync(10))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(10);
        entries[0].EntryReference.ShouldBe("ENTRY-000001");
        entries[9].EntryReference.ShouldBe("ENTRY-000010");
    }

    /// <summary>
    /// Mock message detector for CAMT.053 messages.
    /// </summary>
    private sealed class Camt053MessageDetector : IMessageDetector
    {
        public Task<MessageIdentifier> DetectAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MessageIdentifier.Camt.V053_08);
        }

        public Task<MessageDetectionResult> DetectWithDetailsAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MessageDetectionResult
            {
                MessageId = MessageIdentifier.Camt.V053_08,
                Namespace = Camt053V08Namespace,
                HasApplicationHeader = false,
                RootElementName = "Document"
            });
        }

        public Task<(bool Success, MessageIdentifier? MessageId, string? Error)> TryDetectAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<(bool, MessageIdentifier?, string?)>(
                (true, MessageIdentifier.Camt.V053_08, null));
        }
    }
}
