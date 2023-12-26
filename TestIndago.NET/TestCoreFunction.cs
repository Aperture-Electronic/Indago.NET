using Indago;
using Indago.Analyzable;
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
        var signals = from s in server.Signals(withDeclaration: true)
            where s.Depth > 2 && s.Name == "clk"
            select s;
        var signalList = signals.ToList();

        Console.WriteLine($"There is {signalList.Count} signals in the server.");
        foreach (var s in signalList)
        {
            Console.WriteLine($"{"Signal:", 20}\t{s.FullPath}");
            Console.WriteLine($"{"Depth:", 20}\t{s.Depth}");
            Console.WriteLine($"{"Declaration:", 20}\t{s.Declaration}");
            Console.WriteLine($"{"Source:", 20}");
            Console.WriteLine(s.Declaration?.Source.GetSourceCode());
            Console.WriteLine();
        }
    }

    [TestMethod]
    public void TestGetSignalValue()
    {
        var signalReset = (from s in server.Signals()
            where s.Depth == 1 && s.Name.Contains("reset")
            select s).ToList();

        Assert.IsTrue(signalReset.Count > 0);
        
        var resetSignal = signalReset[0];

        var values = resetSignal.Values((0, "ns")).ToList();

        Assert.IsTrue(values.Count > 0);
        
        foreach (var tp in values)
        {
            Console.WriteLine(tp);
        }
    }

    [TestMethod]
    public void TestGetSignalValueAtTime()
    {
        var signalReset = (from s in server.Signals()
            where s.Depth == 1 && s.Name.Contains("reset")
            select s).ToList();

        Assert.IsTrue(signalReset.Count > 0);
        
        var resetSignal = signalReset[0];


        var valueAt100Ns = resetSignal.ValueAtTime((100, "ns"));
        
        Console.WriteLine(valueAt100Ns);
    }

    [TestMethod]
    public void TestGetSignalValueAtTimeAndAnalyze()
    {
        var bigSignals = (from s in server.Signals()
            where s.Size == 512 && s.Depth == 1
            select s).ToList();
        
        Assert.IsTrue(bigSignals.Count > 0);

        var bigSignal = bigSignals[0];

        var values = bigSignal.Values().ToList();
        
        Assert.IsTrue(values.Count > 0);
        
        // Random get a value
        var random = new Random();
        var randomValue = values[random.Next(values.Count)];
        
        // Parse to analyzable value
        AnalyzableIntegerValue analyzableValue = randomValue;
        
        Assert.IsTrue(analyzableValue.Value > 0);
        
        // Get hexadecimal value
        string hexValue = analyzableValue.HexadecimalString.TrimStart('0');
        
        Console.WriteLine($"Hexadecimal value is: {hexValue}");
        
        // Get original value
        string originalValue = randomValue.ValueString.Replace("'h", "");
        
        Assert.AreEqual(originalValue.ToUpper(), hexValue);
        
        // Split it to 8 x 64-bit blocks
        var blocks = new AnalyzableIntegerValue[8];
        for (var i = 0; i < 8; i++)
        {
            blocks[i] = analyzableValue[(i * 64, 64)];
        }
        
        // Print each block
        foreach (var block in blocks)
        {
            Console.WriteLine(block.HexadecimalString);
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

    [TestMethod]
    public void TestGetScopes()
    {
        var scopes = (from s in server.Scopes()
            where s.Depth > 0
            select s).ToList();
        
        Assert.IsTrue(scopes.Count > 0);
        
        // Display all scope name with itentation
        foreach (var scope in scopes)
        {
            Console.WriteLine($"{scope.Depth, 2} {scope.Name}");
        }
    }

    [TestMethod]
    public void TestGetHierarchy()
    {
        var topScope = server.Scopes().Where(s => s.Depth == 0).ToList().First();
        
        Stack<Scope> scopesToQuery = [];
        scopesToQuery.Push(topScope);

        while (scopesToQuery.Count > 0)
        {
            // Get the final query
            var currentScope = scopesToQuery.Pop();
            
            // Get its the children 
            var children = currentScope.Children().ToList();
            
            // Push them to the stack
            foreach (var child in children)
            {
                scopesToQuery.Push(child);
            }
            
            // Display the current scope
            Console.WriteLine($"{string.Join(' ', Enumerable.Repeat("| ", currentScope.Depth + 1))}{currentScope.Name}");
        }
    }

    [TestMethod]
    public void TestGetSignalsUnderScope()
    {
        var topScope = server.TopScope();
        
        Assert.IsNotNull(topScope);
        
        // Get all signals under the top scope
        var signals = topScope.Signals().ToList();
        
        Assert.IsTrue(signals.Count > 0);
        
        // Display all signals
        foreach (var signal in signals)
        {
            Console.WriteLine($"{signal.Name}");
        }
    }
}