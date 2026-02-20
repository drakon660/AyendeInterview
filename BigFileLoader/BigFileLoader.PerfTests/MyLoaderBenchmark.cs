using BenchmarkDotNet.Attributes;

namespace BigFileLoader.PerfTests;

[RPlotExporter]
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class MyLoaderBenchmark
{
    private const string DataFileLocation = "F:\\src\\AyendeInterview\\data100.txt";
    
    [Benchmark]
    public Task AsyncForeach() => MyFileLoader.Analyze_Foreach_Async(DataFileLocation);
    
    [Benchmark]
    public void While() => MyFileLoader.Analyze_While(DataFileLocation);
   
    [Benchmark]
    public void AnalyzeV2() => MyFileLoader.AnalyzeV3(DataFileLocation);
    
    [Benchmark]
    public void AnalyzeV3() => MyFileLoader.AnalyzeV3(DataFileLocation);
    
    [Benchmark]
    public void AnalyzeV4() => MyFileLoader.AnalyzeV4(DataFileLocation);
    
    [Benchmark]
    public void AnalyzeV5() => MyFileLoader.AnalyzeV5(DataFileLocation);
    
}