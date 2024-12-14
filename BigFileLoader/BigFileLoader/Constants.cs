namespace BigFileLoader;

public class Constants
{
    const int SizeOfDate = 19;// 2015-01-01T16:44:31
    const int SizeOfSpace = 1;
    const int SizeOfId = 8; // 00043064
    const int SizeOfNewLine = 2; // \r\n
    public const int SizeOfRecord = SizeOfDate + SizeOfSpace + SizeOfDate + SizeOfSpace + SizeOfId + SizeOfNewLine;
    
    public static readonly char[] Splitter = [' '];
}