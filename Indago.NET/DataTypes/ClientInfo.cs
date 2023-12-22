using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

public class ClientInfo (string host, int port, string clientPath = "", bool embedded = false)
{
    public string Host => host;
    public int Port => port;
    public string ClientPath => clientPath;
    public bool Embedded => embedded;

    public ClientInfo(IndagoArgs args) : this(args.Host, args.Port ?? -1, embedded: args.Embedded)
    {
        
    }
    
    // Implicit convert between GRPC ClientInfo and Indago ClientInfo
    public static implicit operator ClientInfo(BusinessLogicClientInfo clientInfo) 
        => new(clientInfo.Host, int.Parse(clientInfo.Port), clientInfo.ClientPath, clientInfo.Embedded);

    public static implicit operator BusinessLogicClientInfo(ClientInfo clientInfo)
        => new()
        {
            Host = clientInfo.Host,
            Port = clientInfo.Port.ToString(),
            ClientPath = clientInfo.ClientPath,
            Embedded = clientInfo.Embedded
        };
}