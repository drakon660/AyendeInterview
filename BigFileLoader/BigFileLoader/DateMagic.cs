using System.Runtime.CompilerServices;

namespace BigFileLoader;

public class DateMagic
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ParseDate(string value)
    {
        //format is yyyy-MM-dd'T'HH:mm:ss

        int year = (int)char.GetNumericValue(value[0]) * 1000 +
                   (int)char.GetNumericValue(value[1]) * 100 +
                   (int)char.GetNumericValue(value[2]) * 10 +
                   (int)char.GetNumericValue(value[3]);

        int month = (int)char.GetNumericValue(value[5]) * 10 +
                    (int)char.GetNumericValue(value[6]);

        int day = (int)char.GetNumericValue(value[8]) * 10 +
                  (int)char.GetNumericValue(value[9]);

        int hour = (int)char.GetNumericValue(value[11]) * 10 +
                   (int)char.GetNumericValue(value[12]);

        int minute = (int)char.GetNumericValue(value[14]) * 10 +
                     (int)char.GetNumericValue(value[15]);

        int second = (int)char.GetNumericValue(value[17]) * 10 +
                     (int)char.GetNumericValue(value[18]);

        return new DateTime(year, month, day, hour, minute, second);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ParseDate(ref ReadOnlySpan<char> value)
    {
        //format is yyyy-MM-dd'T'HH:mm:ss

        int year = (int)char.GetNumericValue(value[0]) * 1000 +
                   (int)char.GetNumericValue(value[1]) * 100 +
                   (int)char.GetNumericValue(value[2]) * 10 +
                   (int)char.GetNumericValue(value[3]);

        int month = (int)char.GetNumericValue(value[5]) * 10 +
                    (int)char.GetNumericValue(value[6]);

        int day = (int)char.GetNumericValue(value[8]) * 10 +
                  (int)char.GetNumericValue(value[9]);

        int hour = (int)char.GetNumericValue(value[11]) * 10 +
                   (int)char.GetNumericValue(value[12]);

        int minute = (int)char.GetNumericValue(value[14]) * 10 +
                     (int)char.GetNumericValue(value[15]);

        int second = (int)char.GetNumericValue(value[17]) * 10 +
                     (int)char.GetNumericValue(value[18]);

        return new DateTime(year, month, day, hour, minute, second);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ParseDate2(string value)
    {
        //format is yyyy-MM-dd'T'HH:mm:ss

        int year = (int)char.GetNumericValue(value[0]) * 1000 +
                   (int)char.GetNumericValue(value[1]) * 100 +
                   (int)char.GetNumericValue(value[2]) * 10 +
                   (int)char.GetNumericValue(value[3]);

        int month = (int)char.GetNumericValue(value[5]) * 10 +
                    (int)char.GetNumericValue(value[6]);

        int day = (int)char.GetNumericValue(value[8]) * 10 +
                  (int)char.GetNumericValue(value[9]);

        int hour = (int)char.GetNumericValue(value[11]) * 10 +
                   (int)char.GetNumericValue(value[12]);

        int minute = (int)char.GetNumericValue(value[14]) * 10 +
                     (int)char.GetNumericValue(value[15]);

        int second = (int)char.GetNumericValue(value[17]) * 10 +
                     (int)char.GetNumericValue(value[18]);

        return DateToTicks(year, month, day) + TimeToTicks(hour, minute, second);
    }

    //taken from the System.DateTime class
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long DateToTicks(int year, int month, int day)
    {
        if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
        {
            int[] days = DateTime.IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
            if (day >= 1 && day <= days[month] - days[month - 1])
            {
                int y = year - 1;
                int n = y * 365 + y / 4 - y / 100 + y / 400 + days[month - 1] + day - 1;
                return n * TicksPerDay;
            }
        }

        throw new ArgumentOutOfRangeException(null, "ArgumentOutOfRange_BadYearMonthDay");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long TimeToTicks(int hour, int minute, int second)
    {
        // totalSeconds is bounded by 2^31 * 2^12 + 2^31 * 2^8 + 2^31,
        // which is less than 2^44, meaning we won't overflow totalSeconds.
        long totalSeconds = (long)hour * 3600 + (long)minute * 60 + (long)second;
        if (totalSeconds > MaxSeconds || totalSeconds < MinSeconds)
            throw new ArgumentOutOfRangeException(null, "Overflow_TimeSpanTooLong");
        return totalSeconds * TicksPerSecond;
    }

    private static readonly int[] DaysToMonth365 =
    {
        0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365
    };

    private static readonly int[] DaysToMonth366 =
    {
        0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366
    };

    private const long TicksPerMillisecond = 10000;
    private const long TicksPerSecond = TicksPerMillisecond * 1000;
    private const long TicksPerMinute = TicksPerSecond * 60;
    private const long TicksPerHour = TicksPerMinute * 60;
    private const long TicksPerDay = TicksPerHour * 24;

    const long MaxSeconds = Int64.MaxValue / TicksPerSecond;
    const long MinSeconds = Int64.MinValue / TicksPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ParseTime(ref char[] buffer, int pos)
    {
        var year = ParseInt(ref buffer, pos, 4);
        var month = ParseInt(ref buffer, pos + 5, 2);
        var day = ParseInt(ref buffer, pos + 8, 2);
        var hour = ParseInt(ref buffer, pos + 11, 2);
        var min = ParseInt(ref buffer, pos + 14, 2);
        var sec = ParseInt(ref buffer, pos + 17, 2);
        return new DateTime(year, month, day, hour, min, sec);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt(ref char[] buffer, int pos, int size)
    {
        var val = 0;
        for (int i = pos; i < pos + size; i++)
        {
            val *= 10;
            val += buffer[i] - '0';
        }

        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ByteParseTicks(byte[] buf, int pos)
    {
        // format: yyyy-MM-ddTHH:mm:ss (all ASCII, so byte - '0' works directly)
        int year  = (buf[pos]     - '0') * 1000 + (buf[pos + 1] - '0') * 100
                  + (buf[pos + 2] - '0') * 10   +  buf[pos + 3] - '0';
        int month = (buf[pos + 5] - '0') * 10   +  buf[pos + 6] - '0';
        int day   = (buf[pos + 8] - '0') * 10   +  buf[pos + 9] - '0';
        int hour  = (buf[pos + 11] - '0') * 10  +  buf[pos + 12] - '0';
        int min   = (buf[pos + 14] - '0') * 10  +  buf[pos + 15] - '0';
        int sec   = (buf[pos + 17] - '0') * 10  +  buf[pos + 18] - '0';
        return DateToTicks(year, month, day) + TimeToTicks(hour, min, sec);
    }

    // V11: Computes (exit - entry) duration in ticks directly from raw bytes.
    //
    // Key differences from ByteParseTicks (V10):
    //   - V10 calls ByteParseTicks twice → each computes absolute ticks via
    //     DateToTicks (long = days * TicksPerDay) + TimeToTicks (long = secs * TicksPerSecond)
    //     → 4 long multiplications total, then subtracts the two results.
    //   - V11 parses both timestamps, computes daysDiff and secsDiff as int (32-bit),
    //     then only widens to long for the final 2 multiplications.
    //   - No validation branches (DateToTicks checks year/month/day ranges, TimeToTicks
    //     checks totalSeconds bounds — all unnecessary for known-good parking lot data).
    //   - Inlined leap year (DateTime.IsLeapYear has its own range check inside).
    //   - ReadOnlySpan<int> month tables instead of heap-allocated int[] arrays.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ByteParseDuration(byte[] buf, int entryPos, int exitPos)
    {
        // Parse entry timestamp digits: yyyy-MM-ddTHH:mm:ss
        // All chars are ASCII, so (byte - '0') gives the digit value 0-9 directly.
        int eYear  = (buf[entryPos]      - '0') * 1000 + (buf[entryPos + 1]  - '0') * 100
                   + (buf[entryPos + 2]  - '0') * 10   +  buf[entryPos + 3]  - '0';
        int eMonth = (buf[entryPos + 5]  - '0') * 10   +  buf[entryPos + 6]  - '0';
        int eDay   = (buf[entryPos + 8]  - '0') * 10   +  buf[entryPos + 9]  - '0';
        int eHour  = (buf[entryPos + 11] - '0') * 10   +  buf[entryPos + 12] - '0';
        int eMin   = (buf[entryPos + 14] - '0') * 10   +  buf[entryPos + 15] - '0';
        int eSec   = (buf[entryPos + 17] - '0') * 10   +  buf[entryPos + 18] - '0';

        // Parse exit timestamp digits
        int xYear  = (buf[exitPos]      - '0') * 1000 + (buf[exitPos + 1]  - '0') * 100
                   + (buf[exitPos + 2]  - '0') * 10   +  buf[exitPos + 3]  - '0';
        int xMonth = (buf[exitPos + 5]  - '0') * 10   +  buf[exitPos + 6]  - '0';
        int xDay   = (buf[exitPos + 8]  - '0') * 10   +  buf[exitPos + 9]  - '0';
        int xHour  = (buf[exitPos + 11] - '0') * 10   +  buf[exitPos + 12] - '0';
        int xMin   = (buf[exitPos + 14] - '0') * 10   +  buf[exitPos + 15] - '0';
        int xSec   = (buf[exitPos + 17] - '0') * 10   +  buf[exitPos + 18] - '0';

        // Compute diffs entirely in int (32-bit) arithmetic.
        // daysDiff: difference in calendar days (max ~3.6M, fits int easily).
        // secsDiff: difference in time-of-day seconds (range -86399 to +86399).
        // Only the final conversion to ticks requires long (64-bit) multiplication.
        int daysDiff = DaysSinceEpoch(xYear, xMonth, xDay) - DaysSinceEpoch(eYear, eMonth, eDay);
        int secsDiff = (xHour * 3600 + xMin * 60 + xSec) - (eHour * 3600 + eMin * 60 + eSec);

        return (long)daysDiff * TicksPerDay + (long)secsDiff * TicksPerSecond;
    }

    // Converts a calendar date to a day number since the epoch (day 0 = Jan 1, year 1).
    // Same formula as DateTime's internal DateToTicks, but:
    //   - Returns int days instead of long ticks (defers the TicksPerDay multiply to caller)
    //   - No validation branches (year/month/day range checks removed)
    //   - Uses IsLeapYearFast instead of DateTime.IsLeapYear
    //   - Uses ReadOnlySpan month tables instead of heap int[]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DaysSinceEpoch(int year, int month, int day)
    {
        ReadOnlySpan<int> days = IsLeapYearFast(year) ? DaysToMonth366Span : DaysToMonth365Span;
        int y = year - 1;
        // Gregorian calendar: 365 days/year + leap day corrections + cumulative month days
        return y * 365 + y / 4 - y / 100 + y / 400 + days[month - 1] + day - 1;
    }

    // Leap year check without DateTime.IsLeapYear()'s internal argument validation.
    // Divisible by 4 AND (not divisible by 100 OR divisible by 400).
    // (year & 3) == 0          → divisible by 4    (bitwise, faster than %)
    // (year % 25) != 0         → not divisible by 100  (year/4 % 25 == year % 100)
    // (year & 15) == 0         → divisible by 16, combined with &3 covers divisible by 400
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLeapYearFast(int year) =>
        (year & 3) == 0 && ((year % 25) != 0 || (year & 15) == 0);

    // ReadOnlySpan<int> properties backed by collection expressions.
    // The Roslyn compiler emits these as RuntimeHelpers.CreateSpan from static data
    // embedded directly in the assembly — no heap allocation, no GC pressure,
    // no pointer indirection through a managed int[] object.
    private static ReadOnlySpan<int> DaysToMonth365Span =>
        [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365];

    private static ReadOnlySpan<int> DaysToMonth366Span =>
        [0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366];

    // V12: Same-day fast path — managed version (no unsafe).
    // Compares the date portion (first 10 bytes: yyyy-MM-dd) of entry and exit timestamps
    // using Unsafe.ReadUnaligned for fast 8+2 byte comparison without creating Spans.
    // If dates match (typical for parking: cars enter and leave the same day),
    // we skip both DaysSinceEpoch calls and compute only the time-of-day difference —
    // 1 long multiplication instead of 2, zero calendar arithmetic.
    // Falls back to full ByteParseDuration for overnight/multi-day stays.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ByteParseDurationFast(byte[] buf, int entryPos, int exitPos)
    {
        // Compare yyyy-MM-dd (10 bytes) as long (bytes 0-7) + short (bytes 8-9).
        // Endianness doesn't matter — we're comparing identical byte positions,
        // so if the bytes are equal the longs/shorts will be equal regardless of byte order.
        if (Unsafe.ReadUnaligned<long>(ref buf[entryPos]) == Unsafe.ReadUnaligned<long>(ref buf[exitPos]) &&
            Unsafe.ReadUnaligned<short>(ref buf[entryPos + 8]) == Unsafe.ReadUnaligned<short>(ref buf[exitPos + 8]))
        {
            // Same day — only parse the time portions (HH:mm:ss at offset 11-18)
            int eHour = (buf[entryPos + 11] - '0') * 10 + buf[entryPos + 12] - '0';
            int eMin  = (buf[entryPos + 14] - '0') * 10 + buf[entryPos + 15] - '0';
            int eSec  = (buf[entryPos + 17] - '0') * 10 + buf[entryPos + 18] - '0';
            int xHour = (buf[exitPos + 11]  - '0') * 10 + buf[exitPos + 12]  - '0';
            int xMin  = (buf[exitPos + 14]  - '0') * 10 + buf[exitPos + 15]  - '0';
            int xSec  = (buf[exitPos + 17]  - '0') * 10 + buf[exitPos + 18]  - '0';

            int secsDiff = (xHour * 3600 + xMin * 60 + xSec) - (eHour * 3600 + eMin * 60 + eSec);
            return (long)secsDiff * TicksPerSecond;
        }

        // Different day — full computation with DaysSinceEpoch
        return ByteParseDuration(buf, entryPos, exitPos);
    }

    // V13: Unsafe pointer version of ByteParseDuration.
    // p points to the start of the record (entry timestamp at p+0, exit at p+20).
    // All byte accesses go through raw pointers — zero bounds checks from the runtime.
    // Same int-arithmetic-then-widen strategy as ByteParseDuration (V11).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe long UnsafeParseDuration(byte* p)
    {
        // Parse entry timestamp at p+0..18
        int eYear  = (p[0]  - '0') * 1000 + (p[1]  - '0') * 100 + (p[2]  - '0') * 10 + p[3]  - '0';
        int eMonth = (p[5]  - '0') * 10   +  p[6]  - '0';
        int eDay   = (p[8]  - '0') * 10   +  p[9]  - '0';
        int eHour  = (p[11] - '0') * 10   +  p[12] - '0';
        int eMin   = (p[14] - '0') * 10   +  p[15] - '0';
        int eSec   = (p[17] - '0') * 10   +  p[18] - '0';

        // Parse exit timestamp at p+20..38
        int xYear  = (p[20] - '0') * 1000 + (p[21] - '0') * 100 + (p[22] - '0') * 10 + p[23] - '0';
        int xMonth = (p[25] - '0') * 10   +  p[26] - '0';
        int xDay   = (p[28] - '0') * 10   +  p[29] - '0';
        int xHour  = (p[31] - '0') * 10   +  p[32] - '0';
        int xMin   = (p[34] - '0') * 10   +  p[35] - '0';
        int xSec   = (p[37] - '0') * 10   +  p[38] - '0';

        int daysDiff = DaysSinceEpoch(xYear, xMonth, xDay) - DaysSinceEpoch(eYear, eMonth, eDay);
        int secsDiff = (xHour * 3600 + xMin * 60 + xSec) - (eHour * 3600 + eMin * 60 + eSec);

        return (long)daysDiff * TicksPerDay + (long)secsDiff * TicksPerSecond;
    }

    // V14: Unsafe pointer version with same-day fast path.
    // Combines V12's date comparison trick with V13's zero-bounds-check pointer access.
    // Compares the date portion as long+short (2 comparisons for 10 bytes) via pointer cast.
    // On x86/x64 unaligned loads have no penalty on modern CPUs.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe long UnsafeParseDurationFast(byte* p)
    {
        // Compare entry date (p+0..9) vs exit date (p+20..29) as long+short.
        // 2 pointer-width comparisons instead of 10 individual byte checks.
        if (*(long*)p == *(long*)(p + 20) && *(short*)(p + 8) == *(short*)(p + 28))
        {
            // Same day — only parse time-of-day (6 digit pairs), skip all calendar math
            int eHour = (p[11] - '0') * 10 + p[12] - '0';
            int eMin  = (p[14] - '0') * 10 + p[15] - '0';
            int eSec  = (p[17] - '0') * 10 + p[18] - '0';
            int xHour = (p[31] - '0') * 10 + p[32] - '0';
            int xMin  = (p[34] - '0') * 10 + p[35] - '0';
            int xSec  = (p[37] - '0') * 10 + p[38] - '0';

            int secsDiff = (xHour * 3600 + xMin * 60 + xSec) - (eHour * 3600 + eMin * 60 + eSec);
            return (long)secsDiff * TicksPerSecond;
        }

        // Different day — fall back to full computation
        return UnsafeParseDuration(p);
    }
}