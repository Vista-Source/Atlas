using CppAst;

namespace Atlas.Renderers;

internal class FieldRenderer : IGlueRenderer
{
    public string RenderCPP(CppCompilation compilation, FileInfo file)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        foreach (CppField field in compilation.Fields)
        {
            if (field.Comment?.ToString() != Options.ExportComment)
                continue;

            var type = ConversionUtility.NormalizeType(field.Type, TargetLanguage.Cpp);
            var fieldInfo = new FieldInfo
            {
                Name = field.Name,
                Type = type
            };
            fields.Add(fieldInfo);
        }

        var model = new Dictionary<string, object>
        {
            ["namespace"] = Options.Namespace,
            ["lib_name"] = Options.LibraryName,
            ["fields"] = fields
        };

        return TemplateEngine.RenderTemplate("cpp_field_wrapper", model);
    }

    public string RenderCSharp(CppCompilation compilation, FileInfo file)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        foreach (CppField field in compilation.Fields)
        {
            if (field.Comment?.ToString() != Options.ExportComment)
                continue;

            var type = ConversionUtility.NormalizeType(field.Type, TargetLanguage.CSharp);
            var fieldInfo = new FieldInfo
            {
                Name = field.Name,
                Type = type
            };
            fields.Add(fieldInfo);
        }

        var model = new Dictionary<string, object>
        {
            ["namespace"] = Options.Namespace,
            ["lib_name"] = Options.LibraryName,
            ["fields"] = fields
        };

        return TemplateEngine.RenderTemplate("cs_field_wrapper", model);
    }
}

internal class FieldInfo
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
}
