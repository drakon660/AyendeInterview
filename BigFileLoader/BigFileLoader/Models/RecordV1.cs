namespace BigFileLoader.Models;

public class RecordV1
{
    public DateTime Start => DateTime.Parse(_line.Split(' ')[0]);

    public DateTime End => DateTime.Parse(_line.Split(' ')[1]);
    public long Id => long.Parse(_line.Split(' ')[2]);

    public TimeSpan Duration => End - Start;

    private readonly string _line;

    public RecordV1(string line)
    {
        _line = line;
    }
}