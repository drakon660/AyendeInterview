using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BigFileLoader.Models;

namespace BigFileLoader;

public static class AyendeFileLoader
{
    public static IEnumerable<dynamic> Analyze(string filePath)
    {
        var summary = from line in File.ReadAllLines(filePath)
            let record = new RecordV1(line)
            group record by record.Id
            into g
            select new
            {
                Id = g.Key,
                Duration = TimeSpan.FromTicks(g.Sum(r => r.Duration.Ticks))
            };

        return summary.ToList();
    }

    public static IEnumerable<dynamic> AnalyzeV2(string filePath)
    {
        var summary = from line in File.ReadAllLines(filePath)
            let record = new RecordV2(line)
            group record by record.Id
            into g
            select new
            {
                Id = g.Key,
                Duration = TimeSpan.FromTicks(g.Sum(r => r.Duration.Ticks))
            };

        return summary.ToList();
    }
    
    //This is not faster than without parallel (Benchmarkdotnet)
    public static IEnumerable<dynamic> AnalyzeV2_Parallel(string filePath)
    {
        var summary = from line in File.ReadAllLines(filePath).AsParallel()
            let record = new RecordV2(line)
            group record by record.Id
            into g
            select new
            {
                Id = g.Key,
                Duration = TimeSpan.FromTicks(g.Sum(r => r.Duration.Ticks))
            };

        return summary.ToList();
    }

    //v3 - Read the file line by line with a StreamReader / internal dictionary for statistics / default .net parsing for values
    public static IDictionary<long, long> AnalyzeV3(string filePath)
    {
        var stats = new Dictionary<long, long>();

        foreach (var line in File.ReadLines(filePath))
        {
            var parts = line.Split(' ');
            var duration = (DateTime.Parse(parts[1]) - DateTime.Parse(parts[0])).Ticks;
            var id = long.Parse(parts[2]);
            
            stats.TryGetValue(id, out long existingDuration);
            stats[id] = existingDuration + duration;
        }

        return stats;
    }

    //this is not faster than without parallel
    public static IDictionary<long, long> AnalyzeV3_Parallel(string filePath)
    {
        var stats = new ConcurrentDictionary<long, long>();
        Parallel.ForEach(File.ReadLines(filePath), line =>
        {
            var parts = line.Split(' ');
            var duration = (DateTime.Parse(parts[1]) - DateTime.Parse(parts[0])).Ticks;
            var id = long.Parse(parts[2]);
            stats.AddOrUpdate(id, duration, (_, existingDuration) => existingDuration + duration);

        });
        return stats;
    }

    //v4 - Read the file line by line with a StreamReader / internal dictionary for statistics / parsing optimizations
    public static IDictionary<string, long> AnalyzeV4(string filePath)
    {
        StreamReader file = new StreamReader(filePath);
        string line;

        IDictionary<string, long> totalDuration = new Dictionary<string, long>();

        while ((line = file.ReadLine()) != null)
        {
            string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

            string id = lineItems[2];
            DateTime start = DateTime.ParseExact(lineItems[0], "yyyy-MM-dd'T'HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime end = DateTime.ParseExact(lineItems[1], "yyyy-MM-dd'T'HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);

            long duration = (end - start).Ticks;

            if (totalDuration.ContainsKey(id))
                totalDuration[id] += duration;
            else
                totalDuration.Add(id, duration);
        }

        file.Close();

        return totalDuration;
    }

    //v5- Read the file line by line with a StreamReader / internal dictionary for statistics / custom date parsing
    public static IDictionary<string, long> AnalyzeV5(string filePath)
    {
        StreamReader file = new StreamReader(filePath);
        string line;
        Dictionary<string, long> totalDuration = new Dictionary<string, long>();

        while ((line = file.ReadLine()) != null)
        {
            string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

            string id = lineItems[2];
            DateTime start = DateMagic.ParseDate(lineItems[0]);
            DateTime end = DateMagic.ParseDate(lineItems[1]);

            long duration = (end - start).Ticks;

            if (totalDuration.ContainsKey(id))
                totalDuration[id] += duration;
            else
                totalDuration.Add(id, duration);
        }

        file.Close();
        return totalDuration;
    }

    public static IDictionary<string, long> AnalyzeV6(string filePath)
    {
        StreamReader file = new StreamReader(filePath);
        string line;
        Dictionary<string, long> totalDuration = new Dictionary<string, long>();

        while ((line = file.ReadLine()) != null)
        {
            string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

            string id = lineItems[2];
            long start = DateMagic.ParseDate2(lineItems[0]);
            long end = DateMagic.ParseDate2(lineItems[1]);

            long duration = end - start;

            if (totalDuration.ContainsKey(id))
                totalDuration[id] += duration;
            else
                totalDuration.Add(id, duration);
        }

        file.Close();
        return totalDuration;
    }
    
    public static IDictionary<string, long> AnalyzeV7(string filePath)
    {
        ConcurrentDictionary<string, long> totalDuration = new ConcurrentDictionary<string, long>();

        Parallel.ForEach(File.ReadLines(filePath), line =>
        {
            string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

            string id = lineItems[2];
            DateTime start = DateMagic.ParseDate(lineItems[0]);
            DateTime end = DateMagic.ParseDate(lineItems[1]);

            long duration = (end - start).Ticks;

            totalDuration.AddOrUpdate(id, duration, (_, existingDuration) => existingDuration + duration);
        });

        return totalDuration;
    }
 
    public static IDictionary<long, FastRecord> AnalyzeV8(string filePath)
    {
        var stats = new Dictionary<long, FastRecord>();
        using var reader = new RecordReader(filePath);
        while (reader.MoveNext())
            
        {
            if (stats.TryGetValue(reader.Id, out FastRecord value) == false)
            {
                stats[reader.Id] = value = new FastRecord
                {
                    Id = reader.Id
                };
            }
            value.DurationInTicks += reader.Duration;
        }

        return stats;
    }

    public static IDictionary<long, long> AnalyzeV9(string filePath)
    {
        var stats = new Dictionary<long, long>();
        using var reader = new RecordReader(filePath);
        while (reader.MoveNext())
        {
            stats.TryGetValue(reader.Id, out long existingDuration);
            stats[reader.Id] = existingDuration + reader.Duration;
        }

        return stats;
    }

    // V10: raw FileStream bytes (no StreamReader/UTF-8 decode), 1 MB batch reads,
    //      CollectionsMarshal for a single dictionary probe per record,
    //      unrolled 8-digit ID parse.
    public static IDictionary<long, long> AnalyzeV10(string filePath)
    {
        var stats = new Dictionary<long, long>();
        const int recordSize = Constants.SizeOfRecord; // 50 bytes per record (19+1+19+1+8+2)
        const int bufferSize = 1 << 20;                // 1 MB (1,048,576 bytes) processing buffer

        // Open file as raw bytes — bypasses StreamReader's UTF-8 decode overhead entirely.
        // SequentialScan hints the OS to prefetch pages ahead for sequential reads.
        using var fs = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 1 << 16,           // 64 KB FileStream internal buffer
            FileOptions.SequentialScan);

        var buffer = new byte[bufferSize];
        int leftover = 0; // bytes from previous read that didn't form a complete record

        while (true)
        {
            // Read into buffer after any leftover bytes from the previous iteration
            int read = fs.Read(buffer, leftover, bufferSize - leftover);
            int total = leftover + read; // total processable bytes in buffer
            int pos = 0;

            // Process all complete 50-byte records in the buffer
            while (pos + recordSize <= total)
            {
                // Parse entry timestamp at pos+0, exit timestamp at pos+20, car ID at pos+40
                long entryTicks = DateMagic.ByteParseTicks(buffer, pos);
                long exitTicks  = DateMagic.ByteParseTicks(buffer, pos + 20);
                long id         = ByteParseId(buffer, pos + 40);

                // Single dictionary probe: returns a ref to the value slot,
                // creating a new entry (default 0) if the key doesn't exist yet.
                // This replaces the TryGetValue + indexer pattern (2 probes) used in V9.
                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, id, out _);
                val += exitTicks - entryTicks;

                pos += recordSize;
            }

            // Carry over any incomplete record bytes to the start of the buffer
            // for the next read iteration (e.g. if 1MB boundary splits a record)
            leftover = total - pos;
            if (leftover > 0 && pos > 0)
                Buffer.BlockCopy(buffer, pos, buffer, 0, leftover);

            // read == 0 means EOF — all remaining complete records were already processed above
            if (read == 0) break;
        }

        return stats;
    }

    // V11: ByteParseDuration (single call computes exit - entry directly in int arithmetic),
    //      pre-sized dictionary, no validation branches, ReadOnlySpan month tables.
    public static IDictionary<long, long> AnalyzeV11(string filePath)
    {
        const int recordSize = Constants.SizeOfRecord;
        const int bufferSize = 1 << 20;

        using var fs = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 1 << 16,
            FileOptions.SequentialScan);

        // Pre-size dictionary to reduce rehashing during growth.
        // Estimate ~5% of records are unique car IDs. If the actual count exceeds this,
        // Dictionary resizes automatically — we just lose the pre-sizing benefit.
        // If it's fewer, we waste some empty buckets but avoid early rehashes.
        int estimatedRecords = (int)(fs.Length / recordSize);
        var stats = new Dictionary<long, long>(estimatedRecords / 20);

        var buffer = new byte[bufferSize];
        int leftover = 0;

        while (true)
        {
            int read = fs.Read(buffer, leftover, bufferSize - leftover);
            int total = leftover + read;
            int pos = 0;

            while (pos + recordSize <= total)
            {
                // Single call parses both timestamps and computes exit - entry directly.
                // Keeps all intermediate values as int (32-bit), only widens to long
                // for the final 2 multiplications (days * TicksPerDay + secs * TicksPerSecond).
                // V10's ByteParseTicks computed 2 absolute tick values (4 long muls) then subtracted.
                long duration = DateMagic.ByteParseDuration(buffer, pos, pos + 20);
                long id       = ByteParseId(buffer, pos + 40);

                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, id, out _);
                val += duration;

                pos += recordSize;
            }

            leftover = total - pos;
            if (leftover > 0 && pos > 0)
                Buffer.BlockCopy(buffer, pos, buffer, 0, leftover);

            if (read == 0) break;
        }

        return stats;
    }

    // V12: Same as V11 but uses ByteParseDurationFast — same-day fast path
    // that skips DaysSinceEpoch for records where entry and exit share the same date.
    // Still managed code (no unsafe), still uses FileStream with 1 MB buffer.
    public static IDictionary<long, long> AnalyzeV12(string filePath)
    {
        const int recordSize = Constants.SizeOfRecord;
        const int bufferSize = 1 << 20;

        using var fs = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 1 << 16,
            FileOptions.SequentialScan);

        int estimatedRecords = (int)(fs.Length / recordSize);
        var stats = new Dictionary<long, long>(estimatedRecords / 20);

        var buffer = new byte[bufferSize];
        int leftover = 0;

        while (true)
        {
            int read = fs.Read(buffer, leftover, bufferSize - leftover);
            int total = leftover + read;
            int pos = 0;

            while (pos + recordSize <= total)
            {
                // ByteParseDurationFast compares date bytes first (long+short = 10 bytes).
                // Same day → only parses 6 time digits + 1 long mul.
                // Different day → falls back to full ByteParseDuration.
                long duration = DateMagic.ByteParseDurationFast(buffer, pos, pos + 20);
                long id       = ByteParseId(buffer, pos + 40);

                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, id, out _);
                val += duration;

                pos += recordSize;
            }

            leftover = total - pos;
            if (leftover > 0 && pos > 0)
                Buffer.BlockCopy(buffer, pos, buffer, 0, leftover);

            if (read == 0) break;
        }

        return stats;
    }

    // V13: Memory-mapped file + unsafe pointer access.
    // MemoryMappedFile maps the file into virtual memory — the OS handles paging/prefetch.
    // No buffer management, no BlockCopy for leftovers, no FileStream.Read calls.
    // All byte access goes through raw byte* pointers — zero bounds checks.
    // The loop is a simple pointer walk: p += 50 per record.
    public static unsafe IDictionary<long, long> AnalyzeV13(string filePath)
    {
        const int recordSize = Constants.SizeOfRecord;

        long fileLength = new FileInfo(filePath).Length;
        int recordCount = (int)(fileLength / recordSize);
        var stats = new Dictionary<long, long>(recordCount / 20);

        // Map the entire file into virtual memory as read-only.
        // mapName: null = anonymous mapping (not shared with other processes).
        // capacity: 0 = use the file's actual size.
        using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        // AcquirePointer gives us a raw byte* into the mapped memory region.
        // Must be paired with ReleasePointer in a finally block.
        byte* basePtr = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref basePtr);
        try
        {
            // PointerOffset accounts for OS page alignment adjustments.
            // When offset is 0, this is typically 0, but included for correctness.
            basePtr += accessor.PointerOffset;
            byte* end = basePtr + (long)recordCount * recordSize;

            for (byte* p = basePtr; p < end; p += recordSize)
            {
                long duration = DateMagic.UnsafeParseDuration(p);
                long id       = UnsafeParseId(p + 40);

                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, id, out _);
                val += duration;
            }
        }
        finally
        {
            accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        }

        return stats;
    }

    // V14: Memory-mapped file + unsafe pointers + same-day fast path.
    // Combines V13's zero-copy memory-mapped access with V12's date comparison trick.
    // The same-day check uses *(long*) and *(short*) pointer casts — 2 comparisons
    // for 10 bytes, zero overhead compared to individual byte checks.
    public static unsafe IDictionary<long, long> AnalyzeV14(string filePath)
    {
        const int recordSize = Constants.SizeOfRecord;

        long fileLength = new FileInfo(filePath).Length;
        int recordCount = (int)(fileLength / recordSize);
        var stats = new Dictionary<long, long>(recordCount / 20);

        using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        byte* basePtr = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref basePtr);
        try
        {
            basePtr += accessor.PointerOffset;
            byte* end = basePtr + (long)recordCount * recordSize;

            for (byte* p = basePtr; p < end; p += recordSize)
            {
                // UnsafeParseDurationFast: pointer-cast date comparison +
                // same-day shortcut (time-only parse, 1 long mul) or
                // full DaysSinceEpoch fallback for overnight stays.
                long duration = DateMagic.UnsafeParseDurationFast(p);
                long id       = UnsafeParseId(p + 40);

                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, id, out _);
                val += duration;
            }
        }
        finally
        {
            accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        }

        return stats;
    }

    // Managed: unrolled 8-digit car ID parse from byte[] array.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ByteParseId(byte[] buf, int pos) =>
        (long)(buf[pos]     - '0') * 10_000_000 +
        (long)(buf[pos + 1] - '0') *  1_000_000 +
        (long)(buf[pos + 2] - '0') *    100_000 +
        (long)(buf[pos + 3] - '0') *     10_000 +
        (long)(buf[pos + 4] - '0') *      1_000 +
        (long)(buf[pos + 5] - '0') *        100 +
        (long)(buf[pos + 6] - '0') *         10 +
        (long)(buf[pos + 7] - '0');

    // Unsafe: unrolled 8-digit car ID parse from raw byte* pointer.
    // Same logic as ByteParseId but with zero bounds checks.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe long UnsafeParseId(byte* p) =>
        (long)(p[0] - '0') * 10_000_000 +
        (long)(p[1] - '0') *  1_000_000 +
        (long)(p[2] - '0') *    100_000 +
        (long)(p[3] - '0') *     10_000 +
        (long)(p[4] - '0') *      1_000 +
        (long)(p[5] - '0') *        100 +
        (long)(p[6] - '0') *         10 +
        (long)(p[7] - '0');

    // V15: Parallel processing with thread-local dictionaries (fully managed, no unsafe).
    //
    // Why previous parallel attempts (V7, V3_Parallel) failed:
    //   ConcurrentDictionary lock contention killed all parallelism gains — every thread
    //   fought over the same buckets on every record.
    //
    // This approach avoids contention entirely:
    //   1. Read entire file into byte[] (one allocation, ~100 MB for data100.txt)
    //   2. Partition into N chunks by simple arithmetic (records are fixed 50 bytes)
    //      — no scanning, no data copying, just: startByte = chunkIndex * recordsPerChunk * 50
    //   3. Each thread processes its chunk into its own Dictionary<long, long> — zero contention
    //   4. Merge N small dictionaries into one at the end (single-threaded, O(N * uniqueKeys))
    //
    // On a 16-core CPU with the file in OS page cache, the CPU-bound parsing
    // scales nearly linearly across cores.
    public static IDictionary<long, long> AnalyzeV15(string filePath)
    {
        const int recordSize = Constants.SizeOfRecord;

        // Read entire file into memory so all threads can access it without synchronization.
        // After the first benchmark iteration the file is in OS page cache, so this is fast.
        byte[] data = File.ReadAllBytes(filePath);

        int totalRecords = data.Length / recordSize;
        int threadCount = Environment.ProcessorCount;

        // Each thread gets its own dictionary — no ConcurrentDictionary, no locks.
        var localDicts = new Dictionary<long, long>[threadCount];

        Parallel.For(0, threadCount, i =>
        {
            // Compute this thread's byte range by simple arithmetic.
            // Last thread picks up any remainder records from integer division.
            int startRecord = i * (totalRecords / threadCount);
            int endRecord = (i == threadCount - 1)
                ? totalRecords
                : (i + 1) * (totalRecords / threadCount);

            int startByte = startRecord * recordSize;
            int endByte = endRecord * recordSize;

            var local = new Dictionary<long, long>();

            for (int pos = startByte; pos < endByte; pos += recordSize)
            {
                // Same parsing as V12: same-day fast path + int arithmetic
                long duration = DateMagic.ByteParseDurationFast(data, pos, pos + 20);
                long id       = ByteParseId(data, pos + 40);

                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(local, id, out _);
                val += duration;
            }

            localDicts[i] = local;
        });

        // Merge phase: combine all thread-local dictionaries into the first one.
        // This is O(threadCount * uniqueKeysPerThread) — small compared to the parsing work.
        // For 100K unique cars across 16 threads, this is ~1.6M dictionary operations total,
        // vs 2M records parsed — and it runs single-threaded so no contention.
        var stats = localDicts[0];
        for (int i = 1; i < threadCount; i++)
        {
            foreach (var kvp in localDicts[i])
            {
                ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, kvp.Key, out _);
                val += kvp.Value;
            }
        }

        return stats;
    }

    // V16: Memory-mapped file + parallel chunked processing + unsafe pointers.
    // Combines V14's zero-copy mmap (no managed heap allocation for file data)
    // with V15's contention-free thread-local dictionaries.
    // Memory usage: only the N small dictionaries (~MB), not the file (~GB).
    public static unsafe IDictionary<long, long> AnalyzeV16(string filePath)
    {
        const int recordSize = Constants.SizeOfRecord;

        long fileLength = new FileInfo(filePath).Length;
        int totalRecords = (int)(fileLength / recordSize);
        int threadCount = Environment.ProcessorCount;
        var localDicts = new Dictionary<long, long>[threadCount];

        using var mmf = MemoryMappedFile.CreateFromFile(
            filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        byte* basePtr = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref basePtr);
        try
        {
            basePtr += accessor.PointerOffset;

            Parallel.For(0, threadCount, i =>
            {
                int startRecord = i * (totalRecords / threadCount);
                int endRecord = (i == threadCount - 1)
                    ? totalRecords
                    : (i + 1) * (totalRecords / threadCount);

                byte* p   = basePtr + (long)startRecord * recordSize;
                byte* end = basePtr + (long)endRecord * recordSize;

                var local = new Dictionary<long, long>();
                for (; p < end; p += recordSize)
                {
                    long duration = DateMagic.UnsafeParseDurationFast(p);
                    long id       = UnsafeParseId(p + 40);

                    ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(local, id, out _);
                    val += duration;
                }
                localDicts[i] = local;
            });

            var stats = localDicts[0];
            for (int i = 1; i < threadCount; i++)
            {
                foreach (var kvp in localDicts[i])
                {
                    ref long val = ref CollectionsMarshal.GetValueRefOrAddDefault(stats, kvp.Key, out _);
                    val += kvp.Value;
                }
            }

            return stats;
        }
        finally
        {
            accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        }
    }
}

