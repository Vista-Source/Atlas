
namespace Atlas;

public static class Atlas
{
    /// <summary>
    /// Called when the Glue is first requested to be generated.
    /// </summary>
    public static Action<FileInfo> OnPreGenerateGlue { get; set; } = delegate { };

    /// <summary>
    /// Called when the Glue is generated.
    /// </summary>
    public static Action<Output> OnPostGenerateGlue { get; set; } = delegate { };

    /// <summary>
    /// Generate Glue from C++.
    /// </summary>
    /// <param name="cpp">C++ Header File.</param>
    public static Output GenerateGlue(FileInfo cpp)
    {
        // Check if this C++ file has any instance of the ExportComment
        if (!File.ReadAllText(cpp.FullName).Contains(Options.ExportComment))
            return default;

        OnPreGenerateGlue?.Invoke(cpp);

        Output output = new Output();

        output.CS = Glue.GenerateCS(cpp);
        output.CPP = Glue.GenerateCPP(cpp);

        OnPostGenerateGlue?.Invoke(output);

        // Generate the Glue.
        return output;
    }

    /// <summary>
    /// Generates the master C++ file that compiles all the generated headers.
    /// </summary>
    /// <remarks>If this master file isn't in the project, headers wont compile into the program.</remarks>
    /// <param name="headers">Names of the generated headers.</param>
    public static string GenerateMasterCPP(List<string> headers) => Glue.GenerateMasterCPP(headers);
}
