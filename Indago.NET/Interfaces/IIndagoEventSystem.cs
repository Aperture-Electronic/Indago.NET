using Indago.Events;

namespace Indago.Interfaces;

public interface IIndagoEventSystem
{
    /// <summary>
    /// Event triggered on current debug location (CDL) time change.
    /// Moving the waveform CDL marker, changing the debug time,
    /// and initiating operations, such as driver tracing, that
    /// change the debug time are examples of operations included
    /// in this event.
    /// </summary>
    public event CurrentDebugLocationChangedEventHandler? CurrentDebugLocationChanged;
}