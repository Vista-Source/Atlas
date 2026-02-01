using CppAst;
using System.Collections.Generic;
using System.Linq;

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
    /// Maps C++ primitive types to C# equivalents.
    /// </summary>
    private static readonly Dictionary<string, string> CppToCSharpPrimitiveTypeMap = new()
    {
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
    public static string NormalizeType(CppType type, TargetLanguage target)
    {
        type = UnwrapQualifiedType(type);

        return type switch
        {
            CppReferenceType refType => HandleReferenceType(refType, target),
            CppPointerType ptrType => HandlePointerType(ptrType, target),
            CppPrimitiveType primType => HandlePrimitiveType(primType, target),
            CppEnum cppEnum => cppEnum.Name,
            CppClass cppClass => cppClass.Name,
            CppTypedef cppTypedef => cppTypedef.Name,
            _ => type.ToString()
        };
    }

    private static string HandleReferenceType(CppReferenceType refType, TargetLanguage target)
    {
        // For C#, we want to strip the reference (&) and const qualifiers
        if (target == TargetLanguage.CSharp)
        {
            return NormalizeType(refType.ElementType, target);
        }

        // For C++, we want to preserve the reference but still normalize the underlying type
        var elementType = NormalizeType(refType.ElementType, target);
        return $"{elementType}&";
    }

    private static string HandlePointerType(CppPointerType ptrType, TargetLanguage target)
    {
        var elementType = UnwrapQualifiedType(ptrType.ElementType);

        // Special case: const char* -> string in C#
        if (elementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char } &&
            elementType.GetDisplayName().StartsWith("const "))
        {
            return target == TargetLanguage.CSharp ? "string" : ptrType.ToString();
        }

        // General pointer handling: IntPtr for C#, original for C++
        return target == TargetLanguage.CSharp ? "IntPtr" : ptrType.ToString();
    }

    private static string HandlePrimitiveType(CppPrimitiveType primType, TargetLanguage target)
    {
        string cppName = primType.ToString();

        if (target == TargetLanguage.CSharp &&
            CppToCSharpPrimitiveTypeMap.TryGetValue(cppName, out var mapped))
        {
            return mapped;
        }

        return cppName;
    }

    /// <summary>
    /// Recursively unwraps qualified types (const, volatile, etc.) to get the underlying type.
    /// </summary>
    private static CppType UnwrapQualifiedType(CppType type)
    {
        while (type is CppQualifiedType qualified)
        {
            type = qualified.ElementType;
        }
        return type;
    }

    /// <summary>
    /// Gets the display name of a type without qualifiers or references.
    /// </summary>
    private static string GetBaseTypeName(CppType type)
    {
        type = UnwrapQualifiedType(type);
        return type switch
        {
            CppReferenceType refType => GetBaseTypeName(refType.ElementType),
            CppPointerType ptrType => GetBaseTypeName(ptrType.ElementType),
            CppPrimitiveType primType => primType.ToString(),
            CppEnum cppEnum => cppEnum.Name,
            CppClass cppClass => cppClass.Name,
            CppTypedef cppTypedef => cppTypedef.Name,
            _ => type.ToString()
        };
    }
}