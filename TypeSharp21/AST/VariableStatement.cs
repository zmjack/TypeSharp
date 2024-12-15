namespace TypeSharp21.AST;

public partial class VariableStatement : INode
{
    public SyntaxKind Kind => SyntaxKind.VariableStatement;

    public required VariableDeclarationList DeclarationList { get; set; }

    public string GetText()
    {
        return DeclarationList.GetText();
    }
}
