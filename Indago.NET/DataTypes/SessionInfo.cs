namespace Indago.DataTypes;

public class SessionInfo(
    string version,
    bool interactive = false,
    bool lightWeightDatabase = false,
    bool lp = false,
    bool esw = false,
    bool uvm = false,
    uint rpcCount = 0,
    bool guiMode = false,
    bool guiReady = false,
    ulong memory = 0UL,
    ulong allocatedMemory = 0UL,
    ulong maximumMemory = 0UL)
{
    public string Version => version;
    public bool Interactive => interactive;
    public bool LightWeightDatabase => lightWeightDatabase;
    public bool Lp => lp;
    public bool Esw => esw;
    public bool Uvm => uvm;
    public uint RpcCount => rpcCount;
    public bool GuiMode => guiMode;
    public bool GuiReady => guiReady;
    public ulong Memory => memory;
    public ulong AllocatedMemory => allocatedMemory;
    public ulong MaximumMemory => maximumMemory;

    public static implicit operator Com.Cadence.Indago.Scripting.Generated.SessionInfo(SessionInfo sessionInfo)
        => new()
        {
            Version = sessionInfo.Version,
            Interactive = sessionInfo.Interactive,
            Lwd = sessionInfo.LightWeightDatabase,
            Lp = sessionInfo.Lp,
            Esw = sessionInfo.Esw,
            Uvm = sessionInfo.Uvm,
                
            RpcCount = sessionInfo.RpcCount,
            GuiMode = sessionInfo.GuiMode,
            GuiReady = sessionInfo.GuiReady,
                
            Memory = sessionInfo.Memory,
            AllocatedMemory = sessionInfo.AllocatedMemory,
            MaxMemory = sessionInfo.MaximumMemory
        };

    public static implicit operator SessionInfo(Com.Cadence.Indago.Scripting.Generated.SessionInfo sessionInfo)
        => new(sessionInfo.Version, sessionInfo.Interactive, sessionInfo.Lwd,
            sessionInfo.Lp, sessionInfo.Esw, sessionInfo.Uvm,
            sessionInfo.RpcCount, sessionInfo.GuiMode, sessionInfo.GuiReady,
            sessionInfo.Memory, sessionInfo.AllocatedMemory, sessionInfo.MaxMemory);
}