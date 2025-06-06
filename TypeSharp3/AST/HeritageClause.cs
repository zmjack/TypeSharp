namespace TypeSharp.AST;

public partial class HeritageClause : INode
{
    public SyntaxKind Kind => SyntaxKind.HeritageClause;

    public HeritageClause(IToken token, ExpressionWithTypeArguments[] types)
    {
        Token = token;
        Types = types;
    }

    public IToken Token { get; set; }
    public ExpressionWithTypeArguments[] Types { get; set; }

    public string GetText(Indent indent = default)
    {
        return $"{Token.GetText()} {string.Join(",", from type in Types select type.GetText())}";
    }
}
