namespace TypeSharp21.AST;

public partial class Block : INode
{
    public SyntaxKind Kind => SyntaxKind.Block;

    public IStatement[]? Statements { get; set; }

    public string GetText()
    {
        if (Statements is not null && Statements.Length > 0)
        {
            return
                $"""
                {string.Join("\r\n", from node in Statements select $"{new Indent(1)}{node.GetText()}")}
                """;
        }
        else
        {
            return "";
        }
    }
}
