using Indago.DataTypes;
using Indago.ExceptionFlow;
using Indago.LogFlow;
using Indago.ServerUtils;

namespace Indago.Services;

public class RemoteProcedureCallerManager
{
    public bool ServerLaunched { get; private set; }
    public IndagoException? InitializeError { get; private set; }

    private readonly IndagoArgs indagoArgs;
    private readonly IndagoProcess indagoProcess;
    public IndagoConnectivity IndagoConnectivity { get; }
    
    public RemoteProcedureCallerManager(IndagoImplementation impl, IndagoArgs args)
    {
        ServerLaunched = false;
        InitializeError = null;

        try
        {
            indagoArgs = args;

            if (IndagoLog.IndagoScriptingClientDebug)
            {
                IndagoLog.Log(indagoArgs, Console.WriteLine, "init", $"{nameof(indagoArgs)} [args]");
            }
            
            // Launching Indago process
            if (indagoArgs.IsLaunchNeeded)
            {
                if (!indagoArgs.Host.Equals("localhost") &&
                    !indagoArgs.Host.Equals(Environment.GetEnvironmentVariable("HOSTNAME")))
                {
                    throw new IndagoInternalError("Cannot launch Indago server on a remote host");
                }
                else if (string.IsNullOrWhiteSpace(indagoArgs.IndagoRoot))
                {
                    throw new IndagoInternalError("Cannot launch Indago server without neither " +
                                                  "INDAGO_ROOT environment variable nor " +
                                                  "indago_args.indago_root set");
                } 
                
                // Port not specified, tries to open server in a random open port a couple of times
                if (indagoArgs.Port is { } port)
                {
                    if (ServerConnect(indagoArgs.Host, port, 500))
                    {
                        throw new IndagoInternalError($"Port {port} already in use by another Indago server");
                    }

                    indagoProcess = new(indagoArgs);
                    ServerLaunched = true;
                    if (indagoProcess.InitializeError is not null)
                    {
                        throw indagoProcess.InitializeError;
                    }
                }
                else
                {
                    for (var retry = 3; retry > 0; retry--)
                    {
                        indagoArgs.Port = IndagoProcess.GetOpenPort();
                        if (indagoArgs.Port is not { } portReopen)
                        {
                            continue;
                        }

                        if (ServerConnect(indagoArgs.Host, portReopen, 500))
                        {
                            continue;
                        }

                        try
                        {
                            indagoProcess = new(indagoArgs);
                            ServerLaunched = true;
                            if (indagoProcess.InitializeError is not null)
                            {
                                throw indagoProcess.InitializeError;
                            }
                        }
                        catch (IndagoInternalError e)
                        {
                            if (e.Message.Contains("failed to bind to port"))
                            {
                                continue;
                            }

                            throw;
                        }

                        // Success, can quit the loop
                        break;
                    }

                    throw new IndagoInternalError($"Failed to bind to launched process in random port for 3 times");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(indagoArgs.Host) || indagoArgs.Port is null)
                {
                    throw new IndagoInternalError("Cannot connect to Indago server without specifying host and port");
                }
                
                
            }

            IndagoConnectivity = new(indagoArgs.Host, indagoArgs.Port ?? -1, 500);
            if (!IndagoConnectivity.Alive)
            {
                throw new IndagoTimeoutError(
                    $"Could not connect to Indago server on host: {indagoArgs.Host}:{indagoArgs.Port}");
            }
            
            
        }
        catch (IndagoException e)
        {
            InitializeError = e;
        }
    }

    private bool ServerConnect(string host, int port, int timeout)
    {
        try
        {
            var connectivity = new IndagoConnectivity(host, port, timeout);
            return connectivity.Alive;
        }
        catch (IndagoTimeoutError)
        {
            return false;
        }
    }
}