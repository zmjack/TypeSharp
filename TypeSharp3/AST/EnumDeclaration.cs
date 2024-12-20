namespace TypeSharp.AST;

public partial class EnumDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.EnumDeclaration;

    public EnumDeclaration(Identifier name, EnumMember[] members)
    {
        Name = name;
        Members = members;
    }

    public Identifier Name { get; set; }
    public EnumMember[] Members { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            enum {Name.GetText()} {"{"}
            {indent + 1}{string.Join($",\r\n{indent + 1}", from m in Members select m.GetText())}
            {indent}{"}"}
            """;
    }
}
