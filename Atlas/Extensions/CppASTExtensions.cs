using CppAst;

namespace Atlas.Extensions;

public static class CppAstExtensions
{
    public static string ExtractBody(this CppFunction function, string[] fileLines)
    {
        int startLine = function.Span.Start.Line - 1; // Convert to 0-based
        int braceCount = 0;
        bool started = false;
        var bodyLines = new List<string>();

        for (int i = startLine; i < fileLines.Length; i++)
        {
            string line = fileLines[i];

            if (!started)
            {
                if (line.Contains("{"))
                {
                    started = true;
                    braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');
                    bodyLines.Add(line);
                }
            }
            else
            {
                bodyLines.Add(line);
                braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');

                if (braceCount <= 0)
                    break;
            }
        }

        return string.Join("\n", bodyLines);
    }
}
