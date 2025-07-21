using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Atlas;

namespace Atlas.CLI
{
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
                    RunOptionsAndReturnExitCode,
                    _ => 1);
        }

        static int RunOptionsAndReturnExitCode(CLIOptions opts)
        {
#if DEBUG
            if (opts.Target == null)
            {
                var testHeader = new FileInfo("Resources/test.h");
                var glue = Atlas.GenerateGlue(testHeader);

                Console.WriteLine(Atlas.GenerateMasterCPP(new() { "test.h" }));
                Console.WriteLine(glue.CPP);
                Console.WriteLine(glue.CS);
                return 0;
            }
#endif
            Options.Namespace = opts.Namespace;
            Options.LibraryName = opts.LibraryName;

            if (Directory.Exists(opts.Target))
            {
                return GenerateGlueForDirectory(opts.Target);
            }

            return GenerateGlueForFile(new FileInfo(opts.Target));
        }

        static int GenerateGlueForDirectory(string targetDir)
        {
            var headers = Directory.GetFiles(targetDir, "*.h", SearchOption.AllDirectories);
            var validHeaders = new List<string>();
            var outputDir = Path.GetDirectoryName(Path.GetFullPath(targetDir)) ?? ".";

            foreach (var header in headers)
            {
                var glue = Atlas.GenerateGlue(new FileInfo(header));
                if (string.IsNullOrEmpty(glue.CPP) || string.IsNullOrEmpty(glue.CS))
                    continue;

                validHeaders.Add(header);
                string baseName = Path.GetFileNameWithoutExtension(header);

                File.WriteAllText(Path.Combine(outputDir, $"{baseName}.{Options.FilePrefix}.h"), glue.CPP);
                File.WriteAllText(Path.Combine(outputDir, $"{baseName}.{Options.FilePrefix}.cs"), glue.CS);
            }

            var relativeHeaders = validHeaders
                .Select(header =>
                    Path.GetRelativePath(outputDir, header).Replace('\\', '/'))
                .ToList();

            string masterCpp = Atlas.GenerateMasterCPP(relativeHeaders);
            File.WriteAllText(Path.Combine(outputDir, "Atlas.cpp"), masterCpp);

            return 0;
        }

        static int GenerateGlueForFile(FileInfo headerFile)
        {
            var glue = Atlas.GenerateGlue(headerFile);
            string baseName = Path.GetFileNameWithoutExtension(headerFile.FullName);

            File.WriteAllText($"{baseName}.{Options.FilePrefix}.h", glue.CPP);
            File.WriteAllText($"{baseName}.{Options.FilePrefix}.cs", glue.CS);

            string masterCpp = Atlas.GenerateMasterCPP(new() { Path.GetFileName(headerFile.FullName) });
            File.WriteAllText("Atlas.cpp", masterCpp);

            return 0;
        }
    }
}
