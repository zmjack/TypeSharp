namespace TypeSharp.AST;

public partial class Identifier : INode
{
    public SyntaxKind Kind => SyntaxKind.Identifier;

    public Identifier(string name)
    {
        EscapedText = name;
    }

    public string EscapedText { get; }

    public string GetText(Indent indent = default)
    {
        return EscapedText;
    }

    public static implicit operator Identifier(string name)
    {
        return new(name);
    }
}
