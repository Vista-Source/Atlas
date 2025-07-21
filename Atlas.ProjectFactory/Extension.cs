using Atlas.Extensions;
using System.Xml.Linq;
using Atlas.CLI;

namespace Atlas.ProjectFactory;

public class Extension : IExtension
{
    public void OnLoad()
    {
        Program.OnWriteFiles += files =>
        {
            foreach (var file in files)
            {
                if (file.Name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    // Move file to the "Generated" directory
                    var generatedDir = Path.Combine(file.DirectoryName, "Generated");
                    if (!Directory.Exists(generatedDir))
                        Directory.CreateDirectory(generatedDir);

                    var newFilePath = Path.Combine(generatedDir, file.Name);

                    // Move the file
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);

                    File.Move(file.FullName, newFilePath);
                }
            }

            // Create a .csproj file
            var project = new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Library"),
                    new XElement("TargetFramework", "net9.0")
                )
            );

            // Write the project file to the "Generated" directory
            var projectFilePath = Path.Combine(files.First().DirectoryName, "Generated", $"{Options.Namespace}.csproj");
            if (File.Exists(projectFilePath))
                File.Delete(projectFilePath);

            using (var writer = new StreamWriter(projectFilePath))
                project.Save(writer);
        };
    }
}
