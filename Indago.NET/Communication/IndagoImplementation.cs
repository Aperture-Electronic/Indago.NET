using System.Collections;
using Com.Cadence.Indago.Scripting.Generated;
using Com.Cadence.Indago.Scripting.Generated.Gui;
using Grpc.Core;
using Indago.DataTypes;
using Indago.ExceptionFlow;
using Indago.Interfaces;
using Indago.LogFlow;
using Indago.Query;
using Indago.Server;
using SessionInfo = Indago.DataTypes.SessionInfo;

namespace Indago.Communication;

public class IndagoImplementation : IDisposable
{
    private IndagoServer Server { get; }
    private IndagoArgs Arguments { get; }
    private IndagoConnection Connection { get; }
    
    private IndagoEventThread EventThread { get; }
    private BL.BLClient BusinessLogicClient { get; }
    
    public uint ClientId { get; }

    public IIndagoEventSystem EventSystem => EventThread;

    public void Dispose()
    {
        EventThread.Stop();
        BusinessLogicClient.end_session(IndagoQuery.Create(ClientId));
        Connection.Dispose();
    }

    public IndagoImplementation(IndagoArgs args, IndagoConnection connection, IndagoServer server)
    {
        Server = server;
        Arguments = args;
        Connection = connection;

        if (!Connection.Alive)
        {
            throw new IndagoInternalError($"Cannot create IndagoImplementation from a dead connection.");
        }
        
        // Create the BL(business logic) client
        BusinessLogicClient = new (Connection.Channel);

        // Start the session and get the client id
        ClientId = StartSession().Result;
        
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

        // Start event thread
        EventThread = new(this, server);
        EventThread.Start();
    }

    private async Task<uint> StartSession()
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
        IndagoQuery.AssignByClient(query, ClientId);
        List<BusinessLogicInternal> internals = [];
        
        var stream = BusinessLogicClient.get_internals(query);
        while (await stream.ResponseStream.MoveNext())
        {
            internals.AddRange(stream.ResponseStream.Current.Value);
        }

        return internals;
    }

    public async Task<BusinessLogicScopeList> GetScopes(BusinessLogicQuery query)
    {
        IndagoQuery.AssignByClient(query, ClientId);
        BusinessLogicScopeList scopes = new();
        
        var stream = BusinessLogicClient.get_scopes(query);
        while (await stream.ResponseStream.MoveNext())
        {
            scopes.Value.AddRange(stream.ResponseStream.Current.Value);
        }

        return scopes;
    }

    public async Task<BusinessLogicScopeList> GetParent(BusinessLogicQuery query)
    {
        IndagoQuery.AssignByClient(query, ClientId);
        BusinessLogicScopeList scopeList = new();
        
        var scope = await BusinessLogicClient.get_parentAsync(query);

        if (!scope.NoScope)
        {
            scopeList.Value.Add(scope.Value);
        }

        return scopeList;
    }

    public async Task<BusinessLogicTimeValueList> GetValues(BusinessLogicQuery query)
    {
        IndagoQuery.AssignByClient(query, ClientId);
        BusinessLogicTimeValueList values = new();

        var stream = BusinessLogicClient.get_values(query);
        while (await stream.ResponseStream.MoveNext())
        {
            values.Value.AddRange(stream.ResponseStream.Current.Value);
        }

        return values;
    }

    public async Task<BusinessLogicString> GetValueAtTime(BusinessLogicQuery query)
    {
        IndagoQuery.AssignByClient(query, ClientId);
        var value = await BusinessLogicClient.get_value_at_timeAsync(query);
        return value;
    }

    public async Task<BusinessLogicTimePoint> GetCurrentTime()
    {
        var currentTime = await BusinessLogicClient.get_current_timeAsync(new NoParameters());
        return currentTime!;
    }

    public async Task SetCurrentTime(BusinessLogicTimePoint timePoint)
        => await BusinessLogicClient.set_current_timeAsync(timePoint);

    public async Task<List<ServerEvent>> GetPendingEvents()
    {
        List<ServerEvent> pendingEvents = [];
        
        var query = IndagoQuery.Create(ClientId);
        var stream = BusinessLogicClient.get_pending_events(query);

        while (await stream.ResponseStream.MoveNext())
        {
            pendingEvents.Add(stream.ResponseStream.Current);
        }

        return pendingEvents;
    }
}