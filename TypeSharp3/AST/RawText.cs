using System.Text;

namespace TypeSharp.AST;

public partial class RawText : INode
{
    public SyntaxKind Kind => SyntaxKind.Unknown;

    public RawText(string text)
    {
        Text = text;
    }

    public string Text { get; set; }

    public string GetText(Indent indent = default)
    {
        var builder = new StringBuilder();
        var enumerator = Text.ReplaceLineEndings().Split(Environment.NewLine).GetEnumerator()!;
        while (enumerator.MoveNext())
        {
            builder.AppendLine(enumerator.Current.ToString());
            while (enumerator.MoveNext())
            {
                builder.Append(indent.ToString());
                builder.AppendLine(enumerator.Current.ToString());
            }
        }

        builder.Length -= Environment.NewLine.Length;
        return builder.ToString();
    }
}
