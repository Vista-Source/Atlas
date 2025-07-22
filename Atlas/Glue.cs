using CppAst;

namespace Atlas;

internal static class Glue
{
    /// <summary>
    /// Generates the extern "C" wrapper file.
    /// </summary>
    internal static string GenerateCPP(FileInfo cpp)
    {
        var methods = ExtractMethods(cpp, includeBody: true);
        var model = new Dictionary<string, object>
        {
            ["original_header"] = cpp.Name,
            ["methods"] = methods
        };
        return TemplateEngine.RenderTemplate("methods", model);
    }

    /// <summary>
    /// Generates the P/Invoke C# wrapper file.
    /// </summary>
    internal static string GenerateCS(FileInfo cpp)
    {
        var methods = ExtractMethods(cpp, includeBody: false);
        var model = new Dictionary<string, object>
        {
            ["namespace"] = Options.Namespace,
            ["libName"] = Options.LibraryName,
            ["methods"] = methods
        };
        return TemplateEngine.RenderTemplate("pinvoke_wrapper", model);
    }

    /// <summary>
    /// Generates the master C++ file that compiles all the generated headers.
    /// </summary>
    /// <param name="headers">Names of the generated headers.</param>
    internal static (string source, string header) GenerateMasterCPP(List<string> headers)
    {
        // Add file prefix
        headers = headers.Select(h =>
        {
            if (h.EndsWith(".h"))
                return h[..^2] + $".{Options.FilePrefix}.h"; // remove ".h" and add ".prefix.h"
            return h;
        }).ToList();
        var model = new { headers };

        var source = TemplateEngine.RenderTemplate("atlas_master_wrapper", model);
        var header = TemplateEngine.RenderTemplate("atlas_master_header_wrapper", model);

        return (source, header);
    }

    /// <summary>
    /// Extracts exported methods from a C++ file.
    /// </summary>
    private static List<MethodInfo> ExtractMethods(FileInfo cpp, bool includeBody)
    {
        var methods = new List<MethodInfo>();

        var options = new CppParserOptions
        {
            ParseSystemIncludes = false,
            SystemIncludeFolders = { }
        };

        var parsedCPP = CppParser.ParseFile(cpp.FullName, options);

        foreach (var message in parsedCPP.Diagnostics.Messages)
            Console.WriteLine(message);

        var fileLines = includeBody ? File.ReadAllLines(cpp.FullName) : null;

        foreach (var function in parsedCPP.Functions)
        {
            if (function.Comment.ToString() != Options.ExportComment)
                continue;

            var parameters = string.Join(", ",
                function.Parameters.Select(p => $"{p.Type} {p.Name}"));

            var typelessParameters = string.Join(", ",
                function.Parameters.Select(p => $"{p.Name}"));

            var method = new MethodInfo
            {
                Name = function.Name,
                ReturnType = function.ReturnType.ToString(),
                Parameters = parameters,
                Body = includeBody ? $"{function.Name}({typelessParameters});" : ""
            };

            methods.Add(method);
        }

        return methods;
    }
}

public class MethodInfo
{
    public string Name { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string Parameters { get; set; } = "";
    public string Body { get; set; } = "";
}
