using System.Threading.Channels;
using Grpc.Core;
using Grpc.Net.Client;
using Indago.ExceptionFlow;

namespace Indago.Communication;

public class ConnectivityHandle
{
    public GrpcChannel Channel { get; }
    private int Timeout { get; }
    private string HostAddress { get; }
    
    public ConnectivityHandle(string host, int port, int timeout)
    {
        Timeout = timeout;
        
        // Make the address string
        var channelAddress = $"{host}:{port}";
        HostAddress = channelAddress;

        // Create the gRPC channel instance by insecure
        Channel = GrpcChannel.ForAddress(channelAddress, new()
        {
            Credentials = ChannelCredentials.Insecure,
            HttpClient = new(new SocketsHttpHandler
            {
                ConnectTimeout = TimeSpan.FromMilliseconds(timeout)
            })
        });

        Channel.ConnectAsync();
        WaitForReady();
    }

    public bool Alive => Channel.State == ConnectivityState.Ready;

    public void WaitForReady()
    {
        bool inTime = Task.WaitAll([Channel.WaitForStateChangedAsync(ConnectivityState.Ready)],
            TimeSpan.FromMilliseconds(Timeout));
        if (!inTime)
        {
            throw new IndagoTimeoutError($"Could not connect to Indago server on host {HostAddress}");
        }
    }

    public async Task Shutdown() => await Channel.ShutdownAsync();
}