using Indago.ExceptionFlow;

namespace Indago.DataTypes;

public static class LanguageExtension
{
    public static Language ToLanguage(this Com.Cadence.Indago.Scripting.Generated.Language language)
        => language switch
        {
            Com.Cadence.Indago.Scripting.Generated.Language.Vhdl => Language.VHDL,
            Com.Cadence.Indago.Scripting.Generated.Language.Verilog => Language.Verilog,
            _ => throw new IndagoUnsupportedFunctionError("Language not supported")
        };
}