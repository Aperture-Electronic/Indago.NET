using Indago.DataTypes;

namespace Indago.Interfaces;

public interface IDesignSignal
{
    public IQueryable<TimeValue> Values(TimePoint? startTime = null, TimePoint? endTime = null,
        TimeUnit units = TimeUnit.Picoseconds);
    
    public IQueryable<Signal> SubSignals(bool withTransitions = false, bool withDeclaration = false);

    public TimeValue ValueAtTime(TimePoint time);
    
    public string Name { get; }
    public uint Size { get; }
    public string FullPath { get; }
    public Declaration? Declaration { get; }
}