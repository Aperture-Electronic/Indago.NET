namespace Indago.DataTypes;

/// <summary>
/// Indicates the type of scope query.
/// </summary>
public enum ScopeQueryType
{
    /// <summary>
    /// Normal/children query, returns all scopes under the current scope that match the criteria.
    /// </summary>
    NormalChildren,
    /// <summary>
    /// Query the parent scope of the current scope.
    /// </summary>
    Parent,
}