﻿// See https://aka.ms/new-console-template for more information
using static BigFileLoader.Helper;
using BigFileLoader;

var dataFileLocation = GetFilePath("data100.txt");

Console.WriteLine("Ayende");
Measure(()=>AyendeFileLoader.Analyze(dataFileLocation), nameof(AyendeFileLoader.Analyze));
Measure(()=>AyendeFileLoader.AnalyzeV2(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV2));
Measure(()=>AyendeFileLoader.AnalyzeV2_Parallel(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV2_Parallel));
Measure(()=>AyendeFileLoader.AnalyzeV3(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV3));
Measure(()=>AyendeFileLoader.AnalyzeV3_Parallel(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV3_Parallel));
Measure(()=>AyendeFileLoader.AnalyzeV4(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV4));
Measure(()=>AyendeFileLoader.AnalyzeV5(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV5));
Measure(()=>AyendeFileLoader.AnalyzeV6(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV6));
Measure(()=>AyendeFileLoader.AnalyzeV7(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV7));
Measure(()=>AyendeFileLoader.AnalyzeV8(dataFileLocation), nameof(AyendeFileLoader.AnalyzeV8));
Console.WriteLine("My");
await MeasureAsync(()=>MyFileLoader.Analyze_Foreach_Async(dataFileLocation),nameof(MyFileLoader.Analyze_Foreach_Async));
Measure(()=>MyFileLoader.Analyze_While(dataFileLocation),nameof(MyFileLoader.Analyze_Foreach_Async));
Measure(()=>MyFileLoader.AnalyzeV2(dataFileLocation),nameof(MyFileLoader.AnalyzeV2));
Measure(()=>MyFileLoader.AnalyzeV3(dataFileLocation),nameof(MyFileLoader.AnalyzeV3));
Measure(()=>MyFileLoader.AnalyzeV4(dataFileLocation),nameof(MyFileLoader.AnalyzeV4));
Measure(()=>MyFileLoader.AnalyzeV5(dataFileLocation),nameof(MyFileLoader.AnalyzeV5));


