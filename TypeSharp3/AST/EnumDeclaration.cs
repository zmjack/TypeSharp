namespace TypeSharp.AST;

public partial class EnumDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.EnumDeclaration;

    public EnumDeclaration(Identifier name, EnumMember[] members)
    {
        Modifiers = [];
        Name = name;
        Members = members;
    }
    public EnumDeclaration(IModifier[] modifiers, Identifier name, EnumMember[] members)
    {
        Modifiers = modifiers;
        Name = name;
        Members = members;
    }

    /// <summary>
    /// <inheritdoc cref="IModifier" />
    /// </summary>
    public IModifier[] Modifiers { get; set; }
    public Identifier Name { get; set; }
    public EnumMember[] Members { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            {string.Join("", from m in Modifiers select $"{m.GetText()} ")}enum {Name.GetText()} {"{"}{(
                Members.Length > 0
                    ? $"{indent + 1}{string.Join($",\r\n{indent + 1}", from m in Members select m.GetText())}"
                    : ""
            )}            
            {indent}{"}"}
            """;
    }
}
