namespace TypeSharp.AST;

public partial class ClassDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.ClassDeclaration;

    public ClassDeclaration(Identifier name)
    {
        Modifiers = [];
        Name = name;
        Members = [];
    }

    public ClassDeclaration(IModifier modifier, Identifier name)
    {
        Modifiers = [modifier];
        Name = name;
        Members = [];
    }

    public IModifier[] Modifiers { get; set; }
    public Identifier Name { get; set; }
    /// <summary>
    /// <inheritdoc cref="IMember" />
    /// </summary>
    public IMember[] Members { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            {string.Join("", from m in Modifiers select $"{m.GetText()} ")}class {Name.GetText()} {"{"}
            {indent + 1}{string.Join($"\r\n{indent + 1}", from m in Members select m.GetText(indent + 1))}
            {indent}{"}"}
            """;
    }
}
