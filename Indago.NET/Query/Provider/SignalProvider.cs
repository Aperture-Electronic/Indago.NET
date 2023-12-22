using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.Communication;
using Indago.DataTypes;
using Indago.LogFlow;
using Indago.Query.Context;
using Indago.Query.QueryContext;

namespace Indago.Query.Provider;

public class SignalProvider(IndagoImplementation impl, uint handle, uint clientId) : IQueryProvider
{

    public IQueryable CreateQuery(Expression expression)
        => new SignalContext(this, expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => (IQueryable<TElement>)new SignalContext(this, expression);

    public object? Execute(Expression expression)
        => Execute<Signal>(expression);
    
    public bool WithTransitions { get; set; }
    public bool WithDeclaration { get; set; }

    public TResult Execute<TResult>(Expression expression)
    {
        // Create the query option
        BusinessLogicQueryOptions options = new()
        {
            ResponseOpts = new()
            {
                WithTransitions = WithTransitions,
                WithDeclaration = WithDeclaration
            }
        };

        // Get the criteria list from the expression
        var queryContext = new SignalQueryContext(options.Criteria);
        queryContext.ElaborateExpression(expression);

        // Create the query
        BusinessLogicQuery query = new()
        {
            Handle = handle,
            ClientID = clientId,
            Options = options
        };

        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(query, Console.WriteLine, "get_signals", "query [in]");
        }

        // Send the query
        var response = impl.GetInternals(query).Result;

        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(response, Console.WriteLine, "get_signals", "response [out]");
        }

        var signalList = response.Select(signal => new Signal(signal));
        return (TResult)signalList;
    }
}