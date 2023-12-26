using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.Communication;
using Indago.DataTypes;
using Indago.LogFlow;
using Indago.Query.Context;
using Indago.Query.QueryContext;

namespace Indago.Query.Provider;

public class ScopeProvider(IndagoImplementation impl, uint? handle, uint clientId, ScopeQueryType type) : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
        => new ScopeContext(this, expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => (IQueryable<TElement>)new ScopeContext(this, expression);

    public object? Execute(Expression expression)
        => Execute<Scope>(expression);
    
    public bool WithDeclaration { get; set; }
    
    public TResult Execute<TResult>(Expression expression)
    {
        // Create the query option
        BusinessLogicQueryOptions options = new()
        {
            ResponseOpts = new()
            {
                WithDeclaration = WithDeclaration
            }
        };

        // Get the criteria list from the expression
        var queryContext = new SignalQueryContext(options.Criteria);
        queryContext.ElaborateExpression(expression);

        // Create the query
        BusinessLogicQuery query = new()
        {
            ClientID = clientId,
            Options = options
        };

        // If specified, add the handle to the query
        if (handle is { } handleValue) query.Handle = handleValue;

        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(query, Console.WriteLine, "get_scopes", "query [in]");
        }

        // Send the query
        var response = type switch
        {
            ScopeQueryType.NormalChildren => impl.GetScopes(query).Result,
            _ => impl.GetParent(query).Result,
        };

        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(response, Console.WriteLine, "get_scopes", "response [out]");
        }

        var scopeList = response.Value.Select(scope => new Scope(impl, scope));
        return (TResult)scopeList;
    }
}