using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.ExceptionFlow;

namespace Indago.Query;

public static class CritriaExtensions
{
    public static BusinessLogicQueryOperand ToOperand(this ExpressionType type)
        => type switch
        {
            ExpressionType.Equal => BusinessLogicQueryOperand.Equals,
            ExpressionType.GreaterThan => BusinessLogicQueryOperand.GreaterThan,
            ExpressionType.GreaterThanOrEqual => BusinessLogicQueryOperand.GreaterThanEquals,
            ExpressionType.LessThan => BusinessLogicQueryOperand.LessThan,
            ExpressionType.LessThanOrEqual => BusinessLogicQueryOperand.LessThanEquals,
            ExpressionType.NotEqual => BusinessLogicQueryOperand.NotEquals,
            _ => throw new IndagoInvalidCriteriaTypeError($"The expression type {type} is not implemented", type.ToString())
        };

    public static void SetValueByType(this BusinessLogicQueryValue query, object? value)
    {
        switch (value)
        {
            case null:
                throw new IndagoInvalidQueryValueError("The value cannot be null");
            case bool boolValue:
                query.Boolean = boolValue;
                break;
            case ulong ulongValue:
                query.Int = ulongValue;
                break;
            case uint uintValue:
                query.Int = uintValue;
                break;
            case int intValue:
                query.Int = (ulong)intValue;
                break;
            case string strValue:
                query.Str = strValue;
                break;
            // TODO
            default:
                break;
        }
    }
}