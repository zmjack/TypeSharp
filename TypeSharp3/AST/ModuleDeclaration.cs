﻿namespace TypeSharp.AST;

public partial class ModuleDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.ModuleDeclaration;

    public ModuleDeclaration(Identifier name)
    {
        Modifiers = [];
        Name = name;
    }

    public Identifier Name { get; set; }
    public IModifier[]? Modifiers { get; set; }
    public required ModuleBlock Body { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            {string.Join("", from m in Modifiers select $"{m.GetText()} ")}namespace {Name.GetText()} {Body.GetText(indent)}
            """;
    }
}