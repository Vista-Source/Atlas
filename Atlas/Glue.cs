using ClangSharp;
using ClangSharp.Interop;
using System.Text;
using Scriban;

namespace Atlas;

/// <summary>
/// Compiler C++/C#.
/// </summary>
internal static class Glue
{
    /// <summary>
    /// Generates the extern "C" wrapper file.
    /// </summary>
    /// <param name="cpp">C++ Header.</param>
    internal static string GenerateCPP(FileInfo cpp)
    {
        var model = new
        {
            method_name = "test",
            return_type = "void",
        };

        return TemplateEngine.RenderTemplate("extern_c_wrapper", model);
    }

    /// <summary>
    /// Generates the P/Invoke C# wrapper file.
    /// </summary>
    /// <param name="cpp">C++ Header.</param>
    internal static string GenerateCS(FileInfo cpp)
    {
        return "";
    }
}
