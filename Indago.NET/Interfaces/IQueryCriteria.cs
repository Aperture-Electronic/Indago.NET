using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.Interfaces;

public interface IQueryCriteria
{
    public static abstract BusinessLogicCriteriaType GetCriteriaType(MemberExpression member);
}