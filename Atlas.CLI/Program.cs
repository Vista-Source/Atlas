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

            string masterFileDebug = Atlas.GenerateMasterCPP(new() { "test.h" });
            Console.WriteLine(masterFileDebug);

            Console.WriteLine(glueDebug.CPP);
            Console.WriteLine(glueDebug.CS);

            return 0;
        }
#endif
        // Setup Atlas Options
        Options.Namespace = opts.Namespace;
        Options.LibraryName = opts.LibraryName;

        // Check if target is directory
        if (Directory.Exists(opts.Target))
        {
            // Gather all .h headers in the target directory recursively
            string[] headerPaths = Directory.GetFiles(opts.Target, "*.h", SearchOption.AllDirectories);
            var validHeaders = new List<string>();

            foreach (string header in headerPaths)
            {
                var headerGlue = Atlas.GenerateGlue(new FileInfo(header));
                if (string.IsNullOrEmpty(headerGlue.CPP) || string.IsNullOrEmpty(headerGlue.CS))
                    continue;

                validHeaders.Add(header);

                string baseName = Path.GetFileNameWithoutExtension(header);
                string outputDir = Path.GetDirectoryName(Path.GetFullPath(opts.Target)) ?? ".";

                File.WriteAllText(Path.Combine(outputDir, $"{baseName}.{Options.FilePrefix}.h"), headerGlue.CPP);
                File.WriteAllText(Path.Combine(outputDir, $"{baseName}.{Options.FilePrefix}.cs"), headerGlue.CS);
            }

            // Use only the valid headers for master CPP
            var relativeHeaders = validHeaders
                .Select(full => Path.GetRelativePath(Path.GetDirectoryName(Path.GetFullPath(opts.Target)) ?? ".", full).Replace('\\', '/'))
                .ToList();

            string masterCpp = Atlas.GenerateMasterCPP(relativeHeaders);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(opts.Target)) ?? ".", "Atlas.cpp"), masterCpp);

            return 0;
        }

        // Generate the glue
        FileInfo targetHeader = new FileInfo(opts.Target);
        Output glue = Atlas.GenerateGlue(targetHeader);

        // Write glue to files
        File.WriteAllText(Path.GetFileNameWithoutExtension(targetHeader.FullName) + $".{Options.FilePrefix}.h", glue.CPP);
        File.WriteAllText(Path.GetFileNameWithoutExtension(targetHeader.FullName) + $".{Options.FilePrefix}.cs", glue.CS);

        // Write master file
        string masterFile = Atlas.GenerateMasterCPP(new() { Path.GetFileName(targetHeader.FullName) });
        File.WriteAllText("Atlas.cpp", masterFile);

        return 0;
    }
}