namespace TypeSharp.AST;

public partial class Block : INode
{
    public SyntaxKind Kind => SyntaxKind.Block;

    public Block()
    {
        Statements = [];
    }

    /// <summary>
    /// <inheritdoc cref="IStatement"/>
    /// </summary>
    public IStatement[] Statements { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            {"{"}{string.Join("", from node in Statements select $"\r\n{indent + 1}{node.GetText(indent + 1)}")}
            {indent}{"}"}
            """;
    }
}
