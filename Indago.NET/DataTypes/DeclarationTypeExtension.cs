using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

public static class DeclarationTypeExtension
{
    public static DeclarationType ToDeclarationType(this BusinessLogicInternalType type)
        => type switch
        {
            BusinessLogicInternalType.UnknownInternalType => DeclarationType.Variable,
            BusinessLogicInternalType.Field => DeclarationType.Field,
            BusinessLogicInternalType.Event => DeclarationType.Event,
            BusinessLogicInternalType.Constraint => DeclarationType.Constraint,
            BusinessLogicInternalType.Module => DeclarationType.Module,
            BusinessLogicInternalType.SvInterface => DeclarationType.Interface,
            BusinessLogicInternalType.Modport => DeclarationType.Modport,
            BusinessLogicInternalType.Reference => DeclarationType.Reference,
            BusinessLogicInternalType.Program => DeclarationType.Program,
            BusinessLogicInternalType.VhdlComponent => DeclarationType.VHDLComponent,
            BusinessLogicInternalType.VhdlBlock => DeclarationType.VHDLBlock,
            BusinessLogicInternalType.BeginBlock => DeclarationType.BeginBlock,
            BusinessLogicInternalType.ClockingBlock => DeclarationType.ClockingBlock,
            BusinessLogicInternalType.Package => DeclarationType.Package,
            BusinessLogicInternalType.Generate => DeclarationType.Generate,
            BusinessLogicInternalType.GenerateIteration => DeclarationType.GenerateIteration,
            BusinessLogicInternalType.CompilationUnit => DeclarationType.CompilationUnit,
            BusinessLogicInternalType.Primitive => DeclarationType.Primitive,
            BusinessLogicInternalType.Udp => DeclarationType.UserDefinedPrimitive,
            BusinessLogicInternalType.Subroutine => DeclarationType.Subroutine,
            BusinessLogicInternalType.VhdlProcess => DeclarationType.VHDLProcess,
            BusinessLogicInternalType.Always => DeclarationType.Always,
            BusinessLogicInternalType.Initial => DeclarationType.Initial,
            BusinessLogicInternalType.Final => DeclarationType.Final,
            BusinessLogicInternalType.Method => DeclarationType.Method,
            BusinessLogicInternalType.Port => DeclarationType.Port,
            BusinessLogicInternalType.Class => DeclarationType.Class,
            BusinessLogicInternalType.Variable => DeclarationType.Variable,
            BusinessLogicInternalType.Net => DeclarationType.Net,
            BusinessLogicInternalType.Enum => DeclarationType.Enumeration,
            BusinessLogicInternalType.Signal => DeclarationType.Signal,
            BusinessLogicInternalType.Parameter => DeclarationType.Parameter,
            BusinessLogicInternalType.Generic => DeclarationType.Generic,
            BusinessLogicInternalType.Real => DeclarationType.Real,
            BusinessLogicInternalType.Assertion => DeclarationType.Assertion,
            BusinessLogicInternalType.Property => DeclarationType.Property,
            BusinessLogicInternalType.Assertseq => DeclarationType.AssertSequence,
            BusinessLogicInternalType.Transaction => DeclarationType.Transaction,
            BusinessLogicInternalType.InterfaceRef => DeclarationType.InterfaceReference,
            BusinessLogicInternalType.StaticArray => DeclarationType.StaticArray,
            BusinessLogicInternalType.PackedArray => DeclarationType.PackedArray,
            BusinessLogicInternalType.DynamicArray => DeclarationType.DynamicArray,
            BusinessLogicInternalType.AssociativeArray => DeclarationType.AssociativeArray,
            BusinessLogicInternalType.Queue => DeclarationType.Queue,
            BusinessLogicInternalType.PackedStruct => DeclarationType.PackedStructure,
            BusinessLogicInternalType.PackedUnion => DeclarationType.PackedUnion,
            BusinessLogicInternalType.UnpackedStruct => DeclarationType.UnpackedStructure,
            BusinessLogicInternalType.String => DeclarationType.String,
            BusinessLogicInternalType.VhdlEnum => DeclarationType.VHDLEnumeration,
            BusinessLogicInternalType.SupplyNet => DeclarationType.SupplyNet,
            BusinessLogicInternalType.Expression => DeclarationType.Expression,
            BusinessLogicInternalType.Literal => DeclarationType.Literal,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}