using Indago;
using Indago.DataTypes;
using Indago.Server;

namespace TestIndago.NET;

[TestClass]
public class TestCoreFunction
{
    [TestMethod]
    public void TestIndagoServer()
    {
        var args = new IndagoArgs(isLaunchNeeded: false, port: 43079);
        var clientPerf = new ClientPerferences();
        var server = new IndagoServer(args, clientPerf);

        var signals = from s in server.GetSignals(withDeclaration: true)
            where s.Depth == 2 && s.Name == "clk"
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
}