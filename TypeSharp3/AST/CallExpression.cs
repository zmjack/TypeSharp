namespace TypeSharp.AST;

public partial class CallExpression : INode
{
    public SyntaxKind Kind => SyntaxKind.CallExpression;

    public string GetText(Indent indent = default)
    {
        throw new NotImplementedException();
    }
}
