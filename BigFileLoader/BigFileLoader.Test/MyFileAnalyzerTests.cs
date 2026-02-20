namespace BigFileLoader.Test;

public class MyFileAnalyzerTests
{
    [Fact]
    public void Test_Loading_Data()
    {
        MyFileLoader.AnalyzeV5("data.txt");
    }
}