using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

public static class TimeUnitExtension
{
    public static string ToAbbreviation(this TimeUnit unit)
        => unit switch
        {
            TimeUnit.Seconds => "s",
            TimeUnit.Milliseconds => "ms",
            TimeUnit.Microseconds => "us",
            TimeUnit.Nanoseconds => "ns",
            TimeUnit.Picoseconds => "ps",
            TimeUnit.Femtoseconds => "fs",
            TimeUnit.Zeptoseconds => "zs",
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };
    
    public static BusinessLogicTimeUnits ToBusinessLogic(this TimeUnit unit)
        => unit switch
        {
            TimeUnit.Seconds => BusinessLogicTimeUnits.S,
            TimeUnit.Milliseconds => BusinessLogicTimeUnits.Ms,
            TimeUnit.Microseconds => BusinessLogicTimeUnits.Us,
            TimeUnit.Nanoseconds => BusinessLogicTimeUnits.Ns,
            TimeUnit.Picoseconds => BusinessLogicTimeUnits.Ps,
            TimeUnit.Femtoseconds => BusinessLogicTimeUnits.Fs,
            TimeUnit.Zeptoseconds => BusinessLogicTimeUnits.Zs,
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };

    public static TimeUnit ParseTimeUnit(this string unitString)
        => unitString switch
        {
            "s" => TimeUnit.Seconds,
            "sec" => TimeUnit.Seconds,
            "ms" => TimeUnit.Milliseconds,
            "us" => TimeUnit.Microseconds,
            "ns" => TimeUnit.Nanoseconds,
            "ps" => TimeUnit.Picoseconds,
            "fs" => TimeUnit.Femtoseconds,
            "zs" => TimeUnit.Zeptoseconds,
            _ => Enum.Parse<TimeUnit>(unitString)
        };
}