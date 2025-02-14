namespace TypeSharp.AST;

public partial class UnionType : INode
{
    public SyntaxKind Kind => SyntaxKind.UnionType;

    public UnionType(IGeneralType[] types)
    {
        Types = types;
    }

    public IGeneralType[] Types { get; set; }

    public string GetText(Indent indent = default)
    {
        return string.Join(" | ", from x in Types select x.GetText());
    }

    public UnionType WithoutUndefined()
    {
        return new UnionType([
            .. from x in Types where x != UndefinedKeyword.Default select x
        ]);
    }
}
