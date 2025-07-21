using Atlas.Extensions;
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
                    Console.WriteLine($"Generated C# file: {file.FullName}");
                }
            }
        };
    }
}
