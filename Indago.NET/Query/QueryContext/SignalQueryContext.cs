using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Google.Protobuf.Collections;
using Indago.Communication;
using Indago.DataTypes;
using Indago.ExceptionFlow;

namespace Indago.Query.QueryContext;

public class SignalQueryContext(RepeatedField<BusinessLogicQueryCriteria> criteriaList) : QueryContext<Signal>(criteriaList)
{
    
}