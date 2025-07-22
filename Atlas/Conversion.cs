using CppAst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas;

/// <summary>
/// Helpers for converting C++ stuff to C#.
/// </summary>
internal static class Conversion
{
    /// <summary>
    /// Converts a C++ type to a C# type.
    /// </summary>
    internal static string NormalizeType(CppType type)
    {
        // If the type is an enum/struct/class, extract just the name
        if (type is CppEnum enumType)
            return enumType.Name;

        if (type is CppTypedef typedef)
            return typedef.Name;

        return type.ToString();
    }
}
