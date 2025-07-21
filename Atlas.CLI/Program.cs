using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    [Option("extensions", Separator = ',', HelpText = "Comma-separated list of extension libraries to load.")]
    public IEnumerable<string> Extensions { get; set; }
}

public class Program
{
    /// <summary>
    /// Action to be called when files are written.
    /// </summary>
    public static Action<List<FileInfo>> OnWriteFiles { get; set; } = delegate { };

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

        foreach (var extension in opts.Extensions ?? Enumerable.Empty<string>())
        {
            if (!string.IsNullOrEmpty(extension))
            {
                try
                {
                    Extensions.ExtensionManager.LoadExtension(extension);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to load extension '{extension}': {ex.Message}");
                    return 1;
                }
            }
        }

        if (Directory.Exists(opts.Target))
        {
            return GenerateGlueForDirectory(opts.Target);
        }

        return GenerateGlueForFile(new FileInfo(opts.Target));
    }

    static int GenerateGlueForDirectory(string targetDir)
    {
        var writtenFiles = new List<FileInfo>();
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

            var cppPath = Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.h");
            var csPath = Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.cs");

            File.WriteAllText(cppPath, glue.CPP);
            File.WriteAllText(csPath, glue.CS);

            writtenFiles.Add(new FileInfo(cppPath));
            writtenFiles.Add(new FileInfo(csPath));
        }

        var relativeHeaders = validHeaders
            .Select(header =>
                Path.GetRelativePath(targetDir, header).Replace('\\', '/'))
            .ToList();

        string masterCpp = Atlas.GenerateMasterCPP(relativeHeaders);
        var masterPath = Path.Combine(targetDir, "Atlas.cpp");
        File.WriteAllText(masterPath, masterCpp);
        writtenFiles.Add(new FileInfo(masterPath));

        OnWriteFiles(writtenFiles);
        return 0;
    }

    static int GenerateGlueForFile(FileInfo headerFile)
    {
        var writtenFiles = new List<FileInfo>();

        var glue = Atlas.GenerateGlue(headerFile);
        string baseName = Path.GetFileNameWithoutExtension(headerFile.FullName);
        string headerDir = Path.GetDirectoryName(headerFile.FullName) ?? ".";

        var cppPath = Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.h");
        var csPath = Path.Combine(headerDir, $"{baseName}.{Options.FilePrefix}.cs");
        var masterPath = Path.Combine(headerDir, "Atlas.cpp");

        File.WriteAllText(cppPath, glue.CPP);
        File.WriteAllText(csPath, glue.CS);
        File.WriteAllText(masterPath, Atlas.GenerateMasterCPP(new() { Path.GetFileName(headerFile.FullName) }));

        writtenFiles.Add(new FileInfo(cppPath));
        writtenFiles.Add(new FileInfo(csPath));
        writtenFiles.Add(new FileInfo(masterPath));

        OnWriteFiles(writtenFiles);
        return 0;
    }
}
