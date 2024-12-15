namespace TypeSharp21.AST;

public partial class IntersectionType : INode
{
    public SyntaxKind Kind => SyntaxKind.IntersectionType;

    public IntersectionType(params IType[] types)
    {
        Types = types;
    }

    public IType[] Types { get; set; }

    public string GetText()
    {
        return string.Join(" | ", from x in Types select x.GetText());
    }
}
