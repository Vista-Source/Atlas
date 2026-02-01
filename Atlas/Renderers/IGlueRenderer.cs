using CppAst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Renderers;

/// <summary>
/// Renderer responsible for generating the Glue code.
/// </summary>
public interface IGlueRenderer
{
    /// <summary>
    /// Renders the Glue code for C++.
    /// </summary>
    /// <returns>C++ Header File.</returns>
    public string RenderCPP(CppCompilation compilation, FileInfo file);

    /// <summary>
    /// Renders the Glue code for C#.
    /// </summary>
    /// <returns>C# Source File.</returns>
    public string RenderCSharp(CppCompilation compilation, FileInfo file);
}
