namespace Camtify.Parsing.Tests;

/// <summary>
/// Unit tests for the <see cref="StreamingParserExtensions"/> class.
/// </summary>
public class StreamingParserExtensionsTests
{
    private sealed class TestEntry
    {
        public required int Id { get; init; }
        public required string Value { get; init; }
    }

    private static async IAsyncEnumerable<TestEntry> GenerateEntries(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            await Task.Delay(1); // Simulate async work
            yield return new TestEntry { Id = i, Value = $"Value {i}" };
        }
    }

    [Fact]
    public async Task BatchAsync_WithValidBatchSize_ShouldGroupEntriesCorrectly()
    {
        // Arrange
        var source = GenerateEntries(10);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.BatchAsync(3))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(4);
        batches[0].Count.ShouldBe(3);
        batches[1].Count.ShouldBe(3);
        batches[2].Count.ShouldBe(3);
        batches[3].Count.ShouldBe(1); // Remainder
        batches[3][0].Id.ShouldBe(10);
    }

    [Fact]
    public async Task BatchAsync_WithExactBatchSize_ShouldHaveNoRemainder()
    {
        // Arrange
        var source = GenerateEntries(9);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.BatchAsync(3))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(3);
        batches.All(b => b.Count == 3).ShouldBeTrue();
    }

    [Fact]
    public async Task BatchAsync_WithEmptySource_ShouldReturnNoBatches()
    {
        // Arrange
        var source = GenerateEntries(0);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.BatchAsync(5))
        {
            batches.Add(batch);
        }

        // Assert
        batches.ShouldBeEmpty();
    }

    [Fact]
    public async Task BatchAsync_WithSingleItem_ShouldReturnOneBatch()
    {
        // Arrange
        var source = GenerateEntries(1);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.BatchAsync(5))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(1);
        batches[0].Count.ShouldBe(1);
        batches[0][0].Id.ShouldBe(1);
    }

    [Fact]
    public async Task BatchAsync_WithBatchSizeOne_ShouldReturnIndividualItems()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.BatchAsync(1))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(5);
        batches.All(b => b.Count == 1).ShouldBeTrue();
    }

    [Fact]
    public async Task BatchAsync_WithLargeBatchSize_ShouldReturnAllInOneBatch()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.BatchAsync(100))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(1);
        batches[0].Count.ShouldBe(5);
    }

    [Fact]
    public async Task BatchAsync_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        IAsyncEnumerable<TestEntry> source = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await foreach (var batch in source.BatchAsync(5))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task BatchAsync_WithZeroBatchSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var batch in source.BatchAsync(0))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task BatchAsync_WithNegativeBatchSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var batch in source.BatchAsync(-1))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task BatchAsync_WithCancellation_ShouldStopProcessing()
    {
        // Arrange
        var source = GenerateEntries(100);
        using var cts = new CancellationTokenSource();

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        var exception = await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await foreach (var batch in source.BatchAsync(10, cts.Token))
            {
                batches.Add(batch);
                if (batches.Count == 3)
                {
                    cts.Cancel();
                }
            }
        });

        // Assert
        batches.Count.ShouldBe(3);
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task ProcessInParallelAsync_ShouldProcessAllEntries()
    {
        // Arrange
        var source = GenerateEntries(20);
        var processed = new List<int>();
        var lockObj = new object();

        // Act
        await source.ProcessInParallelAsync(
            async (entry, ct) =>
            {
                await Task.Delay(10, ct); // Simulate work
                lock (lockObj)
                {
                    processed.Add(entry.Id);
                }
            },
            maxDegreeOfParallelism: 4);

        // Assert
        processed.Count.ShouldBe(20);
        processed.OrderBy(x => x).SequenceEqual(Enumerable.Range(1, 20)).ShouldBeTrue();
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithMaxDegreeOfParallelism_ShouldRespectLimit()
    {
        // Arrange
        var source = GenerateEntries(10);
        var concurrentCount = 0;
        var maxConcurrent = 0;
        var lockObj = new object();

        // Act
        await source.ProcessInParallelAsync(
            async (entry, ct) =>
            {
                lock (lockObj)
                {
                    concurrentCount++;
                    if (concurrentCount > maxConcurrent)
                    {
                        maxConcurrent = concurrentCount;
                    }
                }

                await Task.Delay(50, ct);

                lock (lockObj)
                {
                    concurrentCount--;
                }
            },
            maxDegreeOfParallelism: 3);

        // Assert
        maxConcurrent.ShouldBeLessThanOrEqualTo(3);
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        IAsyncEnumerable<TestEntry> source = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await source.ProcessInParallelAsync(async (entry, ct) => await Task.CompletedTask);
        });
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithNullProcessor_ShouldThrowArgumentNullException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await source.ProcessInParallelAsync(null!);
        });
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithZeroMaxDegreeOfParallelism_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await source.ProcessInParallelAsync(
                async (entry, ct) => await Task.CompletedTask,
                maxDegreeOfParallelism: 0);
        });
    }

    [Fact]
    public async Task BufferAsync_ShouldBufferEntries()
    {
        // Arrange
        var source = GenerateEntries(10);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.BufferAsync(5))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(10);
        entries.Select(e => e.Id).SequenceEqual(Enumerable.Range(1, 10)).ShouldBeTrue();
    }

    [Fact]
    public async Task BufferAsync_WithEmptySource_ShouldReturnNoEntries()
    {
        // Arrange
        var source = GenerateEntries(0);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.BufferAsync(5))
        {
            entries.Add(entry);
        }

        // Assert
        entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task BufferAsync_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        IAsyncEnumerable<TestEntry> source = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await foreach (var entry in source.BufferAsync(5))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task BufferAsync_WithZeroBufferSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var entry in source.BufferAsync(0))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task TakeAsync_ShouldLimitEntries()
    {
        // Arrange
        var source = GenerateEntries(100);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.TakeAsync(10))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(10);
        entries[0].Id.ShouldBe(1);
        entries[9].Id.ShouldBe(10);
    }

    [Fact]
    public async Task TakeAsync_WithZeroCount_ShouldReturnNoEntries()
    {
        // Arrange
        var source = GenerateEntries(10);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.TakeAsync(0))
        {
            entries.Add(entry);
        }

        // Assert
        entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task TakeAsync_WithCountGreaterThanSource_ShouldReturnAllEntries()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.TakeAsync(100))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(5);
    }

    [Fact]
    public async Task TakeAsync_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        IAsyncEnumerable<TestEntry> source = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await foreach (var entry in source.TakeAsync(5))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task TakeAsync_WithNegativeCount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var entry in source.TakeAsync(-1))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task SkipAsync_ShouldSkipEntries()
    {
        // Arrange
        var source = GenerateEntries(10);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.SkipAsync(5))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(5);
        entries[0].Id.ShouldBe(6);
        entries[4].Id.ShouldBe(10);
    }

    [Fact]
    public async Task SkipAsync_WithZeroCount_ShouldReturnAllEntries()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.SkipAsync(0))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(5);
    }

    [Fact]
    public async Task SkipAsync_WithCountGreaterThanSource_ShouldReturnNoEntries()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.SkipAsync(100))
        {
            entries.Add(entry);
        }

        // Assert
        entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task SkipAsync_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        IAsyncEnumerable<TestEntry> source = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await foreach (var entry in source.SkipAsync(5))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task SkipAsync_WithNegativeCount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var source = GenerateEntries(5);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var entry in source.SkipAsync(-1))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task CombinedExtensions_SkipTakeBatch_ShouldWorkTogether()
    {
        // Arrange
        var source = GenerateEntries(100);

        // Act
        var batches = new List<IReadOnlyList<TestEntry>>();
        await foreach (var batch in source.SkipAsync(10).TakeAsync(20).BatchAsync(5))
        {
            batches.Add(batch);
        }

        // Assert
        batches.Count.ShouldBe(4);
        batches.All(b => b.Count == 5).ShouldBeTrue();
        batches[0][0].Id.ShouldBe(11); // Skipped first 10
        batches[3][4].Id.ShouldBe(30); // Took 20 entries
    }

    [Fact]
    public async Task CombinedExtensions_TakeBuffer_ShouldWorkTogether()
    {
        // Arrange
        var source = GenerateEntries(100);

        // Act
        var entries = new List<TestEntry>();
        await foreach (var entry in source.TakeAsync(10).BufferAsync(3))
        {
            entries.Add(entry);
        }

        // Assert
        entries.Count.ShouldBe(10);
        entries.Select(e => e.Id).SequenceEqual(Enumerable.Range(1, 10)).ShouldBeTrue();
    }
}
