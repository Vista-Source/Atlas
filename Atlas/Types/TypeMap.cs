
using Atlas.Writers;

namespace Atlas.Types;

/// <summary>
/// Base-Class for all TypeMaps.
/// </summary>
/// <remarks>TypeMaps are used to interop C++ types into C# types.</remarks>
public abstract class TypeMap
{
    /// <summary> Converts the native type into a C# type. </summary>
    public abstract void NativeToManaged(BaseWriter writer);
}

/// <summary>
/// Marks a class that inherits <see cref="TypeMap"/> as a TypeMap.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TypeMapAttribute : Attribute
{
    /// <summary> The C++ type this TypeMap is responsible for. </summary>
    public string NativeType { get; set; }

    /// <summary> Initializes a new instance of the <see cref="TypeMapAttribute"/> class. </summary>
    public TypeMapAttribute(string nativeType)
    {
        NativeType = nativeType;
    }
}
