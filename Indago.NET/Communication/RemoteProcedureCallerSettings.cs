using Indago.DataTypes;

namespace Indago.Communication;

public struct RemoteProcedureCallerSettings(
    ReturnType returnType = ReturnType.List,  
    ServiceType serviceType = ServiceType.BusinessLogic,
    bool requestIsStream = false, bool responseIsStream = false, bool responseMessageIsList = false)
{
    public ReturnType ReturnType => returnType;
    public ServiceType ServiceType => serviceType;
    public bool RequestIsStream => requestIsStream;
    public bool ResponseIsStream => responseIsStream;
    public bool ResponseMessageIsList => responseMessageIsList;
}