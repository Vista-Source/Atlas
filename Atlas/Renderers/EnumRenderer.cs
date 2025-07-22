using CppAst;

namespace Atlas.Renderers;

internal class EnumRenderer : IGlueRenderer
{
    public string RenderCPP(CppCompilation compilation, FileInfo file)
    {
        // Interop can just handle enums as ints, no C++ stuff is needed
        return string.Empty;
    }

    public string RenderCSharp(CppCompilation compilation, FileInfo file)
    {
        var enums = ExtractEnums(compilation);
        var model = new Dictionary<string, object>
        {
            ["namespace"] = Options.Namespace,
            ["enums"] = enums
        };

        return TemplateEngine.RenderTemplate("cs_enum_wrapper", model);
    }

    private static List<EnumInfo> ExtractEnums(CppCompilation compilation)
    {
        var result = new List<EnumInfo>();

        foreach (var @enum in compilation.Enums)
        {
            if (@enum.Comment?.ToString() != Options.ExportComment)
                continue;

            var enumValues = @enum.Items
                .Select(item => new EnumValueInfo
                {
                    Name = item.Name,
                    Value = item.Value.ToString() ?? ""
                })
                .ToList();

            result.Add(new EnumInfo
            {
                Name = @enum.Name,
                IsScoped = @enum.IsScoped,
                UnderlyingType = @enum.IntegerType.ToString(),
                Values = enumValues
            });
        }

        return result;
    }
}

internal class EnumInfo
{
    public string Name { get; set; } = "";
    public bool IsScoped { get; set; }
    public string UnderlyingType { get; set; } = "int";
    public List<EnumValueInfo> Values { get; set; } = new();
}

internal class EnumValueInfo
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
}