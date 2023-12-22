namespace Indago;

public class ClientPerferences(bool quiet = false, int randomPortRetry = 3, int serverTimeout = 500)
{
    /// <summary>
    /// Disable all console output from the server.
    /// </summary>
    public bool Quiet => quiet;
    
    /// <summary>
    /// Allow how many retries when the server fails to open a random port.
    /// </summary>
    public int RandomPortRetry => randomPortRetry;

    /// <summary>
    /// Server connection timeout in milliseconds. 
    /// </summary>
    public int ServerTimeout => serverTimeout;
}