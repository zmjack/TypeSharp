namespace TypeSharp.AST;

public interface INode
{
    SyntaxKind Kind { get; }
    string GetText(Indent indent = default);
}
