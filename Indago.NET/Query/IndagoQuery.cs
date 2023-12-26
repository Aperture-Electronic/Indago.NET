using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.Query;

public static class IndagoQuery
{
    /// <summary>
    /// Use the client ID to create a empty query
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns></returns>
    public static BusinessLogicQuery Create(uint clientId) => new() 
    {
        ClientID = clientId
    };

    /// <summary>
    /// Promises to assign the client ID to the query
    /// </summary>
    /// <param name="query">Query to assign</param>
    /// <param name="clientId">Client ID</param>
    /// <returns></returns>
    public static void AssignByClient(BusinessLogicQuery query, uint clientId)
    {
        query.ClientID = clientId;
    }
}