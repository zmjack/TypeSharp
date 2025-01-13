namespace TypeSharp.AST;

public partial class Constructor : INode
{
    public SyntaxKind Kind => SyntaxKind.Constructor;

    public Constructor(Parameter[] parameters)
    {
        Parameters = parameters;
    }

    public Parameter[] Parameters { get; set; }
    public required Block Body { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            constructor({string.Join(", ", from p in Parameters select p.GetText())}) {Body.GetText(indent)}
            """;
    }
}
