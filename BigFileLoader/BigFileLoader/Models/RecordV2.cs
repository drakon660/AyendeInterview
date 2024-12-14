namespace BigFileLoader.Models;

public class RecordV2
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }
    public long Id { get; set; }

    public TimeSpan Duration { get; set; }

    public RecordV2(string line)
    {
        string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

        Start = DateTime.Parse(lineItems[0]);
        End = DateTime.Parse(lineItems[1]);
        Duration = this.End - this.Start;

        Id = long.Parse(lineItems[2]);
    }
}