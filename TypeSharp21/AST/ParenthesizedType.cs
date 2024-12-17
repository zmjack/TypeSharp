namespace TypeSharp.AST;

public partial class ParenthesizedType : INode
{
    public SyntaxKind Kind => SyntaxKind.ParenthesizedType;

    public ParenthesizedType(IGeneralType type)
    {
        Type = type;
    }

    public IGeneralType Type { get; set; }

    public string GetText(Indent indent = default)
    {
        return $"({Type.GetText()})";
    }
}
