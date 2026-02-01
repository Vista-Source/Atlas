using Atlas;

namespace Atlas.Tests;

public class TemplateTests
{
    [Fact]
    public void RenderTemplate_WithValidModel_RendersExpectedOutput()
    {
        var now = new DateTime(2025, 07, 21, 18, 20, 50);
        var model = new
        {
            name = "Alice",
            date = now.ToString("dd/MM/yyyy hh:mm:ss tt"),
            is_active = true
        };

        var output = TemplateEngine.RenderTemplate("test_template", model);

        Assert.Contains("Hello Alice!", output);
        Assert.Contains($"Today is {model.date}.", output);
        Assert.Contains("Status: Active", output);
    }
}
