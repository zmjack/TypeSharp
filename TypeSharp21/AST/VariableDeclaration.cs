namespace TypeSharp21.AST;

public partial class VariableDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.VariableDeclaration;

    public VariableDeclaration(Identifier name)
    {
        Name = name;
    }
    public VariableDeclaration(Identifier name, IInitializer initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public Identifier Name { get; set; }
    public IGeneralType Type { get; set; }
    public IInitializer? Initializer { get; set; }

    public string GetText()
    {
        if (Initializer is not null)
        {
            return $"{Name.GetText()} = {Initializer.GetText()}";
        }
        else
        {
            return Name.GetText();
        }
    }
}
