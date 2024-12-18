namespace TypeSharp.AST;

public partial class TypeReference : INode
{
    public SyntaxKind Kind => SyntaxKind.TypeReference;

    public static TypeReference Date { get; } = new TypeReference("Date");
    public static TypeReference Promise(IGeneralType[] typeArguments)
    {
        return new TypeReference(nameof(Promise), typeArguments);
    }

    public TypeReference(Identifier typeName)
    {
        TypeName = typeName;
        TypeArguments = [];
    }

    public TypeReference(Identifier typeName, IGeneralType[] typeArguments)
    {
        TypeName = typeName;
        TypeArguments = typeArguments;
    }

    public Identifier TypeName { get; set; }
    public IGeneralType[] TypeArguments { get; set; }

    public string GetText(Indent indent = default)
    {
        if (TypeArguments.Length != 0)
        {
            return $"{TypeName.GetText()}<{string.Join(", ", from arg in TypeArguments select arg.GetText())}>";
        }
        else
        {
            return TypeName.GetText();
        }
    }
}
