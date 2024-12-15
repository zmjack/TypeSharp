namespace TypeSharp21.AST;

public partial class SetAccessor : INode
{
    public SyntaxKind Kind => SyntaxKind.SetAccessor;

    public SetAccessor(Identifier name, Parameter parameter)
    {
        Name = name;
        Parameters = [parameter];
    }

    public IModifier[]? Modifiers { get; set; }
    public Identifier Name { get; set; }
    public Parameter[] Parameters { get; set; }

    public string GetText()
    {
        return $"{string.Join("", from m in Modifiers ?? [] select $"{m.GetText()} ")}set {Name.GetText()}({string.Join(", ", from p in Parameters select p.GetText())})";
    }
}
