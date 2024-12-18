namespace TypeSharp.AST;

public partial class InterfaceDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;

    public InterfaceDeclaration(Identifier name)
    {
        Name = name;
        Members = [];
        TypeParameters = [];
    }
    public InterfaceDeclaration(Identifier name, TypeParameter[] typeParameters)
    {
        Name = name;
        Members = [];
        TypeParameters = typeParameters;
    }

    public Identifier Name { get; set; }
    public IMember[] Members { get; set; }
    public TypeParameter[] TypeParameters { get; set; }

    public string GetText(Indent indent = default)
    {
        var generics = TypeParameters.Length != 0
            ? $"<{string.Join(", ", from p in TypeParameters select p.GetText())}>"
            : "";

        return
            $"""
            interface {Name.GetText()}{generics} {"{"}
            {indent + 1}{string.Join($"\r\n{indent + 1}", from m in Members select $"{m.GetText(indent + 1)};")}
            {indent}{"}"}
            """;
    }
}
