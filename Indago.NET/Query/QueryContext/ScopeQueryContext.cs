using System.Linq.Expressions;
using Com.Cadence.Indago.Scripting.Generated;
using Google.Protobuf.Collections;
using Indago.DataTypes;

namespace Indago.Query.QueryContext;

public class ScopeQueryContext(RepeatedField<BusinessLogicQueryCriteria> criteriaList) : QueryContext<Scope>(criteriaList)
{

}