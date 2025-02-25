﻿namespace TypeSharp.AST;

public partial class InterfaceDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;

    public InterfaceDeclaration(Identifier name)
    {
        Modifiers = [];
        Name = name;
        Members = [];
        TypeParameters = [];
    }
    public InterfaceDeclaration(Identifier name, TypeParameter[] typeParameters)
    {
        Modifiers = [];
        Name = name;
        Members = [];
        TypeParameters = typeParameters;
    }
    public InterfaceDeclaration(IModifier[] modifiers, Identifier name)
    {
        Modifiers = modifiers;
        Name = name;
        Members = [];
        TypeParameters = [];
    }
    public InterfaceDeclaration(IModifier[] modifiers, Identifier name, TypeParameter[] typeParameters)
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
            {string.Join("", from m in Modifiers select $"{m.GetText()} ")}interface {Name.GetText()}{generics} {"{"}{(
                Members.Length > 0
                    ? string.Join("", from m in Members select $"\r\n{indent + 1}{m.GetText(indent + 1)};")
                    : ""
            )}
            {indent}{"}"}
            """;
    }
}
