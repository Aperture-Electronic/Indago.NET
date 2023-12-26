using Grpc.Core;
using Grpc.Net.Client;
using Indago.DataTypes;
using Indago.ExceptionFlow;

namespace Indago.Communication;

public class IndagoConnection : IDisposable
{
   private IndagoArgs Arguments { get; }
   private int Timeout { get; }
   
   public GrpcChannel Channel { get; }

   public bool Alive => Channel.State == ConnectivityState.Ready;
   
   public IndagoConnection(IndagoArgs args, int timeout = 500)
   {
      Arguments = args;
      Timeout = timeout;

      var grpcUrl = $"http://{Arguments.Host}:{Arguments.Port}";

      AppContext.SetSwitch(
         "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
      
      // Create a channel with the server
      Channel = GrpcChannel.ForAddress(grpcUrl, new ()
      {
         Credentials = ChannelCredentials.Insecure,
         MaxSendMessageSize = null,
         MaxReceiveMessageSize = null
      });
      
      bool connectInTime = Task.WaitAll([Channel.ConnectAsync()], timeout);

      if (!connectInTime)
      {
         throw new IndagoTimeoutError($"Connect to {grpcUrl} timed out after {timeout}ms");
      }
   }

   public void Dispose() => Channel.Dispose();
}