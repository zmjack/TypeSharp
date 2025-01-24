namespace TypeSharp.AST;

public partial class ExportDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.ExportDeclaration;

    public required NamedExports ExportClause { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            export {"{"}{ExportClause.GetText(indent)}
            {indent}{"}"}
            """;
    }
}
