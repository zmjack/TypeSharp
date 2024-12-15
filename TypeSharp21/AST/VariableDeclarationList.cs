namespace TypeSharp21.AST;

public class VariableDeclarationList : INode
{
    public SyntaxKind Kind => SyntaxKind.VariableDeclarationList;

    public SyntaxFlags Flags { get; set; }
    public List<VariableDeclaration> Declarations { get; set; } = [];

    public string GetText()
    {
        var flag = Flags switch
        {
            SyntaxFlags.Var => "var",
            SyntaxFlags.Let => "let",
            SyntaxFlags.Const => "const",
            _ => throw new NotImplementedException(),
        };
        return $"{flag} {string.Join(", ", from d in Declarations select d.GetText())};";
    }
}
