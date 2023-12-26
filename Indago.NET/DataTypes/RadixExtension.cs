using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

public static class RadixExtension
{
    public static BusinessLogicRadixValue ToBusinessLogicRadixValue(this Radix radix)
        => radix switch
        {
            Radix.ASCII => BusinessLogicRadixValue.Ascii,
            Radix.Binary => BusinessLogicRadixValue.Binary,
            Radix.Octal => BusinessLogicRadixValue.Octal,
            Radix.Decimal => BusinessLogicRadixValue.Decimal,
            Radix.Hexadecimal => BusinessLogicRadixValue.Hexadecimal,
            Radix.AsRecorded => BusinessLogicRadixValue.AsRecorded,
            Radix.DecimalOnesComplement => BusinessLogicRadixValue.DecimalOnesComplement,
            Radix.DecimalTwosComplement => BusinessLogicRadixValue.DecimalTwosComplement,
            _ => throw new ArgumentOutOfRangeException(nameof(radix), radix, null)
        };
}