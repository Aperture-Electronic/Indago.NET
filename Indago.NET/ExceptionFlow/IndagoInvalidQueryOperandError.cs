namespace Indago.ExceptionFlow;

public class IndagoInvalidQueryOperandError(string message = "", string operand = "") : IndagoException(message)
{
    public string Operand => operand;
}