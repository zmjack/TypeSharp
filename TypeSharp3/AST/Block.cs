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
            {"{"}
            {indent + 1}{string.Join($"\r\n{indent + 1}", from node in Statements select node.GetText(indent + 1))}
            {indent}{"}"}
            """;
    }
}
