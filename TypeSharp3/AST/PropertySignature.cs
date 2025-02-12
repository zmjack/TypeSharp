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
        IGeneralType? type = Type;
        if (type is not null)
        {
            if (QuestionToken is not null && type is UnionType union)
            {
                type = new UnionType([
                    .. from x in union.Types where x != UndefinedKeyword.Default select x
                ]);
            }
            return $"{Name.GetText()}{QuestionToken?.GetText()}: {type.GetText()}";
        }
        else
        {
            return $"{Name.GetText()}{QuestionToken?.GetText()}";
        }
    }
}
