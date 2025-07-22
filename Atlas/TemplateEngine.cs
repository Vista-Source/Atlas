
using Scriban;

namespace Atlas;

/// <summary>
/// Text Template Engine.
/// </summary>
public static class TemplateEngine
{

    /// <summary>
    /// Renders a template.
    /// </summary>
    /// <param name="templateName">Template name inside of Resources folder.</param>
    /// <param name="model">Any required data.</param>
    /// <returns>Rendered template.</returns>
    public static string RenderTemplate(string templateName, object model)
    {
        string templatePath = Path.Combine(AppContext.BaseDirectory, "Resources", $"{templateName}.scriban");
        string templateText = File.ReadAllText(templatePath);

        var template = Template.Parse(templateText);

        // Convert model to dictionary (or create one)
        var dict = model switch
        {
            null => new Dictionary<string, object>(),
            IDictionary<string, object> d => new Dictionary<string, object>(d),
            _ => new Dictionary<string, object>()
        };

        // If model is a non-dictionary object, try to copy its public properties
        if (!(model is IDictionary<string, object>))
        {
            foreach (var prop in model.GetType().GetProperties())
            {
                if (!dict.ContainsKey(prop.Name))
                {
                    dict[prop.Name] = prop.GetValue(model);
                }
            }
        }

        // Add or override defaults
        dict["namespace"] = Options.Namespace;
        dict["lib_name"] = Options.LibraryName;

        return template.Render(dict);
    }
}