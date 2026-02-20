using BenchmarkDotNet.Attributes;

namespace BigFileLoader.PerfTests;

[RPlotExporter]
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class AyendeLoaderBenchmark
{
    private const string DataFileLocation = "F:\\src\\AyendeInterview\\data100.txt";
    
    [Benchmark]
    public void Ayende_V1() => AyendeFileLoader.Analyze(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V2() => AyendeFileLoader.AnalyzeV2(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V2_Parallel() => AyendeFileLoader.AnalyzeV2_Parallel(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V3() => AyendeFileLoader.AnalyzeV3(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V4() => AyendeFileLoader.AnalyzeV4(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V5() => AyendeFileLoader.AnalyzeV5(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V6() => AyendeFileLoader.AnalyzeV6(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V7() => AyendeFileLoader.AnalyzeV7(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V8() => AyendeFileLoader.AnalyzeV8(DataFileLocation);
}