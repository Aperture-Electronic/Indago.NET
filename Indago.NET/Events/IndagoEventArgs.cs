using Com.Cadence.Indago.Scripting.Generated.Gui;

namespace Indago.Events;

public class IndagoEventArgs(ServerEvent originalEvent) : EventArgs
{
    public string Name => originalEvent.Name;
    public string Details => originalEvent.Details;
    public string Source => originalEvent.Source;
    public EventType Type => originalEvent.Type;

    protected Dictionary<string, GUIServerObjectPropertyValue> Properties { get; } =
        originalEvent.Properties.Pairs.ToDictionary<PairsEntry, string, GUIServerObjectPropertyValue>
            (property => property.Key, property => property.Value);
}