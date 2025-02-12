namespace TypeSharp.AST;

public partial class Parameter : INode
{
    public SyntaxKind Kind => SyntaxKind.Parameter;

    public Parameter(Identifier name, IGeneralType type)
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
