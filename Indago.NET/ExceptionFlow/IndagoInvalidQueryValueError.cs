namespace Indago.ExceptionFlow;

public class IndagoInvalidQueryValueError(string message = "", string value = "") : IndagoException(message)
{
    public string Value => value;
}