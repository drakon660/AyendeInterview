using System.Diagnostics;
using System.Reflection;

namespace BigFileLoader;

public class Helper
{
    public static string GetFilePath(string fileName)
    {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;

        string assemblyDirectory = Path.GetDirectoryName(assemblyPath);

        string fourLevelsUp = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "..", "..", "..", fileName));

        return fourLevelsUp;
    }

    public static void Measure(Action action, string methodName)
    {
        var startTimeStamp = Stopwatch.GetTimestamp();

        action();

        var endTimeStamp = Stopwatch.GetTimestamp();

        var elapsed = Stopwatch.GetElapsedTime(startTimeStamp, endTimeStamp);

        Console.WriteLine($"{methodName} Elapsed time: {elapsed.TotalSeconds}");
    }

    public static async Task MeasureAsync(Func<Task> action, string methodName)
    {
        var startTimeStamp = Stopwatch.GetTimestamp();

        await action();

        var endTimeStamp = Stopwatch.GetTimestamp();

        var elapsed = Stopwatch.GetElapsedTime(startTimeStamp, endTimeStamp);

        Console.WriteLine($"{methodName} Elapsed time: {elapsed.TotalSeconds}");
    }
}