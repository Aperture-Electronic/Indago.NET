using Indago;
using Indago.DataTypes;
using Indago.Events;
using Indago.Server;

namespace TestIndago.NET;

[TestClass]
public class TestEventSystem
{
    [TestMethod]
    public void TestEventThread()
    {
        var args = new IndagoArgs(isLaunchNeeded: false, port: 33681);
        var clientPerf = new ClientPerferences();
        var server = new IndagoServer(args, clientPerf);
        
        // Wait for 10s and then you can monitor the event in console
        Thread.Sleep(TimeSpan.FromSeconds(10));

        Console.WriteLine("Event thread moniting is done.");
        Assert.IsTrue(true);
        
        server.Dispose();
    }

    [TestMethod]
    public void TestCDLEventMoniting()
    {
        var args = new IndagoArgs(isLaunchNeeded: false, port: 33681);
        var clientPerf = new ClientPerferences();
        var server = new IndagoServer(args, clientPerf);

        server.EventSystem.CurrentDebugLocationChanged += CDLEventHandler;
        
        // Wait for 10s and then you can monitor the event in console
        Thread.Sleep(TimeSpan.FromSeconds(10));
        
        Assert.IsTrue(true);

        server.Dispose();
    }

    private static void CDLEventHandler(object? sender, CurrentDebugLocationChangeEventArgs e)
    {
        if (sender is not IndagoServer indagoServer)
        {
            return;
        }
        
        // Get the current debug location
        var currentTime = indagoServer.CurrentTime; 
        
        Console.WriteLine($"Current debug time has been changed to {currentTime}");
    }
}