
namespace Atlas;

public static class Atlas
{
    /// <summary>
    /// Generate Glue from C++.
    /// </summary>
    /// <param name="cpp">C++ Header File.</param>
    public static Output GenerateGlue(FileInfo cpp)
    {
        Output output = new Output();

        output.CS = Glue.GenerateCS(cpp);
        output.CPP = Glue.GenerateCPP(cpp);

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
