namespace TypeSharp.AST;

public partial class Parameter : INode
{
    public SyntaxKind Kind => SyntaxKind.Parameter;

    public Parameter(Identifier name, IGeneralType type)
    {
        Name = name;
        Type = type;
    }
    public Parameter(Identifier name, IGeneralType type, IGeneralType initializer)
    {
        Name = name;
        Type = type;
        Initializer = initializer;
    }

    public Identifier Name { get; set; }
    public IGeneralType? Type { get; set; }
    public IGeneralType? Initializer { get; set; }
    public QuestionToken? QuestionToken { get; set; }

    public string GetText(Indent indent = default)
    {
        if (Type is not null)
        {
            if (Initializer is not null)
            {
                return $"{Name.GetText()}{QuestionToken?.GetText()}: {Type.GetText()} = {Initializer.GetText()}";
            }
            else
            {
                return $"{Name.GetText()}{QuestionToken?.GetText()}: {Type.GetText()}";
            }
        }
        else
        {
            if (Initializer is not null)
            {
                return $"{Name.GetText()}{QuestionToken?.GetText()} = {Initializer.GetText()}";
            }
            else
            {
                return $"{Name.GetText()}{QuestionToken?.GetText()}";
            }
        }
    }
}
