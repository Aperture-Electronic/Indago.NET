using Indago.Communication;

namespace Indago.ServerUtils;

/// <summary>
/// Start Indago server, either locally/remotely, to communicate with the client.
/// </summary>
public class IndagoConnectivity(string host, int port, int timeout)
{
    public ConnectivityHandle ConnectivityHandle { get; } = new(host, port, timeout);
    
    public bool Alive => ConnectivityHandle.Alive;

    public async Task Shutdown() => await ConnectivityHandle.Shutdown();
}