using Atlas;

namespace AtlasGrapher;

internal class Program
{
    const string CPPPath = "D:\\Vista\\Atlas\\Atlas\\demo.h";

    static void Main(string[] args)
    {
        Output output = Atlas.Atlas.GenerateGlue(new FileInfo(CPPPath));
        Console.WriteLine(output.CPP);
        Console.WriteLine(output.CS);
    }
}
