using Com.Cadence.Indago.Scripting.Generated.Gui;
using Indago.Events;
using Indago.Interfaces;
using Indago.Server;

namespace Indago.Communication;

public class IndagoEventThread(IndagoImplementation impl, IndagoServer server) : IIndagoEventSystem
{
    private CancellationTokenSource CancellationTokenSource { get; } = new();

    private Thread? thread = null;

    #region Events
    public event CurrentDebugLocationChangedEventHandler? CurrentDebugLocationChanged;
    
    #endregion
    
    /// <summary>
    /// Start the event thread
    /// </summary>
    public void Start()
    {
        Stop();
        
        // Create a new thread
        thread = new(ThreadRun);
        
        // Start the thread
        thread.Start(CancellationTokenSource.Token);
    }

    /// <summary>
    /// Stop the event thread
    /// </summary>
    public void Stop()
    {
        if (thread?.IsAlive != true) return;
        
        // If the thread is still running, stop it
        CancellationTokenSource.Cancel();

        // Wait for the thread stop
        while (thread.ThreadState != ThreadState.Stopped)
        {
            Thread.Sleep(1);
        }

        // Reset the cancellation token source
        CancellationTokenSource.TryReset();
    }

    private void ThreadRun(object? args)
    {
        if (args is not CancellationToken cancelToken)
        {
            throw new ArgumentException($"The argument must be a {nameof(CancellationToken)}", nameof(args));
        }
        
        // Infinite loop for monitoring the event from Indago
        for (;;)
        {
            // Check if the thread is cancelled
            if (cancelToken.IsCancellationRequested)
            {
                break;
            }
            
            // Get pending events from Indago Server
            var pendingEvents = impl.GetPendingEvents().Result;
            
            // Check each event and handle it (if user registered a handler)
            foreach (var pendingEvent in pendingEvents)
            {
                // Debug
                // Console.WriteLine($"Detected event: {pendingEvent.Name}, type: {pendingEvent.Type} @ {DateTime.Now:HH:mm:ss}");
                
                ProcessEvent(pendingEvent);
            }
            
            Thread.Sleep(50);
        }
    }
    
    private void ProcessEvent(ServerEvent pendingEvent)
    {
        switch (pendingEvent.Type)
        {
            case EventType.CdlChange:
                CurrentDebugLocationChangeEventArgs cdlChangeEventArgs = new(pendingEvent);
                CurrentDebugLocationChanged?.Invoke(server, cdlChangeEventArgs);
                break;
            default:
                break;
            // Unsupported event
        }
    }
}