using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.Communication;
using Indago.ExceptionFlow;
using Indago.Interfaces;
using Indago.Query.Context;

namespace Indago.DataTypes;

/// <summary>
/// A class represents an instantiated node in the hierarchy of the users
/// environment (initially only design scopes are supported however, future will
/// also include testbench objects).
/// </summary>
public class Scope(IndagoImplementation impl, BusinessLogicScope blScope) : IDesignScope, IQueryCriteria
{
    /// <summary>
    /// The handle of the scope for further query
    /// </summary>
    private uint handle => blScope.Handle;

    /// <summary>
    /// The name of the scope
    /// </summary>
    public string Name => blScope.Name;

    /// <summary>
    /// Path of the scope
    /// </summary>
    public string Path => blScope.Path;

    /// <summary>
    /// Depth in design of the signal
    /// </summary>
    public int Depth => string.IsNullOrWhiteSpace(Path) ? -1 : Path.Count(c => c == '.');

    /// <summary>
    /// Get the declaration information of this <see cref="Scope"/>.
    /// It includes the name, type, source location, etc.
    /// Please note the declaration information is only available
    /// when the scope is queried by set the argument
    /// withDeclaration to true
    /// </summary>
    /// <seealso cref="DataTypes.Declaration"/>
    public Declaration? Declaration { get; } =
        blScope.HasNoDeclaration ? null : new Declaration(blScope.FetchedDeclaration);
    
    private SignalContext SignalContext => new SignalContext(impl, handle);
    
    /// <summary>
    /// Get the queryable signal list from this <see cref="Scope"/>.
    /// </summary>
    /// <param name="withTransitions">The fetched signals has transitions record</param>
    /// <param name="withDeclaration">The fetched signals has declaration record</param>
    /// <returns>Queryable signal list that support LINQ query on it</returns>
    /// <seealso cref="Signal"/>
    /// <seealso cref="Declaration"/>
    public IQueryable<Signal> Signals(bool withTransitions = false, bool withDeclaration = false)
    {
        SignalContext.WithTransitions = withTransitions;
        SignalContext.WithDeclaration = withDeclaration;

        return SignalContext;
    }
    
    private ScopeContext ParentScopeContext => new (impl, handle, ScopeQueryType.Parent);
    private ScopeContext ChildrenScopeContext => new (impl, handle);
    
    /// <summary>
    /// Get the scope instance that contains this scope.
    /// </summary>
    /// <param name="withDeclaration">The fetched scope has declaration record</param>
    /// <returns>Scope object representing the parent of the current scope.</returns>
    /// <seealso cref="Scope"/>
    /// <seealso cref="Declaration"/>
    public Scope? Parent(bool withDeclaration = false)
    {
        ParentScopeContext.WithDeclaration = withDeclaration;
        var queryResult = ParentScopeContext.ToList();
        return queryResult.Count > 0 ? queryResult[0] : null;
    }
    
    /// <summary>
    /// Get the queryable list of scope objects instantiated within this scope.
    /// </summary>
    /// <param name="withDeclaration">The fetched scope has declaration record</param>
    /// <returns>Queryable list of scope children</returns>
    /// <seealso cref="Scope"/>
    /// <seealso cref="Declaration"/>
    public IQueryable<Scope> Children(bool withDeclaration = false)
    {
        ChildrenScopeContext.WithDeclaration = withDeclaration;

        return ChildrenScopeContext;
    }

    public static BusinessLogicCriteriaType GetCriteriaType(MemberExpression member)
        => member.Member.Name switch
        {
            nameof(Name) => BusinessLogicCriteriaType.Name,
            nameof(Path) => BusinessLogicCriteriaType.Path,
            nameof(Depth) => BusinessLogicCriteriaType.Depth,
            _ => throw new IndagoInvalidCriteriaTypeError($"Invalid criteria type for {nameof(Scope)}", member.Member.Name)
        };
}