using BenchmarkDotNet.Attributes;

namespace BigFileLoader.PerfTests;

[RPlotExporter]
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class AyendeLoaderBenchmark
{
    private const string DataFileLocation = "F:\\src\\AyendeInterview\\data100.txt";
    
    // [Benchmark]
    // public void Ayende_V1() => AyendeFileLoader.Analyze(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V2() => AyendeFileLoader.AnalyzeV2(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V2_Parallel() => AyendeFileLoader.AnalyzeV2_Parallel(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V3() => AyendeFileLoader.AnalyzeV3(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V4() => AyendeFileLoader.AnalyzeV4(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V5() => AyendeFileLoader.AnalyzeV5(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V6() => AyendeFileLoader.AnalyzeV6(DataFileLocation);
    //
    // [Benchmark]
    // public void Ayende_V7() => AyendeFileLoader.AnalyzeV7(DataFileLocation);
    //
    [Benchmark]
    public void Ayende_V8() => AyendeFileLoader.AnalyzeV8(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V9() => AyendeFileLoader.AnalyzeV9(DataFileLocation);

    [Benchmark]
    public void Ayende_V10() => AyendeFileLoader.AnalyzeV10(DataFileLocation);

    [Benchmark]
    public void Ayende_V11() => AyendeFileLoader.AnalyzeV11(DataFileLocation);

    [Benchmark]
    public void Ayende_V12() => AyendeFileLoader.AnalyzeV12(DataFileLocation);

    [Benchmark]
    public void Ayende_V13() => AyendeFileLoader.AnalyzeV13(DataFileLocation);

    [Benchmark]
    public void Ayende_V14() => AyendeFileLoader.AnalyzeV14(DataFileLocation);

    [Benchmark]
    public void Ayende_V15() => AyendeFileLoader.AnalyzeV15(DataFileLocation);
    
    [Benchmark]
    public void Ayende_V16() => AyendeFileLoader.AnalyzeV16(DataFileLocation);
}