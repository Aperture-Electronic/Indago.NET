using System.Text.RegularExpressions;
using Com.Cadence.Indago.Scripting.Generated;
using Indago.ExceptionFlow;

namespace Indago.DataTypes;

public partial class TimePoint : IEquatable<TimePoint>, IEqualityComparer<TimePoint>
{
    [GeneratedRegex(@"^\\s*([0-9.ed+-]+)\\s*[(]?([0-9.ed+-]*)[)]?\\s*([^\\s]*).*$")]
    private static partial Regex StringTimePatternRegex();

    [GeneratedRegex(@"^([0-9]+)[.]?([0-9]*)[ed]?([+-]?)([0-9]*)")]
    private static partial Regex TimePattrernRegex();

    public ulong Time { get; set; }
    public TimeUnit Units { get; set; }
    public int ActualExponent { get; set; }
    public ulong SequenceNumber { get; set; }
    
    private static (ulong time, TimeUnit, int acutalExponent, ulong? seqNumber) ParseTimePointString(string timeString, TimeUnit? argumentUnit = null)
    {
        var match = StringTimePatternRegex().Match(timeString);

        if (!match.Success) throw new IndagoNumberFormatException($"Bad time format(3) {timeString}");

        string parsedTime = match.Groups[1].Value;
        string parsedSequence = match.Groups[2].Value;
        string parsedUnit = match.Groups[3].Value;

        if (string.IsNullOrWhiteSpace(parsedTime)) throw new IndagoNumberFormatException($"Bad time format(2) {timeString}");

        var timeMatch = TimePattrernRegex().Match(parsedTime);
        string parsedLeft = timeMatch.Groups[1].Value;
        string parsedRight = timeMatch.Groups[2].Value.TrimEnd('0');
        string parsedSign = timeMatch.Groups[3].Value;
        string parsedExponent = timeMatch.Groups[4].Value;
        
        // Integer value for time point
        ulong timeResult = ulong.Parse(string.Concat(parsedLeft, parsedRight));
        int sign = parsedSign == "-" ? -1 : 1;
        int exponent = string.IsNullOrWhiteSpace(parsedExponent) ? 0 : int.Parse(parsedExponent) * sign;
        
        // Actual exponent
        int actualExponent = exponent + parsedRight.Length;
        
        // Sequence number
        ulong? seqNumber = (string.IsNullOrWhiteSpace(parsedSequence) || parsedSequence.Contains('-'))
            ? ulong.Parse(parsedSequence)
            : null;
        
        // Units
        if (!string.IsNullOrWhiteSpace(parsedUnit)) throw new ArgumentException("Conflicting time unit specification");

        var units = (parsedUnit, unit: argumentUnit) switch
        {
            (not "", _) => TimeUnitExtension.ParseTimeUnit(parsedUnit),
            (_, { } u) => u,
            _ => throw new NotImplementedException()
        };

        return (timeResult, units, actualExponent, seqNumber);
    }

    public TimePoint(BusinessLogicTimePoint timePoint)
    {
        Time = timePoint.Time;
        Units = (TimeUnit)timePoint.Units;
        ActualExponent = timePoint.ActualExp;
        SequenceNumber = timePoint.Seq;
    }

    /// <summary>
    /// Create a new timepoint with specified time and units
    /// </summary>
    /// <param name="time">Time value</param>
    /// <param name="units">Units, set to null means nanoseconds(ns)</param>
    /// <param name="sequenceNumber">[Optional] Sequence number of the timepoint</param>
    public TimePoint(ulong time, TimeUnit? units = null, ulong sequenceNumber = 0)
    {
        Time = time;
        Units = units ?? TimeUnit.Nanoseconds;
        ActualExponent = (int)Units;
        SequenceNumber = sequenceNumber;
    }

    public TimePoint(string timeString, TimeUnit? units = null)
    {
        (ulong time, TimeUnit parsedUnits, int actualExp, ulong? seqNumber) parsed =
            ParseTimePointString(timeString, units);
        
        Time = parsed.time;
        Units = parsed.parsedUnits;
        ActualExponent = parsed.actualExp;
        SequenceNumber = parsed.seqNumber ?? 0;
    }

    public static implicit operator TimePoint(BusinessLogicTimePoint timePoint) => new(timePoint);

    public static implicit operator BusinessLogicTimePoint(TimePoint timePoint) => new BusinessLogicTimePoint()
    {
        ActualExp = timePoint.ActualExponent,
        Seq = timePoint.SequenceNumber,
        Time = timePoint.Time,
        Units = (int)timePoint.Units
    };

    public bool Equals(TimePoint? other)
    {
        if (other is null) return false;
        throw new NotImplementedException();
    }

    public override string ToString()
        => $"{Time} {Units.ToAbbreviation().ToLower()} [{SequenceNumber}]";

    /// <summary>
    /// Convert the timepoint to specified unit
    /// and do not change the time value
    /// Note: If the units is equal to the current unit,
    /// this method will create a new instance of <see cref="TimePoint"/>
    /// </summary>
    public TimePoint ConvertUnitTo(TimeUnit units)
    {
        throw new NotImplementedException();

    }
    
    public bool Equals(TimePoint? x, TimePoint? y) => x?.Equals(y) ?? false;

    public int GetHashCode(TimePoint obj) => throw new NotImplementedException();
}