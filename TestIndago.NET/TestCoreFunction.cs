using Indago;
using Indago.DataTypes;
using Indago.Server;

namespace TestIndago.NET;

[TestClass]
public class TestCoreFunction
{
    private static readonly IndagoArgs args = new IndagoArgs(isLaunchNeeded: false, port: 43079);
    private static readonly ClientPerferences clientPerf = new ClientPerferences();
    private static readonly IndagoServer server = new IndagoServer(args, clientPerf);
    
    [TestMethod]
    public void TestSignalQuery()
    {
        var signals = from s in server.GetSignals(withDeclaration: true)
            where s.Depth == 3 && s.Name == "clk"
            select s;
        var signalList = signals.ToList();

        Console.WriteLine($"There is {signalList.Count} signals in the server.");
        foreach (var s in signalList)
        {
            Console.WriteLine($"{"Signal:", 20}\t{s}");
            Console.WriteLine($"{"Signal Declaration:", 20}\t{s.Declaration}");
            Console.WriteLine($"{"Source:", 20}");
            Console.WriteLine(s.Declaration?.Source.GetSourceCode());
            Console.WriteLine();
        }
    }

    [TestMethod]
    public void TestGetCurrentTime()
    {
        var time = server.CurrentTime;

        Console.WriteLine(time);
    }

    [TestMethod]
    public void TestSetCurrentTime()
    {
        var getTime = server.CurrentTime.ConvertUnitTo(TimeUnit.Nanoseconds);

        Console.WriteLine($"Current time is: {getTime}");
        
        var setTime = new TimePoint(1000, TimeUnit.Nanoseconds).ConvertUnitTo(TimeUnit.Nanoseconds);

        Console.WriteLine($"Set time to: {setTime}");
        
        server.CurrentTime = setTime;
        getTime = server.CurrentTime.ConvertUnitTo(TimeUnit.Nanoseconds);

        Console.WriteLine($"Current time is: {getTime}");
        
        Assert.AreEqual(setTime, getTime);
        
        Console.WriteLine();
    }
}