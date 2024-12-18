namespace TypeSharp.AST;

public partial class TypeParameter : INode
{
    public SyntaxKind Kind => SyntaxKind.TypeParameter;

    public TypeParameter(Identifier name)
    {
        Name = name;
    }

    public Identifier Name { get; set; }
    public IGeneralType? Constraint { get; set; }

    public string GetText(Indent indent = default)
    {
        if (Constraint is not null)
        {
            return $"{Name.GetText()} extends {Constraint.GetText()}";
        }
        else
        {
            return $"{Name.GetText()}";
        }
    }
}
