namespace IndagoSharp.DataTypes;

/// <summary>
/// Indicating whether a Contributor signal is part of the
/// value or control path of the driving logic.
/// </summary>
public enum ContributorType
{
    /// <summary>
    /// A contributor that is part of the value/data path in the driving logic.
    /// </summary>
    Value,
    /// <summary>
    /// A contributor that is part of the control path of the driving logic.
    /// (example: sensitivity list signals, clocks, ternary operator controls.)
    /// </summary>
    Control,
    /// <summary>
    /// An automatically-added contributor that is an input port of a scope in
    /// a higher level of hierarchy from the driver's scope.  These contributors
    /// are useful for situations where the driver scope is not probed, but the
    /// user wishes to have some information about potential contributors from
    /// a probed hierarchy above.
    /// </summary>
    Implicit
}