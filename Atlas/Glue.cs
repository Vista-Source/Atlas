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
        var methods = new List<MethodInfo>()
        {
            new MethodInfo("TestMethod", "void", "")
        };

        return TemplateEngine.RenderTemplate("extern_c_wrapper", new { methods });
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
    public string name { get; set; } = "";
    public string return_type { get; set; } = "";
    public string @params { get; set; } = "";

    public MethodInfo(string name, string return_type, string @params)
    {
        this.name = name;
        this.return_type = return_type;
        this.@params = @params;
    }
}