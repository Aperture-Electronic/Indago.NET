using System.Text.RegularExpressions;
using Indago.ExceptionFlow;

namespace Indago.DataTypes;

public partial class TimePoint(float time, TimeUnit units, int? sequenceNumber = null)
{
    [GeneratedRegex(@"^\\s*([0-9.ed+-]+)\\s*[(]?([0-9.ed+-]*)[)]?\\s*([^\\s]*).*$")]
    private static partial Regex StringTimePatternRegex();

    [GeneratedRegex(@"^([0-9]+)[.]?([0-9]*)[ed]?([+-]?)([0-9]*)")]
    private static partial Regex TimePattrernRegex();

    public TimeUnit Units => units;
    public int ActualExponent { get; private set; }
    public int? SequenceNumber => sequenceNumber;
    public string Label => $"";
    
    private (int time, TimeUnit, int acutalExponent, int? seqNumber) ParseTimePointString(string timeString, TimeUnit? argumentUnit = null)
    {
        var match = StringTimePatternRegex().Match(timeString);

        if (!match.Success) throw new IndagoNumberFormatException($"Bad time format(3) {time}");

        string parsedTime = match.Groups[1].Value;
        string parsedSequence = match.Groups[2].Value;
        string parsedUnit = match.Groups[3].Value;

        if (string.IsNullOrWhiteSpace(parsedTime)) throw new IndagoNumberFormatException($"Bad time format(2) {time}");

        var timeMatch = TimePattrernRegex().Match(parsedTime);
        string parsedLeft = timeMatch.Groups[1].Value;
        string parsedRight = timeMatch.Groups[2].Value.TrimEnd('0');
        string parsedSign = timeMatch.Groups[3].Value;
        string parsedExponent = timeMatch.Groups[4].Value;
        
        // Integer value for time point
        int timeResult = int.Parse(string.Concat(parsedLeft, parsedRight));
        int sign = parsedSign == "-" ? -1 : 1;
        int exponent = string.IsNullOrWhiteSpace(parsedExponent) ? 0 : int.Parse(parsedExponent) * sign;
        
        // Actual exponent
        int actualExponent = exponent + parsedRight.Length;
        
        // Sequence number
        int? seqNumber = (string.IsNullOrWhiteSpace(parsedSequence) || parsedSequence.Contains('-'))
            ? int.Parse(parsedSequence)
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
    
    public TimePoint(int time, TimeUnit units, int? sequenceNumber = null) : 
        this((float)time, units, sequenceNumber)
    {
        
    }

    public TimePoint(string time, TimeUnit units, int? sequenceNumber = null) : 
        this(float.Parse(time), units, sequenceNumber)
    {
        
    }

    
}