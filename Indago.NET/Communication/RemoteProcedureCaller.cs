using System.Collections;
using Grpc.Net.Client;
using Indago.ExceptionFlow;
using Indago.ServerUtils;
using Indago.Services;

namespace Indago.Communication;

public class RemoteProcedureCaller(IndagoImplementation impl, RemoteProcedureCallerManager manager)
{
    public IndagoConnectivity IndagoConnectivity => manager.IndagoConnectivity;
    public GrpcChannel Channel => manager.IndagoConnectivity.ConnectivityHandle.Channel;
    public bool IsClosed { get; private set; } = false;

    public void Call(string methodName, object requestMessage, RemoteProcedureCallerSettings settings)
    {
        if (IsClosed) throw new IndagoInternalError("Cannot use IndagoServer after closing it");
        if (methodName.Equals("close"))
        {
            IsClosed = true;
        }
        
        
        
        if ((settings.RequestIsStream) && (requestMessage is IEnumerable enumMessage))
        {
            
        }
        else
        {
            
        }
        
        
    }
}