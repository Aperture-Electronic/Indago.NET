using System.Collections;
using System.Linq.Expressions;
using Indago.Communication;
using Indago.DataTypes;
using Indago.Query.Provider;

namespace Indago.Query.Context;

public class SignalContext : IQueryable<Signal>
{
    private readonly SignalProvider signalProvider;
    public IQueryProvider Provider => signalProvider;
    public Type ElementType => typeof(Signal);
    public Expression Expression { get; }

    private bool withDeclaration = false;
    private bool withTransitions = false;

    public bool WithDeclaration
    {
        get => withDeclaration;
        set
        {
            withDeclaration = value;
            signalProvider.WithDeclaration = withDeclaration;
        }
    }

    public bool WithTransitions
    {
        get => withTransitions;
        set
        {
            withTransitions = value;
            signalProvider.WithTransitions = withTransitions;
        }
    }
    
    public SignalContext(IndagoImplementation impl, uint handle = 0)
    {
        signalProvider = new(impl, handle, impl.ClientId);
        Expression = Expression.Constant(this);
    }

    public IEnumerator<Signal> GetEnumerator()
        => Provider.Execute<IEnumerable<Signal>>(Expression).GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    internal SignalContext(IQueryProvider provider, Expression expression)
    {
        if (provider is not SignalProvider sigProvider)
        {
            throw new ArgumentException($"Provider must be a {nameof(SignalProvider)}", nameof(provider));
        }

        signalProvider = sigProvider;
        Expression = expression;
    }

}