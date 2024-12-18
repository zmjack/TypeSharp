using System.Text.Encodings.Web;

namespace TypeSharp.AST;

public partial class StringLiteral : INode
{
    public SyntaxKind Kind => SyntaxKind.StringLiteral;

    public StringLiteral(string value)
    {
        Text = JavaScriptEncoder.UnsafeRelaxedJsonEscaping.Encode(value);
    }

    public string? Text { get; set; }

    public string GetText(Indent indent = default)
    {
        return $"\"{Text}\"";
    }
}
