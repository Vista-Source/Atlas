
using Atlas.Writers;

namespace Atlas.Types;

[TypeMap("int")]
internal class IntTypeMap : TypeMap
{
    /// <inheritdoc/>
    public override void NativeToManaged(BaseWriter writer) => writer.WriteLine("int");
}
