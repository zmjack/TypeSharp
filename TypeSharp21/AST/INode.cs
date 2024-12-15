namespace TypeSharp21.AST;

public interface INode
{
    SyntaxKind Kind { get; }
    string GetText();
}
