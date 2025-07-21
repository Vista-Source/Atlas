using CppAst;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.Extensions
{
    public static class CppAstExtensions
    {
        public static string ExtractBody(this CppFunction function, string[] fileLines)
        {
            int startLine = function.Span.Start.Line - 1; // 0-based
            int endLine = function.Span.End.Line;

            // Find the line where the function body starts (the line containing the first '{')
            int bodyStartLine = -1;
            for (int i = startLine; i <= endLine && i < fileLines.Length; i++)
            {
                if (fileLines[i].Contains("{"))
                {
                    bodyStartLine = i;
                    break;
                }
            }

            if (bodyStartLine == -1)
                return ""; // No body found

            int braceCount = 0;
            var bodyLines = new List<string>();

            // From bodyStartLine, collect lines until braces are balanced
            for (int i = bodyStartLine; i < fileLines.Length; i++)
            {
                string line = fileLines[i];
                bodyLines.Add(line);

                // Count '{' and '}' occurrences in the line
                braceCount += line.Count(c => c == '{');
                braceCount -= line.Count(c => c == '}');

                // Once balanced (braceCount == 0), stop collecting
                if (braceCount == 0)
                    break;
            }

            // Combine collected lines and extract the inner body between braces
            string body = ExtractInnerBody(string.Join("\n", bodyLines));
            if (body.StartsWith("\n"))
                body = body.Substring(1);

            return body;
        }

        public static string ExtractInnerBody(string fullFunction)
        {
            int startBrace = fullFunction.IndexOf('{');
            int endBrace = fullFunction.LastIndexOf('}');

            if (startBrace == -1 || endBrace == -1 || endBrace <= startBrace)
                return "";

            // Extract content inside the outermost braces
            var body = fullFunction.Substring(startBrace + 1, endBrace - startBrace - 1);

            return body;
        }
    }
}
