namespace BigFileLoader;

public class RecordReader : IDisposable
{
    public long Duration;
    public long Id;
    private readonly StreamReader _streamReader;

    private const int SizeOfDate = 19;// 2015-01-01T16:44:31
    private const int SizeOfSpace = 1;
    private const int SizeOfId = 8; // 00043064
    private const int SizeOfNewLine = 2; // \r\n
    private const int SizeOfRecord = SizeOfDate + SizeOfSpace + SizeOfDate + SizeOfSpace + SizeOfId + SizeOfNewLine;

    private readonly char[] _buffer = new char[SizeOfRecord];

    public RecordReader(string file)
    {
        _streamReader = new StreamReader(file);
    }

    public bool MoveNext()
    {
        int sizeRemaining = _buffer.Length;
        int index = 0;
        while (sizeRemaining > 0)
        {
            var read = _streamReader.ReadBlock(_buffer, index, sizeRemaining);
            if (read == 0)
                return false;
            index += read;
            sizeRemaining -= read;
        }

        Duration = (ParseTime(20) - ParseTime(0)).Ticks;
        Id = ParseInt(40, 8);

        return true;
    }

    private DateTime ParseTime(int pos)
    {
        var year = ParseInt(pos, 4);
        var month = ParseInt(pos + 5, 2);
        var day = ParseInt(pos + 8, 2);
        var hour = ParseInt(pos + 11, 2);
        var min = ParseInt(pos + 14, 2);
        var sec = ParseInt(pos + 17, 2);
        return new DateTime(year, month, day, hour, min, sec);
    }

    private int ParseInt(int pos, int size)
    {
        var val = 0;
        for (int i = pos; i < pos + size; i++)
        {
            val *= 10;
            val += _buffer[i] - '0';
        }
        return val;
    }

    public void Dispose()
    {
        _streamReader.Dispose();
    }
}