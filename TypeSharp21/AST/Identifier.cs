namespace TypeSharp21.AST;

public class Identifier : INode
{
    public SyntaxKind Kind => SyntaxKind.Identifier;

    public Identifier(string name)
    {
        EscapedText = name;
    }

    public string EscapedText { get; }

    public string GetText()
    {
        return EscapedText;
    }

    public static implicit operator Identifier(string name)
    {
        return new(name);
    }
}
