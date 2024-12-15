namespace TypeSharp21.AST;

public partial class ParenthesizedType : INode
{
    public SyntaxKind Kind => SyntaxKind.ParenthesizedType;

    public ParenthesizedType(IGeneralType type)
    {
        Type = type;
    }

    public IGeneralType Type { get; set; }

    public string GetText()
    {
        return $"({Type.GetText()})";
    }
}
