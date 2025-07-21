using CommandLine;
using Atlas;

namespace Atlas.CLI;

class CLIOptions
{
    [Option('n', "namespace", Required = false, HelpText = "Default C# Namespace for generated code.")]
    public string Namespace { get; set; }

    [Option('l', "libraryname", Required = false, HelpText = "Name of the C++ library that will hold the exported functions.")]
    public string LibraryName { get; set; }

    [Option('t', "target", Required = false, HelpText = "Target header or directory to generate glue for.")]
    public string Target { get; set; }
}

class Program
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<CLIOptions>(args)
            .MapResult(
                (CLIOptions opts) => RunOptionsAndReturnExitCode(opts),
                errs => 1);
    }

    static int RunOptionsAndReturnExitCode(CLIOptions opts)
    {
#if DEBUG
        // Debugging: Log the outputted glue
        if (opts.Target == null)
        {
            FileInfo targetHeaderDebug = new FileInfo("Resources/test.h");
            Output glueDebug = Atlas.GenerateGlue(targetHeaderDebug);

            string masterFile = Atlas.GenerateMasterCPP(new() { "test.h" });
            Console.WriteLine(masterFile);

            Console.WriteLine(glueDebug.CPP);
            Console.WriteLine(glueDebug.CS);

            return 0;
        }
#endif
        // Setup Atlas Options
        Options.Namespace = opts.Namespace;
        Options.LibraryName = opts.LibraryName;

        // Generate the glue
        FileInfo targetHeader = new FileInfo(opts.Target);
        Output glue = Atlas.GenerateGlue(targetHeader);

        // Write glue to files
        File.WriteAllText(Path.GetFileNameWithoutExtension(targetHeader.FullName) + ".g.h", glue.CPP);
        File.WriteAllText(Path.GetFileNameWithoutExtension(targetHeader.FullName) + ".g.cs", glue.CS);

        return 0;
    }
}