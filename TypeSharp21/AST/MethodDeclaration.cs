namespace TypeSharp.AST;

public partial class MethodDeclaration : INode
{
    public SyntaxKind Kind => SyntaxKind.MethodDeclaration;

    public MethodDeclaration(Identifier name, Parameter[] parameters)
    {
        Modifiers = [];
        Name = name;
        Parameters = parameters;
    }
    public MethodDeclaration(Identifier name, Parameter[] parameters, IGeneralType type)
    {
        Modifiers = [];
        Name = name;
        Parameters = parameters;
        Type = type;
    }

    public MethodDeclaration(IModifier[] modifiers, Identifier name, Parameter[] parameters)
    {
        Modifiers = modifiers;
        Name = name;
        Parameters = parameters;
    }
    public MethodDeclaration(IModifier[] modifiers, Identifier name, Parameter[] parameters, IGeneralType type)
    {
        Modifiers = modifiers;
        Name = name;
        Parameters = parameters;
        Type = type;
    }

    public IModifier[] Modifiers { get; set; }
    public Identifier Name { get; set; }
    public Parameter[] Parameters { get; set; }
    public IGeneralType? Type { get; set; }
    public required Block Body { get; set; }

    public string GetText(Indent indent = default)
    {
        if (Type is not null)
        {
            return
                $"""
                {string.Join("", from m in Modifiers select $"{m.GetText()} ")}{Name.GetText()}({string.Join(", ", from p in Parameters select p.GetText())}): {Type.GetText()} {Body.GetText(indent)}
                """;
        }
        else
        {
            return
                $"""
                {string.Join("", from m in Modifiers select $"{m.GetText()} ")}{Name.GetText()}({string.Join(", ", from p in Parameters select p.GetText())}) {Body.GetText(indent)}
                """;
        }
    }
}
