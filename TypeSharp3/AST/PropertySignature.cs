namespace TypeSharp.AST;

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
    public QuestionToken? QuestionToken { get; set; }

    public string GetText(Indent indent = default)
    {
        if (Type is not null)
        {
            return $"{Name.GetText()}{QuestionToken?.GetText()}: {Type.GetText()}";
        }
        else
        {
            return $"{Name.GetText()}{QuestionToken?.GetText()}";
        }
    }
}
