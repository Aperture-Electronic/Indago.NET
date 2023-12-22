using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

public static class TimeUnitExtension
{
    public static TimeUnit ParseTimeUnit(string str)
        => str.ToLower() switch
        {
            "s" => TimeUnit.Seconds,
            "sec" => TimeUnit.Seconds,
            "ms" => TimeUnit.Milliseconds,
            "us" => TimeUnit.Microseconds,
            "ns" => TimeUnit.Nanoseconds,
            "ps" => TimeUnit.Picoseconds,
            "fs" => TimeUnit.Femtoseconds,
            "zs" => TimeUnit.Zeptoseconds,
            _ => throw new ArgumentOutOfRangeException(nameof(str))
        };
    
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
}