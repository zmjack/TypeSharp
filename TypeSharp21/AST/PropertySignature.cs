namespace TypeSharp21.AST;

public partial class PropertySignature : INode, InterfaceDeclaration.IMember
{
    public SyntaxKind Kind => SyntaxKind.PropertySignature;

    public PropertySignature(Identifier name)
    {
        Name = name;
    }

    public PropertySignature(Identifier name, IGeneralType type)
    {
        Name = name;
        Type = type;
    }

    public Identifier Name { get; set; }
    public IGeneralType? Type { get; set; }

    public string GetText()
    {
        if (Type is not null)
        {
            return $"{Name.GetText()}: {Type.GetText()}";
        }
        else
        {
            return $"{Name.GetText()}";
        }
    }
}
