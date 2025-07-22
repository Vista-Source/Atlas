
using CppAst;

namespace Atlas.Renderers;

/// <summary>
/// Renderer responsible for generating top-level function glue code.
/// </summary>
internal class FunctionRenderer : IGlueRenderer
{
    public string RenderCPP(CppCompilation compilation, FileInfo file)
    {
        var methods = ExtractMethods(compilation, includeBody: true);
        var model = new Dictionary<string, object>
        {
            ["methods"] = methods
        };

        return TemplateEngine.RenderTemplate("extern_c_wrapper", model);
    }

    public string RenderCSharp(CppCompilation compilation, FileInfo file)
    {
        var methods = ExtractMethods(compilation, includeBody: false);
        var model = new Dictionary<string, object>
        {
            ["namespace"] = Options.Namespace,
            ["lib_name"] = Options.LibraryName,
            ["methods"] = methods
        };

        return TemplateEngine.RenderTemplate("pinvoke_wrapper", model);
    }

    /// <summary>
    /// Extracts exported methods from a C++ file.
    /// </summary>
    private static List<MethodInfo> ExtractMethods(CppCompilation compilation, bool includeBody)
    {
        var methods = new List<MethodInfo>();

        var options = new CppParserOptions
        {
            ParseSystemIncludes = false,
            SystemIncludeFolders = { }
        };

        foreach (var message in compilation.Diagnostics.Messages)
            Console.WriteLine(message);

        foreach (var function in compilation.Functions)
        {
            if (function.Comment.ToString() != Options.ExportComment)
                continue;

            var parameters = string.Join(", ",
                function.Parameters.Select(p => $"{NormalizeType(p.Type)} {p.Name}"));

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

    private static string NormalizeType(CppType type)
    {
        // If the type is an enum/struct/class, extract just the name
        if (type is CppEnum enumType)
            return enumType.Name;

        if (type is CppTypedef typedef)
            return typedef.Name;

        return type.ToString();
    }
}

public class MethodInfo
{
    public string Name { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string Parameters { get; set; } = "";
    public string Body { get; set; } = "";
}
