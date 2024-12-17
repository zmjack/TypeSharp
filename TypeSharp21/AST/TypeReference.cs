namespace TypeSharp.AST;

public partial class TypeReference : INode
{
    public SyntaxKind Kind => SyntaxKind.TypeReference;

    public static TypeReference Date { get; } = new TypeReference("Date");

    public TypeReference(Identifier typeName)
    {
        TypeName = typeName;
    }

    public Identifier TypeName { get; set; }

    public string GetText(Indent indent = default)
    {
        return TypeName.GetText();
    }
}
