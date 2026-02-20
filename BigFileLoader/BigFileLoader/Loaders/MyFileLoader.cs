namespace BigFileLoader;

public class MyFileLoader
{
    public static async Task<IDictionary<long, long>> Analyze_Foreach_Async(string filePath)
    {
        Dictionary<long, long> carPark = new Dictionary<long, long>();
        
        await foreach (var line in File.ReadLinesAsync(filePath))
        {
            var lineSpan = line.AsSpan();
            var starting = lineSpan.Slice(0, 19);
            var ending = lineSpan.Slice(20, 19);
            var carId = lineSpan.Slice(40, 8);
            
            var startingDate = DateTime.Parse(starting);
            var endingDate = DateTime.Parse(ending);
            
            var spent = endingDate.Ticks - startingDate.Ticks;

            var carIdLong = long.Parse(carId);

            if (!carPark.TryAdd(carIdLong, spent))
            {
                var currentSpent = carPark[carIdLong] + spent;
                carPark[carIdLong] = currentSpent;
            }
        }
        
        return carPark;
    }
    
    public static IDictionary<long, long> Analyze_While(string filePath)
    {
        Dictionary<long, long> carPark = new Dictionary<long, long>();
        
        var lines = File.ReadAllLines(filePath);
        
        for(var i = 0; i < lines.Length; i++)
        {
            var lineSpan = lines[i].AsSpan();
            var starting = lineSpan.Slice(0, 19);
            var ending = lineSpan.Slice(20, 19);
            var carId = lineSpan.Slice(40, 8);
            
            var startingDate = DateTime.Parse(starting);
            var endingDate = DateTime.Parse(ending);
            
            var spent = endingDate.Ticks - startingDate.Ticks;

            var carIdLong = long.Parse(carId);

            if (!carPark.TryAdd(carIdLong, spent))
            {
                var currentSpent = carPark[carIdLong] + spent;
                carPark[carIdLong] = currentSpent;
            }
        }
        
        return carPark;
    }
    
    public static IDictionary<long, long> AnalyzeV2(string filePath)
    {
        Dictionary<long, long> carPark = new Dictionary<long, long>();
        
        using StreamReader fileReader = new StreamReader(filePath);
        string line;
        
        while ((line = fileReader.ReadLine())!=null)
        {
            var lineSpan = line.AsSpan();
            var starting = lineSpan.Slice(0, 19);
            var ending = lineSpan.Slice(20, 19);
            var carId = lineSpan.Slice(40, 8);
            
            var startingDate = DateTime.Parse(starting);
            var endingDate = DateTime.Parse(ending);
            
            var spent = endingDate.Ticks - startingDate.Ticks;

            var carIdLong = long.Parse(carId);

            if (!carPark.TryAdd(carIdLong, spent))
            {
                var currentSpent = carPark[carIdLong] + spent;
                carPark[carIdLong] = currentSpent;
            }
        }
        
        return carPark;
    }
    
    public static IDictionary<long, long> AnalyzeV3(string filePath)
    {
        Dictionary<long, long> carPark = new Dictionary<long, long>();
        
        using StreamReader fileReader = new StreamReader(filePath);
        string line;
        while ((line = fileReader.ReadLine())!=null)
        {
            var lineSpan = line.AsSpan();
            var starting = lineSpan.Slice(0, 19);
            var ending = lineSpan.Slice(20, 19);
            var carId = lineSpan.Slice(40, 8);
            
            var startingDate = DateMagic.ParseDate(ref starting);
            var endingDate = DateMagic.ParseDate(ref ending);
            
            var spent = endingDate.Ticks - startingDate.Ticks;

            var carIdLong = long.Parse(carId);

            if (!carPark.TryAdd(carIdLong, spent))
            {
                var currentSpent = carPark[carIdLong] + spent;
                carPark[carIdLong] = currentSpent;
            }
        }
        
        return carPark;
    }
    
    public static IDictionary<long, long> AnalyzeV4(string filePath)
    {
        Dictionary<long, long> carPark = new Dictionary<long, long>();
        
        using StreamReader fileReader = new StreamReader(filePath);
        
        var buffer = new char[Constants.SizeOfRecord];
        
        while (fileReader.ReadBlock(buffer, 0, Constants.SizeOfRecord) == Constants.SizeOfRecord)
        {
            ReadOnlySpan<char> lineSpan = buffer.AsSpan();
           
            var starting = lineSpan.Slice(0, 19);
            var ending = lineSpan.Slice(20, 19);
            var carId = lineSpan.Slice(40, 8);
            
            var startingDate = DateMagic.ParseDate(ref starting);
            var endingDate = DateMagic.ParseDate(ref ending);
            
            var spent = endingDate.Ticks - startingDate.Ticks;

            var carIdLong = long.Parse(carId);

            if (!carPark.TryAdd(carIdLong, spent))
            {
                var currentSpent = carPark[carIdLong] + spent;
                carPark[carIdLong] = currentSpent;
            }
        }
        
        return carPark;
    }
    
    public static IDictionary<long, long> AnalyzeV5(string filePath)
    {
        Dictionary<long, long> carPark = new Dictionary<long, long>();
        
        using StreamReader fileReader = new StreamReader(filePath);
        
        var buffer = new char[Constants.SizeOfRecord];
        
        while (fileReader.ReadBlock(buffer)!=0)
        {
            var spent = (DateMagic.ParseTime(ref buffer, 20) - DateMagic.ParseTime(ref buffer, 0)).Ticks;
            var carIdLong= DateMagic.ParseInt(ref buffer, 40, 8);

            if (!carPark.TryAdd(carIdLong, spent))
            {
                var currentSpent = carPark[carIdLong] + spent;
                carPark[carIdLong] = currentSpent;
            }
        }
        
        return carPark;
    }
}