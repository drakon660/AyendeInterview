// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using BigFileLoader.PerfTests;

BenchmarkSwitcher.FromTypes([typeof(AyendeLoaderBenchmark), typeof(MyLoaderBenchmark)]).Run(args);