namespace TypeSharp.AST;

public partial class VariableStatement : INode
{
    public SyntaxKind Kind => SyntaxKind.VariableStatement;

    public required VariableDeclarationList DeclarationList { get; set; }

    public string GetText(Indent indent = default)
    {
        return DeclarationList.GetText();
    }
}
