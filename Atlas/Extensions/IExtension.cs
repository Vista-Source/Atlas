using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Extensions;

/// <summary>
/// Atlas extension interface.
/// </summary>
public interface IExtension
{
    /// <summary>
    /// Called when the extension is loaded.
    /// </summary>
    public void OnLoad();
}
