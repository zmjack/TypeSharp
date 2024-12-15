namespace TypeSharp21.AST;

public class SourceFile : INode
{
    public SyntaxKind Kind => SyntaxKind.SourceFile;

    public string? FileName { get; set; }

    public required INode[] Statements { get; set; }

    public string GetText()
    {
        return string.Join("\r\n", from node in Statements select node.GetText());
    }
}
