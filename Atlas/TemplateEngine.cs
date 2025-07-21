
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
        return template.Render(model);
    }
}