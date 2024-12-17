namespace TypeSharp.AST;

public partial class InterfaceDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;

    public InterfaceDeclaration(Identifier name)
    {
        Members = [];
        Name = name;
    }

    public Identifier Name { get; set; }
    public IMember[] Members { get; set; }
    public TypeParameter[]? TypeParameters { get; set; }

    public string GetText(Indent indent = default)
    {
        if (TypeParameters is not null && TypeParameters.Length != 0)
        {
            return
                $"""
                interface {Name.GetText()}<{string.Join(", ", from p in TypeParameters select p.GetText())}> {"{"}
                {indent + 1}{string.Join($"\r\n{indent + 1}", from m in Members select m.GetText(indent + 1))}
                {indent}{"}"}
                """;
        }
        else
        {
            return
                $"""
                interface {Name.GetText()} {"{"}
                {indent + 1}{string.Join($"\r\n{indent + 1}", from m in Members select $"{m.GetText(indent + 1)};")}
                {indent}{"}"}
                """;
        }
    }
}
