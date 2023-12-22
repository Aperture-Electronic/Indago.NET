namespace Indago.DataTypes;

/// <summary>
/// An enumeration for declared type of a <see cref="Signal"/> or <see cref="Scope"/>
/// </summary>
public enum DeclarationType
{
    // List of possible values for Scopes
    // Module, Interface, Modport, ClockingBlock, Generate, GenreateIteration,
    // StaticArray, VHDLBlock, VHDLComponent
    
    // List of possible values for Signals
    // Variable, Net, Enumeration, Event, Parameter, Generic, Real, Assertion,
    // Property, AssertSequence, Transaction, TypeDefination, Other,
    // InterfaceReference, PackedArray, DynamicArray, AssociativeArray, Queue,
    // PackedStructure, PackedUnion, String, UnpackedStructure, VHDLEnumeration, 
    // SupplyNet, Expression, Literal
    
    // Other values in the enumeration are reserved for future use
    
    Field,
    Event,
    Constraint,
    Module,
    Interface,
    Modport,
    Reference,
    Program,
    VHDLComponent,
    VHDLBlock,
    BeginBlock,
    ClockingBlock,
    Package,
    /// <summary>
    /// A Verilog generate block
    /// </summary>
    Generate,
    /// <summary>
    /// An instance created in a generate block iteration
    /// </summary>
    GenerateIteration,
    CompilationUnit,
    Primitive,
    UserDefinedPrimitive,
    Subroutine,
    VHDLProcess,
    Always,
    Initial,
    Final,
    Method,
    Port,
    Class,
    /// <summary>
    /// A nonscalar wire, reg, or other variable
    /// </summary>
    Variable,
    /// <summary>
    /// A scalar net (includes wire)
    /// </summary>
    Net,
    Enumeration,
    Signal,
    Parameter,
    /// <summary>
    /// A generic type
    /// </summary>
    Generic,
    Real,
    Assertion,
    Property,
    AssertSequence,
    Transaction,
    /// <summary>
    /// An interface_ref
    /// </summary>
    InterfaceReference,
    /// <summary>
    /// An instance that is part of of a declared static array of scopes
    /// </summary>
    StaticArray,
    PackedArray,
    DynamicArray,
    AssociativeArray,
    Queue,
    PackedStructure,
    PackedUnion,
    UnpackedStructure,
    String,
    VHDLEnumeration,
    SupplyNet,
    /// <summary>
    /// A user-defined expression
    /// </summary>
    Expression,
    /// <summary>
    /// A literal or constant value
    /// </summary>
    Literal
}