namespace Indago.DataTypes;

/// <summary>
/// Enumeration for the radix of a value.
/// </summary>
public enum Radix
{
    /// <summary>
    /// The value is displayed using ASCII characters
    /// </summary>
    ASCII,
    /// <summary>
    /// The value is displayed using binary radix (0 and 1)
    /// </summary>
    Binary,
    /// <summary>
    /// The value is displayed using octal radix (0-7)
    /// </summary>
    Octal,
    /// <summary>
    /// The value is displayed using decimal radix (0-9)
    /// </summary>
    Decimal,
    /// <summary>
    /// The value is displayed using hexadecimal radix (0-9, A-F)
    /// </summary>
    Hexadecimal,
    /// <summary>
    /// The value is displayed using the recorded radix of the signal
    /// </summary>
    AsRecorded,
    /// <summary>
    /// The value is displayed using the 1's complement of the signed decimal radix
    /// </summary>
    DecimalOnesComplement,
    /// <summary>
    /// The value is displayed using the 2's complement of the signed decimal radix
    /// </summary>
    DecimalTwosComplement,
}