using Atlas.Types;

namespace Atlas;

/// <summary>
/// Managed-Native Interop Gen.
/// </summary>
public static class Interop
{
    /// <summary> Generate's Managed C# files from a C++ header file. </summary>
    public static void GenerateManaged(FileInfo header)
    {
        var headerContents = File.ReadAllText(header.FullName);
        GenerateManaged(headerContents);
    }

    /// <summary> Generate's Managed C# files from a C++ header. </summary>
    public static void GenerateManaged(string headerContents)
    {
        TypeMapRegistry.RegisterTypeMaps();
    }
}
