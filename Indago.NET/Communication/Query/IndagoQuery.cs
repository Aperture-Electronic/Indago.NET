using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.Query;

public static class IndagoQuery
{

    private static BusinessLogicResponseOptions CreateResponseOptions(BusinessLogicCriteriaType type)
    {
        BusinessLogicResponseOptions options = new();

        // TODO: Response options
        
        
        
        return options;
    }
    
    public static BusinessLogicQuery Create(uint clientId) => new() 
    {
        ClientID = clientId
    };
    
    public static BusinessLogicQuery Create(uint? handle = null, uint? clientId = null)
    {
        BusinessLogicQuery query = new();
        BusinessLogicQueryOptions options = new();

        if (handle is { } handleNotNull) query.Handle = handleNotNull;
        if (clientId is { } clientIdNotNull) query.ClientID = clientIdNotNull;        
    
        
        return query;
    }
}