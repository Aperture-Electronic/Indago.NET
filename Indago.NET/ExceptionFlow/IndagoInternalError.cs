namespace Indago.ExceptionFlow;

public class IndagoInternalError : IndagoException
{
    public IndagoInternalError(string? message = "") : base(message)
    {
        
    }
}