using System.Diagnostics;

namespace TypeSharp.AST;

[DebuggerDisplay("{TypeName}")]
public partial class TypeReference : INode
{
    public SyntaxKind Kind => SyntaxKind.TypeReference;

    public static TypeReference Record(IGeneralType key, IGeneralType value)
    {
        return new(new Identifier("Record"), [key, value]);
    }
    public static TypeReference Date { get; } = new(new Identifier("Date"));
    public static TypeReference Promise(IGeneralType typeArgument)
    {
        return new(new Identifier("Promise"), [typeArgument]);
    }
    public static TypeReference Array(IGeneralType typeArgument)
    {
        return new(new Identifier("Array"), [typeArgument]);
    }

    public TypeReference(IIdentifier typeName)
    {
        TypeName = typeName;
        TypeArguments = [];
    }

    public TypeReference(IIdentifier typeName, IGeneralType[] typeArguments)
    {
        TypeName = typeName;
        TypeArguments = typeArguments;
    }

    public IIdentifier TypeName { get; set; }
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
