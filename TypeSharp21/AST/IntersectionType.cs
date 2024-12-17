namespace TypeSharp.AST;

public partial class IntersectionType : INode
{
    public SyntaxKind Kind => SyntaxKind.IntersectionType;

    public IntersectionType(params IType[] types)
    {
        Types = types;
    }

    public IType[] Types { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            {string.Join(" | ", from x in Types select x.GetText())};
            """;
    }
}
