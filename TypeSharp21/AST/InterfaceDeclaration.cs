namespace TypeSharp21.AST;

public partial class InterfaceDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;

    public InterfaceDeclaration(Identifier name)
    {
        Name = name;
    }

    public Identifier Name { get; set; }
    public IMember[]? Members { get; set; }
    public TypeParameter[]? TypeParameters { get; set; }

    public string GetText()
    {
        if (TypeParameters is not null && TypeParameters.Length > 0)
        {
            return
                $"""
                interface {Name.GetText()}<{string.Join(", ", from p in TypeParameters select p.GetText())}> {"{"}
                {string.Join($"\r\n", from m in Members ?? [] select $"{new Indent(1)}{m.GetText()};")}
                {"}"}
                """;
        }
        else
        {
            return
                $"""
                interface {Name.GetText()} {"{"}
                {string.Join($"\r\n", from m in Members ?? [] select $"{new Indent(1)}{m.GetText()};")}
                {"}"}
                """;
        }
    }
}
