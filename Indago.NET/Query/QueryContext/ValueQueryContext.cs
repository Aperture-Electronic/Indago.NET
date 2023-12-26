using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Google.Protobuf.Collections;
using Indago.DataTypes;
using Indago.ExceptionFlow;

namespace Indago.Query.QueryContext;

public class ValueQueryContext(RepeatedField<BusinessLogicQueryCriteria> criteriaList) : QueryContext<TimeValue>(criteriaList)
{
   
}