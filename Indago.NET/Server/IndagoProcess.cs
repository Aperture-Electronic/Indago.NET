using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Indago.DataTypes;
using Indago.ExceptionFlow;
using Indago.LogFlow;

namespace Indago.Server;

public class IndagoProcess : IDisposable
{
    private const int IndagoBindErrorStatus = 188;
    private IndagoArgs Arguments { get; }
    private ClientPerferences ClientPerferences { get; }
    public Process Process { get; }

    public string StandardOutput { get; private set; } = "";
    
    public string ErrorMessage { get; private set; } = "";
    
    public IndagoProcess(IndagoArgs args, ClientPerferences clientPerferences)
    {
        Arguments = args;
        ClientPerferences = clientPerferences;

        // Check if file exists before launching the process
        
        // Check if is none before in the case db is not passed
        if (!string.IsNullOrWhiteSpace(Arguments.DbPath))
        {
            if (!File.Exists(Arguments.DbPath))
            {
                throw new IndagoInternalError($"The provided database {Arguments.DbPath} was not found.");
            }
        }
        
        // Checking if is none before in the case light weight database is not passed
        if (!string.IsNullOrWhiteSpace(Arguments.LightWeightDatabase))
        {
            if (!Directory.Exists(Arguments.LightWeightDatabase))
            {
                throw new IndagoInternalError($"The provided lightweight debug database {Arguments.LightWeightDatabase} was not found.");
            }
        }

        if (Arguments.Port is null)
        {
            // Possible usage of IndagoProcess class outside of IndagoServer
            throw new ArgumentException("Port must be specified in IndagoArgs object", nameof(args));
        }

        // If the root path of Indago is not specified, use the default (in PATH)
        if (string.IsNullOrWhiteSpace(Arguments.IndagoRoot))
        {
            Arguments.IndagoRoot = "indago";
        }
        
        // Configure a new process for Indago
        Process = new()
        {
            StartInfo = new()
            {
                FileName = Arguments.IndagoRoot,
                Arguments = string.Join(' ', Arguments.GetParameterList()),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            },
            EnableRaisingEvents = true
        };
        
        // Attach event handlers
        Process.OutputDataReceived += OnIndagoProcessStandardOutput;
        Process.Exited += OnIndagoProcessExited;
        
        // Start the Indago process
        Process.Start();
    }

    private void OnIndagoProcessStandardOutput(object sender, DataReceivedEventArgs e)
    {
        string? line = e.Data;

        StandardOutput += line;

        if (string.IsNullOrWhiteSpace(line)) return;
        if (line.Contains("Error while launching Indago: "))
        {
            ErrorMessage = line.Replace("Error while launching Indago: ", "");
        }

        if (!ClientPerferences.Quiet)
        {
            IndagoLog.Log($"Indago Server Launch: {line}", Console.WriteLine);
        }

        if (!ClientPerferences.Quiet && line.Contains("Connect debugger to port "))
        {
            IndagoLog.Log("Indago Server Launch: *** NOTE: The above port is NOT the port to " +
                          "connect a .NET API client to. This port is used for internal " +
                          "debugging purposes.", Console.WriteLine);
        }
    }

    private void OnIndagoProcessExited(object? sender, EventArgs e)
    {
        var errorMessage = "Indago failed to launch properly: \n";
        if (Process.ExitCode == IndagoBindErrorStatus)
        {
            errorMessage += $".NET Scriping Service failed to bind to port {Arguments.Port}." + 
                            "Port is possibly in use by another process.";
        }

        throw new IndagoInternalError(errorMessage);
    }

    /// <summary>
    /// Try to get an usable port for Indago to use.
    /// </summary>
    public static int GetUsablePort()
    {
        // Start a new socket to request a port from the OS
        Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        socket.Listen();
        if (socket.LocalEndPoint is not IPEndPoint epLocal)
        {
            throw new IndagoInternalError($"Can not get a valid port for Indago to use.");
        }

        int port = epLocal.Port;
        socket.Close(); // Release the socket and give the port to Indago

        return port;
    }

    /// <summary>
    /// Kill the Indago process.
    /// </summary>
    public void Kill()
    {
        if (!Process.HasExited)
        {
            Process.Kill();
        }
    }
    
    public void Dispose()
    {
        Kill();
    }
}