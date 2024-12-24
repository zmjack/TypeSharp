namespace TypeSharp.AST;

public partial class ClassDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.ClassDeclaration;

    public ClassDeclaration(Identifier name)
    {
        Modifiers = [];
        Name = name;
        Members = [];
        TypeParameters = [];
    }
    public ClassDeclaration(Identifier name, TypeParameter[] typeParameters)
    {
        Modifiers = [];
        Name = name;
        Members = [];
        TypeParameters = typeParameters;
    }

    public ClassDeclaration(IModifier[] modifiers, Identifier name)
    {
        Modifiers = modifiers;
        Name = name;
        Members = [];
        TypeParameters = [];
    }
    public ClassDeclaration(IModifier[] modifiers, Identifier name, TypeParameter[] typeParameters)
    {
        Modifiers = modifiers;
        Name = name;
        Members = [];
        TypeParameters = typeParameters;
    }

    /// <summary>
    /// <inheritdoc cref="IModifier" />
    /// </summary>
    public IModifier[] Modifiers { get; set; }
    public Identifier Name { get; set; }
    /// <summary>
    /// <inheritdoc cref="IMember" />
    /// </summary>
    public IMember[] Members { get; set; }
    public TypeParameter[] TypeParameters { get; set; }

    public string GetText(Indent indent = default)
    {
        var generics = TypeParameters.Length != 0
            ? $"<{string.Join(", ", from p in TypeParameters select p.GetText())}>"
            : "";

        return
            $"""
            {string.Join("", from m in Modifiers select $"{m.GetText()} ")}class {Name.GetText()}{generics} {"{"}
            {indent + 1}{string.Join($"\r\n{indent + 1}", from m in Members select m.GetText(indent + 1))}
            {indent}{"}"}
            """;
    }
}
