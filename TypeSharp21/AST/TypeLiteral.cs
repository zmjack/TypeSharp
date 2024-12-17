namespace TypeSharp.AST;

public partial class TypeLiteral : INode
{
    public SyntaxKind Kind => SyntaxKind.TypeLiteral;

    public IMember[]? Members { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""            
            {"{"} {string.Join($", ", from m in Members ?? [] select $"{m.GetText()}")} {"}"}
            """;
    }
}
