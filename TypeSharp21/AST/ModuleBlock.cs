namespace TypeSharp21.AST;

public class ModuleBlock : INode
{
    public SyntaxKind Kind => SyntaxKind.ModuleBlock;

    public INode[]? Statements { get; set; }

    public string GetText()
    {
        return
            $"""
            {string.Join("\r\n", from s in Statements ?? [] select $"{new Indent(1)}{s.GetText()}")}
            """;
    }
}
