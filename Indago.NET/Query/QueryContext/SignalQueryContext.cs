using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Google.Protobuf.Collections;
using Indago.Communication;
using Indago.DataTypes;
using Indago.ExceptionFlow;

namespace Indago.Query.QueryContext;

public class SignalQueryContext(RepeatedField<BusinessLogicQueryCriteria> criteriaList)
{
    public RepeatedField<BusinessLogicQueryCriteria> CriteriaList => criteriaList;

    private void VisitExpression(Expression expression)
    {
        while (true)
        {
            
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    VisitAndAlso((BinaryExpression)expression);
                    break;
                case ExpressionType.Equal:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    VisitComparsion((BinaryExpression)expression, expression.NodeType);
                    break;
                case ExpressionType.Call:
                    VisitMethodCall((MethodCallExpression)expression);
                    break;
                case ExpressionType.Lambda:
                    expression = ((LambdaExpression)expression).Body;
                    continue;
                case ExpressionType.Quote:
                    expression = ((UnaryExpression)expression).Operand;
                    continue;
            }

            break;
        }
    }

    private void VisitAndAlso(BinaryExpression andAlso)
    {
        VisitExpression(andAlso.Left);
        VisitExpression(andAlso.Right);
    }

    private void VisitComparsion(BinaryExpression expression, ExpressionType type)
    {
        var criteria = new BusinessLogicQueryCriteria();

        if (expression.Left.NodeType != ExpressionType.MemberAccess)
        {
            throw new IndagoCriteriaError("Invalid criteria type.", expression.Left.NodeType.ToString());
        }
        
        var member = (MemberExpression)expression.Left;
        criteria.Type = Signal.GetCriteriaType(member);
        criteria.Operand = type.ToOperand();
        
        switch (expression.Right.NodeType)
        {
            case ExpressionType.Constant:
            {
                object? value = ((ConstantExpression)expression.Right).Value;
                criteria.Value = new();
                criteria.Value.SetValueByType(value);
                
                break;
            }
            case ExpressionType.MemberAccess:
                throw new IndagoCriteriaError("Can not compare two members in criteria.", member.Member.Name);
            default:
                throw new IndagoCriteriaError("Invalid criteria value type.", member.Member.Name);
        }

        CriteriaList.Add(criteria);
    }

    private void VisitMethodCall(MethodCallExpression expression)
    {
        if (expression.Method.DeclaringType == typeof(Queryable) && expression.Method.Name == "Where")
        {
            // Queryable.Where method
            VisitExpression((UnaryExpression)expression.Arguments[1]);
        }
        else if (expression.Method.DeclaringType == typeof(string) && expression.Method.Name == "Contains")
        {
            BusinessLogicQueryCriteria criteria = new();
            
            // string.Contains method
            if (expression.Object?.NodeType != ExpressionType.MemberAccess)
            {
                throw new IndagoCriteriaError("Invalid criteria type.", expression.Object?.NodeType.ToString() ?? "");
            }
            
            
            var member = (MemberExpression)expression.Object;
            criteria.Type = Signal.GetCriteriaType(member);

            var argument = expression.Arguments[0];
            switch (argument.NodeType)
            {
                case ExpressionType.Constant:
                {
                    criteria.Operand = BusinessLogicQueryOperand.Contains;
                    criteria.Value = new()
                    {
                        Str = (string)((ConstantExpression)argument).Value!
                    };

                    break;
                }
                case ExpressionType.MemberAccess:
                    throw new IndagoCriteriaError("Compare two members in criteria is not supported.", member.Member.Name);
                default:
                    throw new IndagoCriteriaError("Invalid criteria value type.", member.Member.Name);
            }
            
            CriteriaList.Add(criteria);
        }
        else
        {
            throw new IndagoInvalidQueryOperandError("Invalid criteria operand. " +
                                                     "Maybe you can try the method after convert the result to a list", expression.Method.Name);
        }
    }

    public void ElaborateExpression(Expression expression)
    {
        CriteriaList.Clear();
        VisitExpression(expression);
    }
}