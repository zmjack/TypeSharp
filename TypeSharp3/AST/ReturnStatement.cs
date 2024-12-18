namespace TypeSharp.AST;

public partial class ReturnStatement : INode
{
    public SyntaxKind Kind => SyntaxKind.ReturnStatement;

    public ReturnStatement(IExpression expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// <inheritdoc cref="IExpression" />
    /// </summary>
    public IExpression Expression { get; set; }

    public string GetText(Indent indent = default)
    {
        return $"return {Expression.GetText()};";
    }
}
