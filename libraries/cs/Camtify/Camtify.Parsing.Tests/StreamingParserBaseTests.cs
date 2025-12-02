using System.Text;
using System.Xml;
using Camtify.Core;
using Camtify.Core.Parsing;
using Camtify.Parsing.Examples;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for the <see cref="StreamingParserBase{TDocument, TEntry}"/> class.
/// </summary>
public class StreamingParserBaseTests
{
    /// <summary>
    /// Mock streaming parser for testing.
    /// </summary>
    private sealed class TestStreamingParser : StreamingParserBase<TestDocument, TestEntry>
    {
        private readonly Func<XmlReader, CancellationToken, Task<TestEntry>>? _parseEntryFunc;
        private readonly Func<Stream, MessageIdentifier, CancellationToken, Task<object>>? _parseHeaderFunc;
        private readonly Func<object, int?>? _getExpectedEntryCountFunc;

        public override IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; } = new[]
        {
            new MessageIdentifier("test.001.001.01")
        };

        protected override string EntryElementName => "Entry";

        public TestStreamingParser(
            IXmlReaderFactory xmlReaderFactory,
            IMessageDetector messageDetector,
            Func<XmlReader, CancellationToken, Task<TestEntry>>? parseEntryFunc = null,
            Func<Stream, MessageIdentifier, CancellationToken, Task<object>>? parseHeaderFunc = null,
            Func<object, int?>? getExpectedEntryCountFunc = null)
            : base(xmlReaderFactory, messageDetector, NullLogger.Instance)
        {
            _parseEntryFunc = parseEntryFunc;
            _parseHeaderFunc = parseHeaderFunc;
            _getExpectedEntryCountFunc = getExpectedEntryCountFunc;
        }

        protected override async Task<TestEntry> ParseEntryAsync(
            XmlReader reader,
            CancellationToken cancellationToken)
        {
            if (_parseEntryFunc is not null)
            {
                return await _parseEntryFunc(reader, cancellationToken);
            }

            // Use pure XmlReader with depth tracking
            var entry = new TestEntry();

            if (reader.IsEmptyElement)
            {
                await reader.ReadAsync();
                return entry;
            }

            var entryDepth = reader.Depth;
            await reader.ReadAsync();

            while (reader.Depth > entryDepth)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "Id":
                            entry.Id = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken) ?? string.Empty;
                            break;
                        case "Value":
                            entry.Value = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken) ?? string.Empty;
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

        protected override async Task<object> ParseHeaderAsync(
            Stream stream,
            MessageIdentifier messageId,
            CancellationToken cancellationToken)
        {
            if (_parseHeaderFunc is not null)
            {
                return await _parseHeaderFunc(stream, messageId, cancellationToken);
            }

            return new TestDocument { MessageId = messageId.ToString() };
        }

        protected override int? GetExpectedEntryCount(object header)
        {
            if (_getExpectedEntryCountFunc is not null)
            {
                return _getExpectedEntryCountFunc(header);
            }

            if (header is TestDocument doc)
            {
                return doc.ExpectedEntryCount;
            }

            return null;
        }
    }

    /// <summary>
    /// Test document type.
    /// </summary>
    private sealed class TestDocument
    {
        public required string MessageId { get; init; }
        public int? ExpectedEntryCount { get; init; }
    }

    /// <summary>
    /// Test entry type.
    /// </summary>
    private sealed class TestEntry
    {
        public string Id { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    private static string GenerateTestXml(int entryCount)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<Document xmlns=\"urn:test\">");

        for (var i = 1; i <= entryCount; i++)
        {
            sb.AppendLine("  <Entry>");
            sb.AppendLine($"    <Id>ENTRY-{i:D4}</Id>");
            sb.AppendLine($"    <Value>Value {i}</Value>");
            sb.AppendLine("  </Entry>");
        }

        sb.AppendLine("</Document>");
        return sb.ToString();
    }

    [Fact]
    public async Task ParseEntriesAsync_WithValidXml_ShouldParseAllEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        var xml = GenerateTestXml(5);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(5);
        entries[0].Id.ShouldBe("ENTRY-0001");
        entries[0].Value.ShouldBe("Value 1");
        entries[4].Id.ShouldBe("ENTRY-0005");
        entries[4].Value.ShouldBe("Value 5");
    }

    [Fact]
    public async Task ParseEntriesAsync_WithEmptyDocument_ShouldReturnNoEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        var xml = GenerateTestXml(0);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            entries.Add(entry);
        }

        // Assert
        entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task ParseEntriesAsync_WithMaxEntries_ShouldLimitResults()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        var xml = GenerateTestXml(100);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var options = new ParseOptions { MaxEntries = 10 };

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream, options))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(10);
        entries[9].Id.ShouldBe("ENTRY-0010");
    }

    [Fact]
    public async Task ParseEntriesAsync_WithProgressReporting_ShouldReportProgress()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        var progressReports = new List<ParseProgress>();
        var progress = new Progress<ParseProgress>(p => progressReports.Add(p));

        var xml = GenerateTestXml(2500);
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
        progressReports.ShouldContain(p => p.Status == ParseStatus.ParsingEntries && p.EntriesParsed == 1000);
        progressReports.ShouldContain(p => p.Status == ParseStatus.ParsingEntries && p.EntriesParsed == 2000);
        progressReports.ShouldContain(p => p.Status == ParseStatus.Completed && p.EntriesParsed == 2500);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithCancellation_ShouldStopParsing()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        var xml = GenerateTestXml(1000);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        using var cts = new CancellationTokenSource();

        // Act
        var entries = new List<TestEntry>();
        var exception = await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await foreach (var entry in parser.ParseEntriesAsync(stream, cancellationToken: cts.Token))
            {
                entries.Add(entry);
                if (entries.Count == 10)
                {
                    cts.Cancel();
                }
            }
        });

        // Assert
        entries.Count.ShouldBe(10);
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task ParseEntriesAsync_WithStopOnFirstError_ShouldThrowOnError()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();

        var callCount = 0;
        var parser = new TestStreamingParser(
            xmlReaderFactory,
            messageDetector,
            parseEntryFunc: async (reader, ct) =>
            {
                callCount++;
                if (callCount == 3)
                {
                    throw new InvalidOperationException("Test error");
                }

                // Use pure XmlReader
                var entry = new TestEntry();
                if (reader.IsEmptyElement)
                {
                    await reader.ReadAsync();
                    return entry;
                }

                var entryDepth = reader.Depth;
                await reader.ReadAsync();

                while (reader.Depth > entryDepth)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case "Id":
                                entry.Id = await reader.ReadElementContentAsStringOrDefaultAsync(ct) ?? string.Empty;
                                break;
                            case "Value":
                                entry.Value = await reader.ReadElementContentAsStringOrDefaultAsync(ct) ?? string.Empty;
                                break;
                            default:
                                await reader.SkipElementAsync(ct);
                                break;
                        }
                    }
                    else
                    {
                        await reader.ReadAsync();
                    }
                }

                return entry;
            });

        var xml = GenerateTestXml(10);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var options = new ParseOptions { StopOnFirstError = true };

        // Act & Assert
        var entries = new List<TestEntry>();
        await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await foreach (var entry in parser.ParseEntriesAsync(stream, options))
            {
                entries.Add(entry);
            }
        });

        entries.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithoutStopOnFirstError_ShouldContinueOnError()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();

        var callCount = 0;
        var parser = new TestStreamingParser(
            xmlReaderFactory,
            messageDetector,
            parseEntryFunc: async (reader, ct) =>
            {
                callCount++;
                if (callCount == 3)
                {
                    throw new InvalidOperationException("Test error");
                }

                // Use pure XmlReader
                var entry = new TestEntry();
                if (reader.IsEmptyElement)
                {
                    await reader.ReadAsync();
                    return entry;
                }

                var entryDepth = reader.Depth;
                await reader.ReadAsync();

                while (reader.Depth > entryDepth)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case "Id":
                                entry.Id = await reader.ReadElementContentAsStringOrDefaultAsync(ct) ?? string.Empty;
                                break;
                            case "Value":
                                entry.Value = await reader.ReadElementContentAsStringOrDefaultAsync(ct) ?? string.Empty;
                                break;
                            default:
                                await reader.SkipElementAsync(ct);
                                break;
                        }
                    }
                    else
                    {
                        await reader.ReadAsync();
                    }
                }

                return entry;
            });

        var xml = GenerateTestXml(10);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var options = new ParseOptions { StopOnFirstError = false };

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in parser.ParseEntriesAsync(stream, options))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(9); // 10 entries, 1 failed
        entries[0].Id.ShouldBe("ENTRY-0001");
        entries[1].Id.ShouldBe("ENTRY-0002");
        entries[2].Id.ShouldBe("ENTRY-0004"); // Entry 3 failed
    }

    [Fact]
    public async Task ParseWithContextAsync_ShouldParseHeaderAndEntries()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(
            xmlReaderFactory,
            messageDetector,
            getExpectedEntryCountFunc: header =>
            {
                if (header is TestDocument doc)
                {
                    return doc.ExpectedEntryCount;
                }
                return null;
            });

        var xml = GenerateTestXml(5);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var result = await parser.ParseWithContextAsync(stream);

        // Assert
        result.ShouldNotBeNull();
        result.MessageId.ShouldBe(new MessageIdentifier("test.001.001.01"));
        result.Header.ShouldBeOfType<TestDocument>();
        result.Entries.ShouldNotBeNull();

        var entries = new List<TestEntry>();
        await foreach (var entry in result.Entries)
        {
            entries.Add(entry);
        }

        entries.Count.ShouldBe(5);
    }

    [Fact]
    public async Task CountEntriesAsync_ShouldCountWithoutFullParsing()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        var xml = GenerateTestXml(100);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var count = await parser.CountEntriesAsync(stream);

        // Assert
        count.ShouldBe(100);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithLargeFile_ShouldMaintainConstantMemory()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        // Generate large XML (10,000 entries)
        var xml = GenerateTestXml(10000);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var memoryBefore = GC.GetTotalMemory(true);

        // Act
        var processedCount = 0;
        await foreach (var entry in parser.ParseEntriesAsync(stream))
        {
            processedCount++;
            // Process entry without storing it
        }

        var memoryAfter = GC.GetTotalMemory(true);
        var memoryGrowth = memoryAfter - memoryBefore;

        // Assert
        processedCount.ShouldBe(10000);
        // Memory growth should be minimal (< 10MB for 10k entries)
        // This is a soft assertion as GC behavior can vary
        memoryGrowth.ShouldBeLessThan(10 * 1024 * 1024);
    }

    [Fact]
    public async Task ParseEntriesAsync_WithNullStream_ShouldThrowArgumentNullException()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await foreach (var entry in parser.ParseEntriesAsync(null!))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task ParseWithContextAsync_WithNullStream_ShouldThrowArgumentNullException()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await parser.ParseWithContextAsync(null!);
        });
    }

    [Fact]
    public async Task CountEntriesAsync_WithNullStream_ShouldThrowArgumentNullException()
    {
        // Arrange
        var xmlReaderFactory = new Iso20022XmlReaderFactory();
        var messageDetector = new TestMessageDetector();
        var parser = new TestStreamingParser(xmlReaderFactory, messageDetector);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await parser.CountEntriesAsync(null!);
        });
    }

    /// <summary>
    /// Test message detector that always returns a test message identifier.
    /// </summary>
    private sealed class TestMessageDetector : IMessageDetector
    {
        public Task<MessageIdentifier> DetectAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MessageIdentifier("test.001.001.01"));
        }

        public Task<MessageDetectionResult> DetectWithDetailsAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MessageDetectionResult
            {
                MessageId = new MessageIdentifier("test.001.001.01"),
                Namespace = "urn:test",
                HasApplicationHeader = false,
                RootElementName = "Document"
            });
        }

        public Task<(bool Success, MessageIdentifier? MessageId, string? Error)> TryDetectAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<(bool, MessageIdentifier?, string?)>(
                (true, new MessageIdentifier("test.001.001.01"), null));
        }
    }
}
