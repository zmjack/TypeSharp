namespace TypeSharp.AST;

public class VariableDeclarationList : INode
{
    public SyntaxKind Kind => SyntaxKind.VariableDeclarationList;

    public SyntaxFlags Flags { get; set; }
    public VariableDeclaration[]? Declarations { get; set; }

    public string GetText(Indent indent = default)
    {
        var flag = Flags switch
        {
            SyntaxFlags.Var => "var",
            SyntaxFlags.Let => "let",
            SyntaxFlags.Const => "const",
            _ => throw new NotImplementedException(),
        };
        return $"{flag} {string.Join(", ", from d in Declarations ?? [] select d.GetText())};";
    }
}
