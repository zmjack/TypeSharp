namespace TypeSharp21.AST;

public partial class TypeReference : INode
{
    public SyntaxKind Kind => SyntaxKind.TypeReference;

    public static TypeReference Date { get; } = new TypeReference("Date");

    public TypeReference(Identifier typeName)
    {
        TypeName = typeName;
    }

    public Identifier TypeName { get; set; }

    public string GetText()
    {
        return TypeName.GetText();
    }
}
