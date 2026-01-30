using BfevLibrary.Parsers;

namespace BfevLibrary.Core;

public class VariableDefValue
{
    public bool? Bool { get; set; }
    
    public int? Int { get; set; }

    public float? Float { get; set; }

    public bool[]? BoolArray { get; set; }

    public int[]? IntArray { get; set; }

    public float[]? FloatArray { get; set; }

    protected VariableDefValue()
    {
    }
    
    protected void Read(BfevReader reader, long value, ushort count, ContainerDataType type)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (type) {
            case ContainerDataType.Bool:
                Bool = (int)value != 0;
                break;
            case ContainerDataType.Int:
                Int = (int)value;
                break;
            case ContainerDataType.Float:
                Float = (float)value;
                break;
            case ContainerDataType.BoolArray: {
                BoolArray = reader.ReadObjectsPtr(new bool[count], () => reader.ReadInt32() != 0, value);
                break;
            }
            case ContainerDataType.IntArray: {
                IntArray = reader.ReadObjectsPtr(new int[count], reader.ReadInt32, value);
                break;
            }
            case ContainerDataType.FloatArray: {
                FloatArray = reader.ReadObjectsPtr(new float[count], reader.ReadSingle, value);
                break;
            }
            default:
                throw new NotSupportedException($"Unsupported variable type: {type}");
        }
    }

    protected Action? WriteData(BfevWriter writer)
    {
        if (Bool != null) {
            writer.Write(Bool.Value ? 1L : 0L);
            return null;
        }
        
        if (Int != null) {
            writer.Write((ulong)Int);
            return null;
        }
        
        if (Float != null) {
            writer.Write((ulong)Float);
            return null;
        }
        
        if (BoolArray != null || IntArray != null || FloatArray != null) {
            return writer.ReservePtr();
        }

        throw new InvalidOperationException("Invalid variable def data");
    }

    protected int GetCount(ContainerDataType type)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return type switch {
            ContainerDataType.Bool => 1,
            ContainerDataType.Int => 1,
            ContainerDataType.Float => 1,
            ContainerDataType.BoolArray => BoolArray!.Length,
            ContainerDataType.IntArray => IntArray!.Length,
            ContainerDataType.FloatArray => FloatArray!.Length,
            _ => throw new NotSupportedException($"Unsupported variable type: {type}")
        };
    }

    public ContainerDataType GetDataType()
    {
        if (Bool != null) {
            return ContainerDataType.Bool;
        }
        
        if (Int != null) {
            return ContainerDataType.Int;
        }

        if (Float != null) {
            return ContainerDataType.Float;
        }

        if (BoolArray != null) {
            return ContainerDataType.BoolArray;
        }

        if (IntArray != null) {
            return ContainerDataType.IntArray;
        }

        if (FloatArray != null) {
            return ContainerDataType.FloatArray;
        }

        throw new NullReferenceException("Missing variable data. All possible union types were null.");
    }
}