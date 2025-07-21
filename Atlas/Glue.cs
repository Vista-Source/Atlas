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
        List<MethodInfo> methods = new List<MethodInfo>()
        {
            new MethodInfo()
            {
                Name = "add",
                ReturnType = "void",
                Parameters = "int a, int b"
            },
            new MethodInfo()
            {
                Name = "subtract",
                ReturnType = "int",
                Parameters = "int a, int b"
            }
        };

        var model = new { methods = methods };

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

public class MethodInfo
{
    public string Name { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string Parameters { get; set; } = "";
}
