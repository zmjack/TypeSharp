namespace TypeSharp21.AST;

public partial class LiteralType : INode
{
    public SyntaxKind Kind => SyntaxKind.LiteralType;

    public LiteralType(ILiteral literal)
    {
        Literal = literal;
    }

    public ILiteral Literal { get; set; }

    public string GetText()
    {
        return Literal.GetText();
    }
}
