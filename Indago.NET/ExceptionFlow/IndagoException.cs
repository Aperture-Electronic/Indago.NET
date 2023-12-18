namespace Indago.ExceptionFlow;

public abstract class IndagoException : Exception
{
    protected IndagoException(string? message = "") : base(message)
    {
        
    }
}