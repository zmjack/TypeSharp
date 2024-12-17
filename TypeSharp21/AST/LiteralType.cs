namespace TypeSharp.AST;

public partial class LiteralType : INode
{
    public SyntaxKind Kind => SyntaxKind.LiteralType;

    public LiteralType(ILiteral literal)
    {
        Literal = literal;
    }

    public ILiteral Literal { get; set; }

    public string GetText(Indent indent = default)
    {
        return Literal.GetText();
    }
}
