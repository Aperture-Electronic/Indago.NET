using System.Diagnostics;
using System.Text;
using IndagoSharp.DataTypes;
using IndagoSharp.ExceptionFlow;

namespace IndagoSharp.ServerUtils;

/// <summary>
/// Start Indago server, either locally/remotely, to communicate with the client.
/// </summary>
public class IndagoProcess
{
    private readonly IndagoInternalError processNotStartedException = new
        IndagoInternalError("Indago process has not been started yet");
    
    private readonly IndagoArgs processArgs;
    private readonly Process? process;
    private IndagoWatcher? watcher;

    public IndagoInternalError? InitializeError { get; private set; }
    public int? ProcessId => process?.Id ?? null;
    public bool Alive => !process?.HasExited ?? false;
    public StreamReader StandardOutput => process?.StandardOutput ?? throw processNotStartedException;
    public StreamReader StandardError => process?.StandardError ?? throw processNotStartedException;
    public DateTime LaunchTime => process?.StartTime ?? throw processNotStartedException;
    public DateTime ExitTime => process?.ExitTime ?? throw processNotStartedException;
    
    public IndagoProcess(IndagoArgs args)
    {
        InitializeError = null;
        
        try
        {
            if (!string.IsNullOrWhiteSpace(args.DbPath))
            {
                if (!Path.Exists(args.DbPath))
                {
                    throw new IndagoInternalError($"The provided database {args.DbPath} was not found");
                }
            }

            if (!string.IsNullOrWhiteSpace(args.Lwd))
            {
                if (!Path.Exists(args.Lwd))
                {
                    throw new IndagoInternalError($"The provided lightweight debug database {args.Lwd} was not found");
                }
            }

            processArgs = args;

            if (processArgs.Port is null)
            {
                // Possible usage of IndagoProcess class outside of IndagoServer
                throw new ArgumentException("Port must be specified in IndagoArgs object");
            }

            process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = args.IndagoRoot,
                    Arguments = string.Join(" ", processArgs.GetParameterList()),
                    StandardInputEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
        }
        catch (IndagoInternalError e)
        {
            InitializeError = e;
        }
    }

    public int GetOpenPort()
    {
        return 0;
    }

    public void Kill()
    {
        if (ProcessId is null) return;
        try
        {
            process?.Kill();
        }
        catch (InvalidOperationException e)
        {
            return;
        }
    }

    public void ShutdownWatcher()
    {
        if (watcher is null) return;
        watcher.Shutdown();
        watcher = null;
    }

    public void Shutdown()
    {
        ShutdownWatcher();
        
        while (DateTime.Now - ExitTime < TimeSpan.FromSeconds(3))
        {
            // Process ended, no action needed
            if (!Alive) return;
            
            Thread.Sleep(100);
        }
        
        if (Alive) Kill();
    }
}