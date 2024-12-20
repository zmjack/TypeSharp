namespace TypeSharp.AST;

public partial class EnumMember : INode
{
    public SyntaxKind Kind => SyntaxKind.EnumMember;

    public EnumMember(Identifier name, IInitializer initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public Identifier Name { get; set; }
    public IInitializer Initializer { get; set; }

    public string GetText(Indent indent = default)
    {
        return
            $"""
            {Name.GetText()} = {Initializer.GetText()}
            """;
    }
}
