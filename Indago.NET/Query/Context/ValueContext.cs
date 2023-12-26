using System.Collections;
using System.Linq.Expressions;
using Indago.Communication;
using Indago.DataTypes;
using Indago.Query.Provider;

namespace Indago.Query.Context;

public class ValueContext : IQueryable<TimeValue>
{
    private readonly ValueProvider valueProvider;
    
    public IQueryProvider Provider => valueProvider;
    
    public Type ElementType => typeof(TimeValue);
    public Expression Expression { get; }
    
    private TimeUnit? units = null;

    /// <summary>
    /// The unit of time of responsed time-value pairs.
    /// </summary>
    public TimeUnit? Units
    {
        get => units;
        set
        {
            units = value;
            valueProvider.Units = units;
        }
    }

    private TimePoint? startTime = null;
    private TimePoint? endTime = null;

    /// <summary>
    /// The start time of query
    /// </summary>
    public TimePoint? StartTime
    {
        get => startTime;
        set
        {
            startTime = value;
            valueProvider.StartTime = startTime;
        
        }
    }

    /// <summary>
    /// The end time of query
    /// </summary>
    public TimePoint? EndTime
    {
        get => endTime;
        set
        {
            endTime = value;
            valueProvider.EndTime = endTime;
        }
    }
    
    public ValueContext(IndagoImplementation impl, uint handle = 0)
    {
        valueProvider = new(impl, handle, impl.ClientId);
        Expression = Expression.Constant(this);
    }
    
    public IEnumerator<TimeValue> GetEnumerator()
        => Provider.Execute<IEnumerable<TimeValue>>(Expression).GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    internal ValueContext(IQueryProvider provider, Expression expression)
    {
        if (provider is not ValueProvider valProvider)
        {
            throw new ArgumentException($"Provider must be a {nameof(ValueProvider)}", nameof(provider));
        }

        valueProvider = valProvider;
        Expression = expression;
    }
}