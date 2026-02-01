
namespace Atlas;

/// <summary>
/// Definitions for C++ Atlas Syntax.
/// </summary>
public static class Options
{
    /// <summary>
    /// Comment that defines a C# export.
    /// </summary>
    public static string ExportComment = "[CSharpAPI]";

    /// <summary>
    /// File prefix for generated files.
    /// </summary>
    public static string FilePrefix = "gen";

    /// <summary>
    /// Default C# Namespace for generated code.
    /// </summary>
    public static string Namespace = "Atlas";

    /// <summary>
    /// Name of the C++ library that holds the exported functions.
    /// </summary>
    public static string LibraryName = "Atlas";
}
