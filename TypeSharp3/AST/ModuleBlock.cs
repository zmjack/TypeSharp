namespace TypeSharp.AST;

public class ModuleBlock : INode
{
    public SyntaxKind Kind => SyntaxKind.ModuleBlock;

    public INode[]? Statements { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""            
            {"{"}
            {indent + 1}{string.Join($"\r\n{indent + 1}", from s in Statements ?? [] select s.GetText(indent + 1))}
            {indent}{"}"}
            """;
    }
}
