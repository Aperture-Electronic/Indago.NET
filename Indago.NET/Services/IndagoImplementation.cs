using Indago.DataTypes;

namespace Indago.Services;

public class IndagoImplementation
{
    private readonly IndagoArgs indagoArguments;
    
    public IndagoImplementation(IndagoArgs args)
    {
        indagoArguments = args;

        try
        {
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}