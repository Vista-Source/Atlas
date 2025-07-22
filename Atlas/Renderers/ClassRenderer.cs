using CppAst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Atlas.Renderers;

internal class ClassRenderer : IGlueRenderer
{
    public string RenderCPP(CppCompilation compilation, FileInfo file)
    {
        var model = new Dictionary<string, object>
        {
            ["classes"] = ExtractClasses(compilation, TargetLanguage.Cpp)
        };

        return TemplateEngine.RenderTemplate("cpp_class_wrapper", model);
    }

    public string RenderCSharp(CppCompilation compilation, FileInfo file)
    {
        var model = new Dictionary<string, object>
        {
            ["classes"] = ExtractClasses(compilation, TargetLanguage.CSharp)
        };

        return TemplateEngine.RenderTemplate("cs_class_wrapper", model);
    }

    private List<ClassInfo> ExtractClasses(CppCompilation compilation, TargetLanguage target)
    {
        var classes = new List<ClassInfo>();

        foreach (var @class in compilation.Classes)
        {
            if (@class.Comment?.ToString() != Options.ExportComment)
                continue;

            var classInfo = new ClassInfo
            {
                Name = @class.Name
            };

            foreach (var constructor in @class.Constructors)
            {
                var parameters = constructor.Parameters
                    .Select(p => new
                    {
                        Type = ConversionUtility.NormalizeType(p.Type, target),
                        Name = p.Name
                    })
                    .ToList();

                var constructorInfo = new Constructor
                {
                    Parameters = string.Join(", ", parameters.Select(p => $"{p.Type} {p.Name}")),
                    TypelessParameters = string.Join(", ", parameters.Select(p => p.Name))
                };

                classInfo.Constructors.Add(constructorInfo);
            }

            foreach (var field in @class.Fields)
            {
                classInfo.Fields.Add(new FieldInfo
                {
                    Name = field.Name,
                    Type = ConversionUtility.NormalizeType(field.Type, target)
                });
            }

            foreach (var method in @class.Functions)
            {
                var typelessParameters = string.Join(", ",
    method.Parameters.Select(p => $"{p.Name}"));

                classInfo.Methods.Add(new MethodInfo
                {
                    Name = method.Name,
                    Parameters = string.Join(", ",
                        method.Parameters.Select(p => $"{ConversionUtility.NormalizeType(p.Type, target)} {p.Name}")),
                    ReturnType = ConversionUtility.NormalizeType(method.ReturnType, target),
                    Body = $"{method.Name}({typelessParameters});"

                });
            }

            classes.Add(classInfo);
        }

        return classes;
    }
}

public class ClassInfo
{
    public string Name { get; set; } = "";
    public List<Constructor> Constructors { get; set; } = new List<Constructor>();
    public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();
    public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();
}

public class Constructor
{
    public string Parameters { get; set; } = "";
    public string TypelessParameters { get; set; } = "";
}