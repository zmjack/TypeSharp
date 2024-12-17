namespace TypeSharp.AST;

public partial class UnionType : INode
{
    public SyntaxKind Kind => SyntaxKind.UnionType;

    public UnionType(params IType[] types)
    {
        Types = types;
    }

    public IType[] Types { get; set; }

    public string GetText(Indent indent = default)
    {
        return string.Join(" & ", from x in Types select x.GetText());
    }
}
