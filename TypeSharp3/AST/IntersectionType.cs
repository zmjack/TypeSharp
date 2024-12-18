namespace TypeSharp.AST;

public partial class IntersectionType : INode
{
    public SyntaxKind Kind => SyntaxKind.IntersectionType;

    public IntersectionType(params IGeneralType[] types)
    {
        Types = types;
    }

    public IGeneralType[] Types { get; set; }

    public string GetText(Indent indent = default)
    {
        return string.Join(" & ", from x in Types select x.GetText());
    }
}
