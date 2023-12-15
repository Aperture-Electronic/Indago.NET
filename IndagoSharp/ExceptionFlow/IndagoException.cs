namespace IndagoSharp.ExceptionFlow;

public abstract class IndagoException : Exception
{
    protected IndagoException(string? message = "") : base(message)
    {
        
    }
}