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

            foreach (var header in headers)
            {
                var glue = Atlas.GenerateGlue(new FileInfo(header));
                if (string.IsNullOrEmpty(glue.CPP) || string.IsNullOrEmpty(glue.CS))
                    continue;

                validHeaders.Add(header);

                string baseName = Path.GetFileNameWithoutExtension(header);
                string headerDir = Path.GetDirectoryName(header) ?? ".";

                File.WriteAllText(Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.h"), glue.CPP);
                File.WriteAllText(Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.cs"), glue.CS);
            }

            // Generate Atlas.cpp at the root of the input dir
            var relativeHeaders = validHeaders
                .Select(header =>
                    Path.GetRelativePath(targetDir, header).Replace('\\', '/'))
                .ToList();

            string masterCpp = Atlas.GenerateMasterCPP(relativeHeaders);
            File.WriteAllText(Path.Combine(targetDir, "Atlas.cpp"), masterCpp);

            return 0;
        }

        static int GenerateGlueForFile(FileInfo headerFile)
        {
            var glue = Atlas.GenerateGlue(headerFile);
            string baseName = Path.GetFileNameWithoutExtension(headerFile.FullName);
            string headerDir = Path.GetDirectoryName(headerFile.FullName) ?? ".";

            File.WriteAllText(Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.h"), glue.CPP);
            File.WriteAllText(Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.cs"), glue.CS);

            string masterCpp = Atlas.GenerateMasterCPP(new() { Path.GetFileName(headerFile.FullName) });
            File.WriteAllText(Path.Combine(headerDir, "Atlas.cpp"), masterCpp);

            return 0;
        }
    }
}
