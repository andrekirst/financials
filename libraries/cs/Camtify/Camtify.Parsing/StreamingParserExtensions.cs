using System.Runtime.CompilerServices;

namespace Camtify.Parsing;

/// <summary>
/// Extension methods for streaming parsers.
/// </summary>
public static class StreamingParserExtensions
{
    /// <summary>
    /// Groups entries into batches for efficient database inserts.
    /// </summary>
    /// <typeparam name="TEntry">The entry type.</typeparam>
    /// <param name="source">The source async enumerable.</param>
    /// <param name="batchSize">The batch size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of batches.</returns>
    /// <remarks>
    /// This is useful for bulk database operations where inserting
    /// entries one-by-one would be inefficient.
    /// </remarks>
    /// <example>
    /// <code>
    /// await foreach (var batch in entries.BatchAsync(100))
    /// {
    ///     await dbContext.BulkInsertAsync(batch);
    /// }
    /// </code>
    /// </example>
    public static async IAsyncEnumerable<IReadOnlyList<TEntry>> BatchAsync<TEntry>(
        this IAsyncEnumerable<TEntry> source,
        int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(batchSize, 0);

        var batch = new List<TEntry>(batchSize);

        await foreach (var entry in source.WithCancellation(cancellationToken))
        {
            batch.Add(entry);

            if (batch.Count >= batchSize)
            {
                yield return batch;
                batch = new List<TEntry>(batchSize);
            }
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }

    /// <summary>
    /// Processes entries in parallel (with optional order preservation).
    /// </summary>
    /// <typeparam name="TEntry">The entry type.</typeparam>
    /// <param name="source">The source async enumerable.</param>
    /// <param name="processor">The processing function.</param>
    /// <param name="maxDegreeOfParallelism">Maximum degree of parallelism.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the completion of all processing.</returns>
    /// <remarks>
    /// This method uses <see cref="Parallel.ForEachAsync"/> to process entries
    /// in parallel while respecting the specified degree of parallelism.
    /// <para>
    /// <strong>WARNING:</strong> This method buffers entries internally for parallel processing,
    /// which increases memory usage. For truly constant-memory processing,
    /// use <see cref="BatchAsync{TEntry}"/> with sequential processing instead.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// await entries.ProcessInParallelAsync(
    ///     async (entry, ct) => await ProcessEntryAsync(entry, ct),
    ///     maxDegreeOfParallelism: 4);
    /// </code>
    /// </example>
    public static async Task ProcessInParallelAsync<TEntry>(
        this IAsyncEnumerable<TEntry> source,
        Func<TEntry, CancellationToken, Task> processor,
        int maxDegreeOfParallelism = 4,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(processor);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxDegreeOfParallelism, 0);

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(
            source,
            options,
            async (entry, ct) => await processor(entry, ct));
    }

    /// <summary>
    /// Buffers entries and yields them in chunks for improved throughput.
    /// </summary>
    /// <typeparam name="TEntry">The entry type.</typeparam>
    /// <param name="source">The source async enumerable.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable with buffered entries.</returns>
    /// <remarks>
    /// Buffering can improve throughput by reducing context switches,
    /// but increases memory usage proportional to buffer size.
    /// </remarks>
    public static async IAsyncEnumerable<TEntry> BufferAsync<TEntry>(
        this IAsyncEnumerable<TEntry> source,
        int bufferSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(bufferSize, 0);

        var buffer = new List<TEntry>(bufferSize);

        await foreach (var entry in source.WithCancellation(cancellationToken))
        {
            buffer.Add(entry);

            if (buffer.Count >= bufferSize)
            {
                foreach (var item in buffer)
                {
                    yield return item;
                }
                buffer.Clear();
            }
        }

        foreach (var item in buffer)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Takes only the first N entries from the stream.
    /// </summary>
    /// <typeparam name="TEntry">The entry type.</typeparam>
    /// <param name="source">The source async enumerable.</param>
    /// <param name="count">The number of entries to take.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable with at most N entries.</returns>
    /// <example>
    /// <code>
    /// // Preview first 10 entries
    /// await foreach (var entry in entries.TakeAsync(10))
    /// {
    ///     Console.WriteLine(entry);
    /// }
    /// </code>
    /// </example>
    public static async IAsyncEnumerable<TEntry> TakeAsync<TEntry>(
        this IAsyncEnumerable<TEntry> source,
        int count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

        var taken = 0;
        await foreach (var entry in source.WithCancellation(cancellationToken))
        {
            if (taken >= count)
            {
                yield break;
            }

            yield return entry;
            taken++;
        }
    }

    /// <summary>
    /// Skips the first N entries from the stream.
    /// </summary>
    /// <typeparam name="TEntry">The entry type.</typeparam>
    /// <param name="source">The source async enumerable.</param>
    /// <param name="count">The number of entries to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable starting after N entries.</returns>
    public static async IAsyncEnumerable<TEntry> SkipAsync<TEntry>(
        this IAsyncEnumerable<TEntry> source,
        int count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

        var skipped = 0;
        await foreach (var entry in source.WithCancellation(cancellationToken))
        {
            if (skipped < count)
            {
                skipped++;
                continue;
            }

            yield return entry;
        }
    }
}
