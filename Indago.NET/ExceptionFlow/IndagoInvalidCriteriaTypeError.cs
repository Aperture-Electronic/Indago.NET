namespace Indago.ExceptionFlow;

public class IndagoInvalidCriteriaTypeError(string message = "", string type = "") : IndagoException(message)
{
    public string Type => type;
}