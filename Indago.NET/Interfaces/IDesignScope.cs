using Indago.DataTypes;
using Indago.Query.Context;

namespace Indago.Interfaces;

public interface IDesignScope
{
    public IQueryable<Signal> Signals(bool withTransitions = false, bool withDeclaration = false);
    
    
    public string Name { get; }
    public string Path { get; }
}