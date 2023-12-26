using Com.Cadence.Indago.Scripting.Generated.Gui;

namespace Indago.Events;

public class CurrentDebugLocationChangeEventArgs : IndagoEventArgs
{
    /// <summary>
    /// True when the changing was done as a part of a driver trace flow
    /// </summary>
    public bool ByDriverTracing { get; }

    /// <summary>
    /// True when there was a running driver tracing process during this changing
    /// </summary>
    public bool DuringDriverTracing { get; }
    
    public CurrentDebugLocationChangeEventArgs(ServerEvent serverEvent) : base(serverEvent)
    {
        if (Properties.TryGetValue("by_driver_tracing", out var objValue))
        {
            ByDriverTracing = objValue.Boolean.HasValue && objValue.Boolean.Value;
        }
        else
        {
            ByDriverTracing = false;
        }
        
        if (Properties.TryGetValue("during_driver_tracing", out objValue))
        {
            DuringDriverTracing = objValue.Boolean.HasValue && objValue.Boolean.Value;
        }
        else
        {
            DuringDriverTracing = false;
        }
    }
}