namespace TypeSharp.AST;

public class QuestionToken : INode
{
    public SyntaxKind Kind => SyntaxKind.QuestionToken;

    public string GetText(Indent indent = default)
    {
        return "?";
    }
}
