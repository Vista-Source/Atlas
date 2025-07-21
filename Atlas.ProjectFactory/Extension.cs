using Atlas.Extensions;

namespace Atlas.ProjectFactory;

public class Extension : IExtension
{
    public void OnLoad()
    {
        Console.WriteLine("ProjectFactory extension loaded.");
    }
}
