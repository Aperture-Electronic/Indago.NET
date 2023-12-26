using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Google.Protobuf.Collections;
using Indago.Communication;
using Indago.ExceptionFlow;
using Indago.Interfaces;
using Indago.Query.Context;
using Indago.Server;

namespace Indago.DataTypes;

/// <summary>
/// A class represents a value containing object within
/// an instantiated scope within the users environment.
/// For example, wires, registers, logic, etc. are
/// all represented by the signal type.
/// </summary>
public class Signal(IndagoImplementation impl, BusinessLogicInternal blInternal) : IDesignSignal, IQueryCriteria
{
    /// <summary>
    /// The handle of the signal for further query
    /// </summary>
    private uint handle => blInternal.Handle;
    
    /// <summary>
    /// The name of the signal
    /// </summary>
    public string Name => blInternal.Name;

    /// <summary>
    /// <para>If this signal is a bus, get the number of bits associated with
    /// this signal.</para>
    /// <para>If this signal is a memory, get the number of bits associated with
    /// the entire memory.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// var signalBitWidth = signal.Size;
    /// </code>
    /// </example>
    public uint Size => blInternal.Size;

    /// <summary>
    /// Get the signal type.
    /// </summary>
    /// <seealso cref="DeclarationType"/>
    /// <example>
    /// <code>
    /// if (signal.Type == DeclarationType.Wire)
    ///     Console.WriteLine("This signal is a wire");
    /// </code>
    /// </example>
    [Obsolete($"This property is deprecated, please use {nameof(Declaration)}.{nameof(Declaration.Type)} instead")]
    public DeclarationType Type => blInternal.Type.ToDeclarationType();

    /// <summary>
    /// Depth in design of the signal
    /// </summary>
    public int Depth => FullPath.Count(c => c == '.');

    /// <summary>
    /// Full path of the signal
    /// </summary>
    public string FullPath => blInternal.Path;

    /// <summary>
    /// Language of the signal
    /// </summary>
    [Obsolete($"This property is deprecated, please use {nameof(Declaration)}.{nameof(Declaration.Language)} instead")]
    public Language Language => blInternal.Language.ToLanguage();

    /// <summary>
    /// Get the declaration information for this <see cref="Signal"/>.
    /// It includes the name, type, source location, etc.
    /// Please note the declaration information is only available
    /// when the signal is queried by set the argument
    /// withDeclaration to true
    /// </summary>
    /// <seealso cref="DataTypes.Declaration"/>
    public Declaration? Declaration { get; } =
        blInternal.HasNoDeclaration ? null : new Declaration(blInternal.FetchedDeclaration);

    private ValueContext ValueContext { get; } = new(impl, blInternal.Handle);
    
    /// <summary>
    /// Get the querable value list of the <see cref="Signal"/>.
    /// </summary>
    /// <param name="startTime">Set the minimal time of the returned time-value pairs, null means no-set</param>
    /// <param name="endTime">Set the maximal time of the returned time-value pairs, null means no-set</param>
    /// <param name="units">Time unit of the returned time-value pairs</param>
    /// <returns>A queryable list of the signal list contains time-value pairs</returns>
    public IQueryable<TimeValue> Values(TimePoint? startTime = null, TimePoint? endTime = null, TimeUnit units = TimeUnit.Picoseconds)
    {
        ValueContext.StartTime = null;
        ValueContext.EndTime = null;
        ValueContext.Units = units;
        return ValueContext;
    }

    /// <summary>
    /// Get the value of the signal at the given time
    /// </summary>
    /// <param name="time">Specific time to get time-value pair</param>
    /// <returns>Value queried at the specific time point for a signal</returns>
    public TimeValue ValueAtTime(TimePoint time)
    {
        var queryOptions = new BusinessLogicQueryOptions();
        
        // Prepare the criteria
        queryOptions.Criteria.Add(new BusinessLogicQueryCriteria()
        {
            Type = BusinessLogicCriteriaType.StartTime,
            Operand = BusinessLogicQueryOperand.Equals,
            Value = new() { Tp = time }
        });

        // Get the value
        var value = impl.GetValueAtTime(new()
        {
            Handle = blInternal.Handle,
            ClientID = impl.ClientId,
            Options = queryOptions
        }).Result;

        return new(time, value);
    }
    
    public static BusinessLogicCriteriaType GetCriteriaType(MemberExpression member)
    {
        string memberName = member.Member.Name;
        return memberName switch
        {
            nameof(Name) => BusinessLogicCriteriaType.Name,
            nameof(Size) => BusinessLogicCriteriaType.Size,
            nameof(Depth) => BusinessLogicCriteriaType.Depth,
            nameof(FullPath) => BusinessLogicCriteriaType.FullPath,
            _ => throw new IndagoInvalidCriteriaTypeError($"Invalid criteria type for {nameof(Signal)}", memberName)
        };
    }

    public override string ToString()
        => $"{FullPath}.{Name}({Size})";
}