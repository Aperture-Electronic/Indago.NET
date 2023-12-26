using Indago.Communication;
using Indago.DataTypes;
using Indago.ExceptionFlow;
using Indago.Interfaces;
using Indago.LogFlow;
using Indago.Query.Context;

namespace Indago.Server;

public class IndagoServer : IDesignScope, IDisposable
{
    private IndagoArgs Arguments { get; }
    private ClientPerferences ClientPerferences { get; }
    private IndagoProcess? Process { get; } = null;
    private IndagoConnection? Connection { get; } = null;
    
    private IndagoImplementation? Implementation { get; } = null;
    
    public IndagoServer(IndagoArgs args, ClientPerferences clientPerferences)
    {
        Arguments = args;
        ClientPerferences = clientPerferences;

        if (IndagoLog.IndagoScriptingClientDebug)
        {
            IndagoLog.Log(args, Console.WriteLine, "init", $"{nameof(args)} [args]");
        }

        if (Arguments.IsLaunchNeeded)
        {
            // If need launch the Indago (only valid on localhost)
            if (!Arguments.Host.Equals("localhost") &&
                !Arguments.Host.Equals(Environment.GetEnvironmentVariable("HOSTNAME")))
            {
                throw new IndagoInternalError("Cannot launch Indago on a remote host.");
            }
            else if (string.IsNullOrWhiteSpace(Arguments.IndagoRoot))
            {
                throw new IndagoInternalError("Cannot launch Indago server without neither " +
                "INDAGO_ROOT environment variable nor " +
                "indago_args.indago_root set");
            }
            
            // Port not specified, tries to open server in a random open port a couple of times
            if (Arguments.Port is null)
            {
                var retry = 0;
                for (retry = 0; retry < clientPerferences.RandomPortRetry; retry++)
                {
                    Arguments.Port = IndagoProcess.GetUsablePort();

                    try
                    {
                        // Start Indago server by launching a new process
                        Process = new(Arguments, ClientPerferences);
                    }
                    catch (IndagoInternalError e)
                    {
                        // Retry if only the port is invalid
                        if (e.Message.Contains("failed to bind to port")) continue;
                        throw;
                    }

                    try
                    {
                        Connection = new(Arguments, ClientPerferences.ServerTimeout);
                    }
                    catch (IndagoTimeoutError)
                    {
                        continue;
                    }
                    
                    break;
                }

                if (retry == clientPerferences.RandomPortRetry)
                {
                    throw new IndagoInternalError(
                        $"Failed to bind to launched process in random port for {retry} times");
                }
            }
            else
            {
                Connection = new(Arguments, ClientPerferences.ServerTimeout);
            }
        }
        else
        {
            Connection = new(Arguments, ClientPerferences.ServerTimeout);
        }
        
        if (Connection?.Alive == true)
        {
            Implementation = new(Arguments, Connection, this);
        }
        else
        {
            throw new IndagoInternalError("Cannot connect to Indago server.");
        }
        
        SignalContext = new(Implementation);
        ScopeContext = new(Implementation);
    }

    /// <summary>
    /// The event system to register and monitoring events from Indago Server
    /// </summary>
    public IIndagoEventSystem EventSystem => Implementation?.EventSystem ?? throw new IndagoInternalError("Cannot get event system from Indago server.");
    
    public void Dispose()
    {
        Implementation?.Dispose();
        Process?.Dispose();
    }

    public string Name => "";
    public string Path => "";
    
    private SignalContext SignalContext { get; }

    /// <summary>
    /// Get the queryable signal list from the <see cref="IndagoServer"/>.
    /// </summary>
    /// <param name="withTransitions">The fetched signals has transitions record</param>
    /// <param name="withDeclaration">The fetched signals has declaration record</param>
    /// <returns>Queryable signal list that support LINQ query on it</returns>
    /// <seealso cref="Signal"/>
    /// <seealso cref="Declaration"/>
    public IQueryable<Signal> Signals(bool withTransitions = false, bool withDeclaration = false)
    {
        SignalContext.WithTransitions = withTransitions;
        SignalContext.WithDeclaration = withDeclaration;

        return SignalContext;
    }
    
    private ScopeContext ScopeContext { get; }

    /// <summary>
    /// Get the queryable scope list from the <see cref="IndagoServer"/>.
    /// </summary>
    /// <param name="withDeclaration">The fetched signals has declaration record</param>
    /// <returns>Querable scope list that support LINQ query on it</returns>
    /// <seealso cref="Scope"/>
    /// <seealso cref="Declaration"/>
    public IQueryable<Scope> Scopes(bool withDeclaration = false)
    {
        ScopeContext.WithDeclaration = withDeclaration;

        return ScopeContext;
    }

    /// <summary>
    /// The top scope of the design
    /// </summary>
    public Scope? TopScope(bool withDeclaration = false)
    { 
        var scopes = Scopes(withDeclaration).Where(s => s.Depth == 0).ToList();
        return scopes.Count > 0 ? scopes[0] : null;
    }
    
    public TimePoint CurrentTime
    {
        get => Implementation?.GetCurrentTime().Result ?? 
               throw new IndagoInternalError("Cannot get current time from Indago server.");
        set => Implementation?.SetCurrentTime(value);
    }
}