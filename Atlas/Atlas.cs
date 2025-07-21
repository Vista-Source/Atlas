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
}
