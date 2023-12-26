using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.Communication;
using Indago.DataTypes;
using Indago.LogFlow;
using Indago.Query.Context;
using Indago.Query.QueryContext;

namespace Indago.Query.Provider;

public class ValueProvider(IndagoImplementation impl, uint handle, uint clientId) : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
        => new ValueContext(this, expression);
    
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => (IQueryable<TElement>)new ValueContext(this, expression);

    public object? Execute(Expression expression)
        => Execute<TimeValue>(expression);
    public TimeUnit? Units { get; set; }
    
    public TimePoint? StartTime { get; set; }
    public TimePoint? EndTime { get; set; }

    public TResult Execute<TResult>(Expression expression)
    {
        // Create the query option
        BusinessLogicResponseOptions responseOptions = new();
        if (Units is { } timeUnit)
        {
            responseOptions.Units = timeUnit.ToBusinessLogic();
        }
        
        BusinessLogicQueryOptions options = new()
        {
            ResponseOpts = responseOptions
        };
        
        // Get the criteria list from the expression
        var queryContext = new ValueQueryContext(options.Criteria);
        queryContext.ElaborateExpression(expression);

        // Add start time and end time criteria if they are set
        if (StartTime is { } startTime)
        {
            options.Criteria.Add(new BusinessLogicQueryCriteria()
            {
                Type = BusinessLogicCriteriaType.StartTime,
                Operand = BusinessLogicQueryOperand.Equals,
                Value = new() { Tp = startTime }
            });
        }

        if (EndTime is { } endTime)
        {
            options.Criteria.Add(new BusinessLogicQueryCriteria()
            {
                Type = BusinessLogicCriteriaType.EndTime,
                Operand = BusinessLogicQueryOperand.Equals,
                Value = new() { Tp = endTime }
            });
        }

        // Create the query
        BusinessLogicQuery query = new()
        {
             Handle = handle,
             ClientID = clientId,
             Options = options
        };
        
        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(query, Console.WriteLine, "get_values", "query [in]");
        }
        
        // Send the query
        var response = impl.GetValues(query).Result;
        
        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(response, Console.WriteLine, "get_values", "response [out]");
        }
        
        var valueList = response.Value.Select(value => new TimeValue(value));
        return (TResult)valueList;
    }
}