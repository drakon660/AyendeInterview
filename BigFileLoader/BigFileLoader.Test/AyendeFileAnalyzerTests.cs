namespace BigFileLoader.Test;

public class AyendeFileAnalyzerTests
{
    [Fact]
    public void Test_Loading_Data()
    {   
        AyendeFileAnalyzer.AnalyzeV8("data.txt");
    }
}