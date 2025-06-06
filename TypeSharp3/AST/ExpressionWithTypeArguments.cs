namespace TypeSharp.AST;

public class ExpressionWithTypeArguments : INode
{
    public SyntaxKind Kind => SyntaxKind.ExpressionWithTypeArguments;

    public ExpressionWithTypeArguments(IIdentifier expression)
    {
        Expression = expression;
        TypeArguments = [];
    }
    public ExpressionWithTypeArguments(IIdentifier expression, IGeneralType[] typeArguments)
    {
        Expression = expression;
        TypeArguments = typeArguments;
    }

    public IIdentifier Expression { get; set; }
    public IGeneralType[] TypeArguments { get; set; }

    public string GetText(Indent indent = default)
    {
        return TypeArguments.Any()
            ? $"{Expression.GetText()}<{string.Join(", ", from arg in TypeArguments select arg.GetText())}>"
            : $"{Expression.GetText()}";
    }
}
