namespace TypeSharp.AST;

public class SourceFile : INode
{
    public SyntaxKind Kind => SyntaxKind.SourceFile;

    public string? FileName { get; set; }

    /// <summary>
    /// <inheritdoc cref="IStatement"/>
    /// </summary>
    public required IStatement[] Statements { get; set; }

    public string GetText(Indent indent = default)
    {
        if (indent.Space == 0) indent = new Indent(0, 2);

        return string.Join("\r\n", from node in Statements select node.GetText(indent));
    }
}
