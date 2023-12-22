namespace Indago.ExceptionFlow;

public class IndagoCriteriaError(string message = "", string criteriaMember = "") : IndagoException(message)
{
    public string CriteriaMember => criteriaMember;
}