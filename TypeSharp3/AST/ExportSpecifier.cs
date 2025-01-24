namespace TypeSharp.AST;

public class ExportSpecifier : INode
{
    public SyntaxKind Kind => SyntaxKind.ExportSpecifier;

    public ExportSpecifier(Identifier name)
    {
        Name = name;
    }

    public Identifier Name { get; set; }

    public string GetText(Indent indent = default)
    {
        return Name.GetText();
    }
}
