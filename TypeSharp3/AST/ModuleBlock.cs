namespace TypeSharp.AST;

public class ModuleBlock : INode
{
    public SyntaxKind Kind => SyntaxKind.ModuleBlock;

    public INode[]? Statements { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""            
            {"{"}{string.Join($"", from s in Statements ?? [] select $"\r\n{indent + 1}{s.GetText(indent + 1)}")}
            {indent}{"}"}
            """;
    }
}
