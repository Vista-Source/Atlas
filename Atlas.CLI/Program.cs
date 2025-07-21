using CommandLine;
using Atlas;

namespace Atlas.CLI;

class CLIOptions
{
    [Option('n', "namespace", Required = false, HelpText = "Default C# Namespace for generated code.")]
    public string Namespace { get; set; }

    [Option('l', "libraryname", Required = true, HelpText = "Name of the C++ library that will hold the exported functions.")]
    public string LibraryName { get; set; }

    [Option('t', "targetheader", Required = true, HelpText = "Target header file to generate glue for.")]
    public string TargetHeader { get; set; }
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
        // Setup Atlas Options
        Options.Namespace = opts.Namespace;
        Options.LibraryName = opts.LibraryName;

        // Generate the glue
        FileInfo targetHeader = new FileInfo(opts.TargetHeader);
        Output glue = Atlas.GenerateGlue(targetHeader);

        // Write glue to files
        File.WriteAllText(Path.GetFileNameWithoutExtension(targetHeader.FullName) + ".g.h", glue.CPP);
        File.WriteAllText(Path.GetFileNameWithoutExtension(targetHeader.FullName) + ".g.cs", glue.CS);

        return 0;
    }
}