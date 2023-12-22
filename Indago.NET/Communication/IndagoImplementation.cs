using System.Collections;
using Com.Cadence.Indago.Scripting.Generated;
using Grpc.Core;
using Indago.DataTypes;
using Indago.ExceptionFlow;
using Indago.LogFlow;
using Indago.Query;
using SessionInfo = Indago.DataTypes.SessionInfo;

namespace Indago.Communication;

public class IndagoImplementation
{
    private IndagoArgs Arguments { get; }
    private IndagoConnection Connection { get; }
    private BL.BLClient BusinessLogicClient { get; }
    
    public uint ClientId { get; }
    
    public IndagoImplementation(IndagoArgs args, IndagoConnection connection)
    {
        Arguments = args;
        Connection = connection;

        if (!Connection.Alive)
        {
            throw new IndagoInternalError($"Cannot create IndagoImplementation from a dead connection.");
        }
        
        // Create the BL(business logic) client
        BusinessLogicClient = new (Connection.Channel);

        // Start the session and get the client id
        ClientId = StartSessionAsync().Result;
        
        // Get the server info
        var serverInfo = GetServerInfo().Result;
        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(serverInfo.Version, Console.WriteLine, "get_session_info", $"version");
        }

        if (Arguments.IsGui)
        {
            // TODO: Gui ready
        }
        
        // TODO: Start event thread
        
        
    }

    private async Task<uint> StartSessionAsync()
    {
        ClientInfo clientInfo = new(Arguments);
        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(clientInfo, Console.WriteLine, "start_session", $"client_info [in]");
        }
        
        // Start the session by calling gRPC method
        var clientId = await BusinessLogicClient.start_sessionAsync(clientInfo);

        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(clientId, Console.WriteLine, "start_session", $"client_id [out]");
        }

        return clientId.Value;
    }
    
    private async Task<SessionInfo> GetServerInfo()
    {
        var query = IndagoQuery.Create(ClientId);
        SessionInfo sessionInfo = await BusinessLogicClient.get_session_infoAsync(query);
        return sessionInfo;
    }

    public async Task<string> GetVersion()
    {
        var serverInfo = await GetServerInfo();
        return serverInfo.Version;
    }
    
    public async Task<IEnumerable<BusinessLogicInternal>> GetInternals(BusinessLogicQuery query)
    {
        List<BusinessLogicInternal> internals = [];
        
        try
        {
            var stream = BusinessLogicClient.get_internals(query);
            while (await stream.ResponseStream.MoveNext())
            {
                internals.AddRange(stream.ResponseStream.Current.Value);
            }

            return internals;
        }
        catch (RpcException e)
        {
            if (e.Message.Contains("Failed to deserialize response message"))
            {
                // No any response, may be the list is empty
                return internals;
            }
            else if (e.Message.Contains("Unexpected data after finished"))
            {
                throw new IndagoInternalError("The queried list is too large, please refine your query.");
            }

            throw;
        }
    }
}