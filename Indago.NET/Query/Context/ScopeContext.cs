using System.Collections;
using System.Linq.Expressions;
using Indago.Communication;
using Indago.DataTypes;
using Indago.Query.Provider;

namespace Indago.Query.Context;

public class ScopeContext : IQueryable<Scope>
{
    private readonly ScopeProvider scopeProvider;
    public IQueryProvider Provider => scopeProvider;
    public Type ElementType => typeof(Signal);
    public Expression Expression { get; }
    
    private bool withDeclaration = false;
    
    public bool WithDeclaration
    {
        get => withDeclaration;
        set
        {
            withDeclaration = value;
            scopeProvider.WithDeclaration = withDeclaration;
        }
    }
    
    public ScopeContext(IndagoImplementation impl, uint? handle = 0, ScopeQueryType queryType = ScopeQueryType.NormalChildren)
    {
        scopeProvider = new(impl, handle, impl.ClientId, queryType);
        Expression = Expression.Constant(this);
    }

    public IEnumerator<Scope> GetEnumerator()
        => Provider.Execute<IEnumerable<Scope>>(Expression).GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    internal ScopeContext(IQueryProvider provider, Expression expression)
    {
        if (provider is not ScopeProvider scpProvider)
        {
            throw new ArgumentException($"Provider must be a {nameof(ScopeProvider)}", nameof(provider));
        }

        scopeProvider = scpProvider;
        Expression = expression;
    }
}