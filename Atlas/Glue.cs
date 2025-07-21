using System.Text;
using Atlas.Extensions;
using CppAst;
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
        var methods = new List<MethodInfo>();

        CppParserOptions options = new CppParserOptions()
        {
            ParseSystemIncludes = false,
            SystemIncludeFolders = {}
        };

        // Parse Cpp
        var parsedCPP = CppParser.ParseFile(cpp.FullName, options);

        // Print diagnostic messages
        foreach (var message in parsedCPP.Diagnostics.Messages)
            Console.WriteLine(message);

        // Read file lines for function body
        var fileLines = File.ReadAllLines(cpp.FullName);

        // Parse top level (non classed) functions
        foreach (var function in parsedCPP.Functions)
        {
            if (function.Comment.ToString() != Options.ExportComment)
                continue;

            // Collect parameter strings in a list
            var paramList = new List<string>();
            foreach (var parameter in function.Parameters)
            {
                paramList.Add($"{parameter.Type} {parameter.Name}");
            }

            // Join parameters with ", " (no trailing comma)
            var parameters = string.Join(", ", paramList);

            methods.Add(new MethodInfo
            {
                Name = function.Name,
                ReturnType = function.ReturnType.ToString(),
                Parameters = parameters,
                Body = function.ExtractBody(fileLines)
            });
        }

        var model = new { methods = methods };

        return TemplateEngine.RenderTemplate("extern_c_wrapper", model);
    }

    /// <summary>
    /// Generates the P/Invoke C# wrapper file.
    /// </summary>
    /// <param name="cpp">C++ Header.</param>
    internal static string GenerateCS(FileInfo cpp)
    {
        var methods = new List<MethodInfo>();

        CppParserOptions options = new CppParserOptions()
        {
            ParseSystemIncludes = false,
            SystemIncludeFolders = { }
        };

        // Parse Cpp
        var parsedCPP = CppParser.ParseFile(cpp.FullName, options);

        // Print diagnostic messages
        foreach (var message in parsedCPP.Diagnostics.Messages)
            Console.WriteLine(message);

        // Parse top level (non classed) functions
        foreach (var function in parsedCPP.Functions)
        {
            if (function.Comment.ToString() != Options.ExportComment)
                continue;

            // Collect parameter strings in a list
            var paramList = new List<string>();
            foreach (var parameter in function.Parameters)
            {
                paramList.Add($"{parameter.Type} {parameter.Name}");
            }

            // Join parameters with ", " (no trailing comma)
            var parameters = string.Join(", ", paramList);

            methods.Add(new MethodInfo
            {
                Name = function.Name,
                ReturnType = function.ReturnType.ToString(),
                Parameters = parameters,
            });
        }

        var model = new Dictionary<string, object>
        {
            ["libName"] = Options.LibraryName,
            ["methods"] = methods
        };

        return TemplateEngine.RenderTemplate("pinvoke_wrapper", model);
    }
}

public class MethodInfo
{
    public string Name { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string Parameters { get; set; } = "";
    public string Body { get; set; } = "";
}
