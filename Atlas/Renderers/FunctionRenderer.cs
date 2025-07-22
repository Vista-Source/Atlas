
using CppAst;

namespace Atlas.Renderers;

/// <summary>
/// Renderer responsible for generating top-level function glue code.
/// </summary>
internal class FunctionRenderer : IGlueRenderer
{
    public string RenderCPP(CppCompilation compilation, FileInfo file)
    {
        var methods = ExtractMethods(compilation, includeBody: true, target: TargetLanguage.Cpp);
        var model = new Dictionary<string, object>
        {
            ["methods"] = methods
        };

        return TemplateEngine.RenderTemplate("cpp_function_wrapper", model);
    }

    public string RenderCSharp(CppCompilation compilation, FileInfo file)
    {
        var methods = ExtractMethods(compilation, includeBody: false, target: TargetLanguage.CSharp);
        var model = new Dictionary<string, object>
        {
            ["namespace"] = Options.Namespace,
            ["lib_name"] = Options.LibraryName,
            ["methods"] = methods
        };

        return TemplateEngine.RenderTemplate("cs_function_wrapper", model);
    }

    /// <summary>
    /// Extracts exported methods from a C++ file.
    /// </summary>
    private static List<MethodInfo> ExtractMethods(CppCompilation compilation, bool includeBody, TargetLanguage target)
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
                function.Parameters.Select(p => $"{ConversionUtility.NormalizeType(p.Type, target)} {p.Name}"));

            var typelessParameters = string.Join(", ",
                function.Parameters.Select(p => $"{p.Name}"));

            var method = new MethodInfo
            {
                Name = function.Name,
                ReturnType = ConversionUtility.NormalizeType(function.ReturnType, target),
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
