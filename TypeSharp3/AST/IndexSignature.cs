namespace TypeSharp.AST;

public partial class IndexSignature : INode
{
    public SyntaxKind Kind => SyntaxKind.IndexSignature;

    public IndexSignature(Parameter parameter, IGeneralType type)
    {
        Parameter = parameter;
        Type = type;
    }

    public Parameter Parameter { get; set; }
    public IGeneralType Type { get; set; }

    public string GetText(Indent indent = default)
    {
        return $"[{Parameter.GetText()}]: {Type.GetText()}";
    }
}
