using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

/// <summary>
/// An abstraction containing the information about the declared type of
/// a <see cref="Scope"/> or <see cref="Signal"/>.
/// </summary>
public class Declaration(BusinessLogicDeclaration blDeclaration)
{
    /// <summary>
    /// Gets whether the declared scope is a user-defined primitive or not.
    /// </summary>
    public bool IsUserDefinedPrimitive => blDeclaration.IsUdp;

    /// <summary>
    /// Gets whether the declared scope is built-in primitive or not.
    /// </summary>
    public bool IsPrimitive => blDeclaration.IsPrimitive;

    /// <summary>
    /// Gets whether the declared scope is a library-cell-level scope.
    /// This includes modules declared in a celldefine block, Verilog and HDL primitives,
    /// and user-defined primitives (UDPs). These are all scopes that normally appear
    /// in the Indago GUI under the Library Cells folder in the Hierarchy browser.
    /// </summary>
    public bool IsCell => blDeclaration.IsCell;

    /// <summary>
    /// Get the declaration name, which is the user-defined name in the users source code.
    /// </summary>
    public string Name => blDeclaration.Name;
    
    /// <summary>
    /// Get the language of the declared source code for this object
    /// </summary>
    public Language Language => blDeclaration.Language.ToLanguage();

    /// <summary>
    /// Get the declaration type, which is set based on the specified type
    /// (e.g. module, interface, assertion) in the users source code.
    /// </summary>
    public DeclarationType Type => blDeclaration.Type.ToDeclarationType();

    /// <summary>
    /// Get the Declaration source information (file, line, etc.) for this object
    /// </summary>
    /// <seealso cref="SourceLocation"/>
    public SourceLocation Source { get; } = new(blDeclaration.Source);

    public override string ToString()
        => $"{Language}::{Type} {Name} @({Source})";
}