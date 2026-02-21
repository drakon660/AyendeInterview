# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Purpose

A performance optimization exercise based on a parking lot log file problem. The task: parse a large space-separated text file where each line contains `entry_time exit_time car_id` and compute total parking duration per car. Multiple progressively-optimized implementations are explored and benchmarked.

## File Format

Each record is fixed-width, 50 chars total (Windows line endings):
- `entry_time` (19 chars): `yyyy-MM-ddTHH:mm:ss`
- ` ` (1 char)
- `exit_time` (19 chars): `yyyy-MM-ddTHH:mm:ss`
- ` ` (1 char)
- `car_id` (8 chars): zero-padded integer
- `\r\n` (2 chars)

This fixed-width property is exploited in the later, faster implementations.

## Commands

```bash
# Build
dotnet build BigFileLoader.sln

# Run all tests
dotnet test BigFileLoader.Test/BigFileLoader.Test.csproj

# Run main app (measures all implementations, uses data100.txt)
dotnet run --project BigFileLoader/BigFileLoader.csproj

# Run benchmarks (must be Release build)
dotnet run -c Release --project BigFileLoader.PerfTests/BigFileLoader.PerfTests.csproj
```

## Architecture

Three projects in `BigFileLoader.sln`:

- **`BigFileLoader/`** — Core library (net10.0): all loader implementations, models, and parsing utilities
- **`BigFileLoader.Test/`** — xUnit tests (net9.0)
- **`BigFileLoader.PerfTests/`** — BenchmarkDotNet benchmarks (net10.0)

### Loader implementations

**`AyendeFileLoader`** — Ayende's implementation, V1–V9:
- V1/V2: LINQ over `File.ReadAllLines` using model classes (`RecordV1`, `RecordV2`)
- V3: `File.ReadLines` + `Dictionary` + `DateTime.Parse`
- V4: `StreamReader` + `DateTime.ParseExact` with explicit format
- V5: `StreamReader` + custom `DateMagic.ParseDate` (avoids format parsing overhead)
- V6: `StreamReader` + `DateMagic.ParseDate2` (returns raw ticks, avoids `DateTime` struct)
- V7: `Parallel.ForEach` + `ConcurrentDictionary` (benchmarks show no win)
- V8/V9: `RecordReader` — reads fixed-size 50-char blocks, parses integers directly from char array without allocations

**`MyFileLoader`** — Own implementation, V1–V5:
- Uses `AsSpan()` with fixed offsets (0, 20, 40) to slice fields without splitting
- V1: `await foreach` + `File.ReadLinesAsync`
- V2: `ReadAllLines` + index loop
- V3: `StreamReader.ReadLine` + `DateMagic.ParseDate(ref ReadOnlySpan<char>)`
- V4: `StreamReader.ReadBlock` with fixed-size buffer (same approach as `RecordReader`)
- V5: `ReadBlock` + `DateMagic.ParseTime`/`ParseInt` directly on char array

### Key utility classes

- **`RecordReader`** — The most optimized reader. Reads exactly `SizeOfRecord` (50) chars per `MoveNext()` call using `ReadBlock`, then parses `Duration` (ticks) and `Id` (long) in-place. Used by `AyendeFileLoader.AnalyzeV8` and `AnalyzeV9`.
- **`DateMagic`** — Custom date parsers for the specific `yyyy-MM-ddTHH:mm:ss` format. `ParseDate2` returns ticks directly by reimplementing `DateTime`'s internal math, avoiding struct allocation.
- **`Helper.GetFilePath`** — Resolves data files 5 directory levels above the assembly. Data files (`data.txt`, `data100.txt`, etc.) live at `F:\src\AyendeInterview\`.
- **`Constants.SizeOfRecord`** — 50 chars. Used by both `MyFileLoader.AnalyzeV4/V5` and `RecordReader`.

### Benchmark data

Benchmark classes hardcode `F:\src\AyendeInterview\data100.txt`. Test class expects `data.txt` resolved relative to the assembly location via `Helper.GetFilePath`. Generate test files with the PowerShell script described in `../README.md`.
