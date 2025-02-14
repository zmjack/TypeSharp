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
