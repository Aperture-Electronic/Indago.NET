namespace Indago.DataTypes;

/// <summary>
/// This class is an argument builder for opening an instance of <see cref="ServerUtils.IndagoProcess"/>.
/// </summary>
public class IndagoArgs(string? indagoRoot = null, bool isGui = false,
    string? dbPath = null, bool isLaunchNeeded = true,
    int? port = null, string host = "localhost", string? lwd = null, IEnumerable<string>? extraArgs = null)
{
    public string? IndagoRoot => indagoRoot;
    public bool IsGui => isGui;
    public string? DbPath => dbPath;
    public bool IsLaunchNeeded => isLaunchNeeded;

    public int? Port
    {
        get => port;
        set => port = value;
    }
    
    public string Host => host;
    public string? Lwd => lwd;
    public IEnumerable<string>? ExtraArgs => extraArgs;

    public IEnumerable<string> GetParameterList()
    {
        if (indagoRoot is null) yield break;
        
        if (!isGui) yield return "-nogui";
        if (port is not null) yield return $"-port {port}";
        if (dbPath is not null) yield return $"-db {dbPath}";
        if (lwd is not null) yield return $"-lwd {lwd}";
        if (extraArgs is not null) yield return string.Join(" ", extraArgs);
    }
}