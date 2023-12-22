using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.ExceptionFlow;

namespace Indago.DataTypes;

/// <summary>
/// A class represents a value containing object within
/// an instantiated scope within the users environment.
/// For example, wires, registers, logic, etc. are
/// all represented by the signal type.
/// </summary>
public class Signal(BusinessLogicInternal blInternal)
{
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
    /// Depth in design of the signal (only for query)
    /// Please note that the default query depth is 1
    /// If you want to get all depth's signals, please set criteria <see cref="Depth"/>>0
    /// </summary>
    public int Depth { get; set; }

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
    /// Get the declaration information for this signal.
    /// It includes the name, type, source location, etc.
    /// Please note the declaration information is only available
    /// when the signal is queried by set the argument
    /// withDeclaration to true
    /// of <see cref="Server.IndagoServer.GetSignals"/>
    /// </summary>
    /// <seealso cref="DataTypes.Declaration"/>
    /// <seealso cref="Server.IndagoServer.GetSignals"/>
    public Declaration? Declaration { get; } =
        blInternal.HasNoDeclaration ? null : new Declaration(blInternal.FetchedDeclaration);
    
    public static BusinessLogicCriteriaType GetCriteriaType(MemberExpression member)
    {
        string memberName = member.Member.Name;
        return memberName switch
        {
            nameof(Name) => BusinessLogicCriteriaType.Name,
            nameof(Size) => BusinessLogicCriteriaType.Size,
            nameof(Depth) => BusinessLogicCriteriaType.Depth,
            nameof(FullPath) => BusinessLogicCriteriaType.FullPath,
            _ => throw new IndagoCriteriaError($"Invalid criteria type for {nameof(Signal)}", memberName)
        };
    }

    public override string ToString()
        => $"{FullPath}.{Name}({Size})";
}