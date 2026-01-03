
namespace Atlas.Writers;

public abstract class BaseWriter
{
    /// <summary> The current written content. </summary>
    public string Content => writer.ToString();

    private StringWriter writer = new StringWriter();

    /// <summary> Writes a line to <see cref="Content"/>. </summary>
    public virtual void WriteLine(string text) => writer.WriteLine(text);

    /// <summary> Writes text to <see cref="Content"/>. </summary>
    public virtual void Write(string text) => writer.Write(text);
}
