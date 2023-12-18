using System.Diagnostics;

namespace Indago.ServerUtils;

/// <summary>
/// Watcher thread that monitors an Indago child Process for premature exits.
/// </summary>
public class IndagoWatcher(Process indagoProcess, Process clientProcess, string? logsPath = null, bool quiet = false) : Process
{
    private bool stop = false;
    
    /// <summary>
    /// Check log files for specific error strings and print them
    /// try to return a cause if we can figure it out
    /// </summary>
    private string GrepLogs()
    {
        var message = "Indago Server Terminated Prematurely\n";

        if ((logsPath is not null) || (!Path.Exists(logsPath)))
        {
            return message;
        }
        
        string[] errStrings = ["exception", "error", "problematic"];
        foreach (string file in Directory.GetFiles(logsPath, "*.log"))
        {
            foreach (string line in File.ReadAllLines(file))
            {
                if (!errStrings.Any(err => line.Contains(err))) continue;
                if (!line.Contains("ssh") && !quiet) message += $"{file}: {line}\n";
            }
        }
        
        return message;
    }

    /// <summary>
    /// Watcher run. wake up periodically and check Indago process.
    /// If it died report any error conditions found.
    /// </summary>
    public void Run()
    {
        while (!stop)
        {
            if (indagoProcess.HasExited)
            {
                int exitCode = indagoProcess.ExitCode;
                stop = true;
                
                var message = $"Indago Server Terminated Prematurely with exit code {exitCode} (no logs generated)\n";

                if (logsPath is not null)
                {
                    if (Path.Exists(Path.Join(logsPath, "out_of_memory_error")))
                    {
                        message += "Indago Server Out of Memory\n";
                    }
                }
                else if (Path.Exists(logsPath) && !quiet)
                {
                    message = GrepLogs();
                }
                
                if (!quiet) Console.WriteLine(message);
                {
                    Console.WriteLine($"\n\nERROR: Indago Scripting Server FATAL - {message}\n\n");
                    
                    clientProcess.Kill();
                }
            }
            
            Thread.Sleep(1000);
        }
    }

    public void Shutdown()
    {
        stop = true;
    }
}