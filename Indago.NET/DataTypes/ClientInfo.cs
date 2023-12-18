namespace Indago.DataTypes;

public class ClientInfo (string host, int port, string clientPath, bool embedded)
{
    public string Host => host;
    public int Port => port;
    public string ClientPath => clientPath;
    public bool Embedded => embedded;
}