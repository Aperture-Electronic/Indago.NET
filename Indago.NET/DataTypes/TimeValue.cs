using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.ExceptionFlow;
using Indago.Interfaces;

namespace Indago.DataTypes;

/// <summary>
/// Represents time and value pairs of a signal.
/// Each instance represents one time instant,
/// using a TimePoint object, and its respective value.
/// </summary>
public class TimeValue(BusinessLogicTimeValue blTimeValue) : IQueryCriteria
{
    public TimeValue(TimePoint timePoint, BusinessLogicString value)
        : this(new ()
        {
            Time = timePoint,
            Value = value
        })
    {
                
    }
    
    /// <summary>
    /// Get the time in which the signal value occured.
    /// </summary>
    public TimePoint Time => blTimeValue.Time;
    
    /// <summary>
    /// Get the signal value for the specific time
    /// </summary>
    public string ValueString => blTimeValue.Value.Value;

    /// <summary>
    /// Number of transition of the value (Only for query)
    /// </summary>
    public int Count { get; set; } = 0;
    
    public override string ToString()
        => $"{ValueString}@{Time}";

    public static BusinessLogicCriteriaType GetCriteriaType(MemberExpression member)
    {
        string memberName = member.Member.Name;
        return memberName switch
        {
            nameof(Count) => BusinessLogicCriteriaType.Count,
            nameof(ValueString) => BusinessLogicCriteriaType.ValueString,
            _ => throw new IndagoInvalidCriteriaTypeError($"Invalid criteria type for {nameof(Signal)}", memberName)
        };
    }
    
}