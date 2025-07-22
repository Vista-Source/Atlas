using Atlas.Renderers;
using CppAst;
using System.Text;

namespace Atlas;

internal static class Glue
{
    internal static readonly List<IGlueRenderer> Renderers = new()
    {
        new FunctionRenderer(),
        new EnumRenderer()
    };

    /// <summary>
    /// Generates the extern "C" wrapper file.
    /// </summary>
    internal static string GenerateCPP(FileInfo cpp)
    {
        var options = new CppParserOptions
        {
            ParseSystemIncludes = false,
            SystemIncludeFolders = { }
        };

        var compilation = CppParser.ParseFile(cpp.FullName, options);

        // Run each renderer
        StringBuilder sb = new StringBuilder();
        foreach(var renderer in Renderers)
            sb.AppendLine(renderer.RenderCPP(compilation, cpp));

        return sb.ToString();
    }

    /// <summary>
    /// Generates the P/Invoke C# wrapper file.
    /// </summary>
    internal static string GenerateCS(FileInfo cpp)
    {
        var options = new CppParserOptions
        {
            ParseSystemIncludes = false,
            SystemIncludeFolders = { }
        };

        var compilation = CppParser.ParseFile(cpp.FullName, options);

        // Run each renderer
        StringBuilder sb = new StringBuilder();
        foreach (var renderer in Renderers)
            sb.AppendLine(renderer.RenderCSharp(compilation, cpp));

        return sb.ToString();
    }

    /// <summary>
    /// Generates the master C++ file that compiles all the generated headers.
    /// </summary>
    /// <param name="headers">Names of the generated headers.</param>
    internal static string GenerateMasterCPP(List<string> headers)
    {
        // Add file prefix
        headers = headers.Select(h =>
        {
            if (h.EndsWith(".h"))
                return h[..^2] + $".{Options.FilePrefix}.h"; // remove ".h" and add ".prefix.h"
            return h;
        }).ToList();

        var model = new { headers };
        return TemplateEngine.RenderTemplate("atlas_master_wrapper", model);
    }
}

