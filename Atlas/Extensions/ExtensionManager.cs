
namespace Atlas.Extensions;

public static class ExtensionManager
{
    public static void LoadExtension(string extensionName)
    {
        if (string.IsNullOrEmpty(extensionName))
            throw new ArgumentException("Extension name cannot be null or empty.", nameof(extensionName));

        // Append assembly extension
        if (!extensionName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            extensionName += ".dll";

        // Load assembly
        string fullPath = Path.Combine(AppContext.BaseDirectory, "Extensions", extensionName);

        var assembly = System.Reflection.Assembly.LoadFrom(fullPath);
        var extensionType = assembly.GetTypes().FirstOrDefault(t => typeof(IExtension).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        if (extensionType == null)
            throw new InvalidOperationException($"No valid extension type found in {extensionName}.");

        var extensionInstance = (IExtension)Activator.CreateInstance(extensionType);
        extensionInstance.OnLoad();
    }
}
