using CppAst;
using System.Collections.Generic;

namespace Atlas;

internal enum TargetLanguage
{
    Cpp,
    CSharp,
}

/// <summary>
/// Helpers for converting C++ types to target language syntax.
/// </summary>
internal static class ConversionUtility
{
    /// <summary>
    /// Maps C++ types to C# equivalents.
    /// </summary>
    private static readonly Dictionary<string, string> CppToCSharpTypeMap = new()
    {
        // Primitive types
        ["void"] = "void",
        ["bool"] = "bool",
        ["char"] = "sbyte",
        ["signed char"] = "sbyte",
        ["unsigned char"] = "byte",
        ["short"] = "short",
        ["short int"] = "short",
        ["unsigned short"] = "ushort",
        ["int"] = "int",
        ["signed int"] = "int",
        ["unsigned int"] = "uint",
        ["long"] = "IntPtr",       // platform-dependent
        ["unsigned long"] = "UIntPtr",
        ["long long"] = "long",
        ["unsigned long long"] = "ulong",
        ["float"] = "float",
        ["double"] = "double",
        ["const char *"] = "string",
    };

    /// <summary>
    /// Converts a C++ type to the appropriate target language syntax.
    /// </summary>
    /// <param name="type">The C++ type to convert.</param>
    /// <param name="target">The target language for conversion.</param>
    /// <returns>The converted type name as a string.</returns>
    public static string NormalizeType(CppType type, TargetLanguage target)
    {
        // Unwrap qualified types for consistent processing
        type = UnwrapQualifiedType(type);

        return type switch
        {
            CppEnum e => e.Name,
            CppClass c => c.Name,
            CppTypedef td => td.Name,
            CppPointerType ptr => HandlePointerType(ptr, target),
            CppPrimitiveType primType => HandlePrimitiveType(primType, target),
            _ => type.ToString()
        };
    }

    /// <summary>
    /// Handles pointer type conversion logic.
    /// </summary>
    private static string HandlePointerType(CppPointerType ptr, TargetLanguage target)
    {
        var elem = UnwrapQualifiedType(ptr.ElementType);

        // Special case: const char* -> string in C#
        if (elem is CppPrimitiveType prim && prim.Kind == CppPrimitiveKind.Char)
        {
            return target == TargetLanguage.CSharp ? "string" : ptr.ToString();
        }

        // General pointer handling: IntPtr for C#, original for C++
        return target == TargetLanguage.CSharp ? "IntPtr" : ptr.ToString();
    }

    /// <summary>
    /// Handles primitive type conversion logic.
    /// </summary>
    private static string HandlePrimitiveType(CppPrimitiveType primType, TargetLanguage target)
    {
        string cppName = primType.ToString();

        if (target == TargetLanguage.CSharp &&
            CppToCSharpTypeMap.TryGetValue(cppName, out var mapped))
        {
            return mapped;
        }

        return cppName;
    }

    /// <summary>
    /// Recursively unwraps qualified types to get the underlying type.
    /// </summary>
    private static CppType UnwrapQualifiedType(CppType type)
    {
        while (type is CppQualifiedType qualified)
            type = qualified.ElementType;
        return type;
    }
}