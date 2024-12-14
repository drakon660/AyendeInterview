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
}