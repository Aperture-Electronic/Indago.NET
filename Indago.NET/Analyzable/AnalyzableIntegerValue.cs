using System.Numerics;
using System.Text.RegularExpressions;
using Indago.DataTypes;

namespace Indago.Analyzable;

public partial class AnalyzableIntegerValue
{
    public BigInteger Value { get; }
    
    [GeneratedRegex(@"(?<size>\d*)'(?<signed>[sS]?)(?<radix>[bBoOdDhH]?)(?<value>[a-fA-F\d_]+)")]
    private static partial Regex VerilogValueRegex();

    public AnalyzableIntegerValue(BigInteger value)
    {
        Value = value;
    }
    
    public AnalyzableIntegerValue(string stringValue)
    {
        // Match the verilog style value string
        string valueString = stringValue.Trim();
        
        var match = VerilogValueRegex().Match(valueString);

        if (!match.Success)
        {
            throw new ArgumentException("The value string is not in Verilog style.", nameof(stringValue));
        }
        
        var sizeGroup = match.Groups["size"];
        // var signGroup = match.Groups["signed"];
        var radixGroup = match.Groups["radix"];
        var valueGroup = match.Groups["value"];

        // Replace the delimiters in value string
        string valuePureString = valueGroup.Value.Replace("_", "").ToLower();
        
        // Check the radix, if not specified, use decimal
        var radix = (radixGroup.Value, radixGroup.Value.ToLower()) switch
        {
            ("", _) => Radix.Decimal,
            (_, "b") => Radix.Binary,
            (_, "o") => Radix.Octal,
            (_, "d") => Radix.Decimal,
            (_, "h") => Radix.Hexadecimal,
            _ => throw new ArgumentException($"The radix {radixGroup.Value} is not supported.", nameof(stringValue))
        };
        
        // Check if the value string has the correct radix
        bool isCorrectRadix = radix switch
        {
            Radix.Binary => valuePureString.All(c => c is '0' or '1'),
            Radix.Octal => valuePureString.All(c => c is >= '0' and <= '7'),
            Radix.Decimal => valuePureString.All(c => c is >= '0' and <= '9'),
            _ => valuePureString.All(c => c is (>= '0' and <= '9') or (>= 'a' and <= 'f'))
        };
        
        if (!isCorrectRadix)
        {
            throw new ArgumentException($"The value string {valuePureString} is not in {radix} radix.", nameof(stringValue));
        }
        
        // Generate the big integer value
        int length = valuePureString.Length;
        Value = new(0);
        for (var i = 0; i < length; i++)
        {
            char c = valuePureString[i];
            Value *= radix switch
            {
                Radix.Binary => 2,
                Radix.Octal => 8,
                Radix.Decimal => 10,
                _ => 16
            };

            Value += c switch
            {
                >= '0' and <= '9' => c - '0',
                _ => c - 'a' + 10
            };
        }
        
        // If specified the size, generate a mask for it
        if (sizeGroup.Value.Length <= 0) return;
        int size = int.Parse(sizeGroup.Value);
        Value &= (new BigInteger(1) << size) - 1;
    }

    public static implicit operator AnalyzableIntegerValue(TimeValue timeValue)
        => new (timeValue.ValueString);

    public string HexadecimalString => Value.ToString("X");

    /// <summary>
    /// Select the bit or part of the value
    /// </summary>
    /// <param name="msb">Most significant bit or specific to select</param>
    /// <param name="lsb">Least significant bit to select, if select one bit, set to null</param>
    /// <returns>Selected bit or range</returns>
    public AnalyzableIntegerValue BitSelect(int msb, int? lsb = null)
    {
        if (lsb == null)
        {
            // Bit select
            return new((Value >> msb) & 1);
        }
        
        // Part select
        int length = msb - lsb.Value + 1;
        return new((Value >> lsb.Value) & ((new BigInteger(1) << length) - 1));
    }

    public AnalyzableIntegerValue this[Range range] => BitSelect(range.Start.Value, range.End.Value);
    public AnalyzableIntegerValue this[(int start, int length) v] => BitSelect(v.start + v.length - 1, v.start);
    public AnalyzableIntegerValue this[int index] => BitSelect(index);
}