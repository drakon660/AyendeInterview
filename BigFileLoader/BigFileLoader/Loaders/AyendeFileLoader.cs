using System.Collections.Concurrent;
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
}

