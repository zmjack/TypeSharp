namespace TypeSharp21.AST;

public partial class GetAccessor : INode
{
    public SyntaxKind Kind => SyntaxKind.GetAccessor;

    public GetAccessor(Identifier name)
    {
        Name = name;
    }

    public GetAccessor(Identifier name, IGeneralType type)
    {
        Name = name;
        Type = type;
    }

    public GetAccessor(IModifier modifier, Identifier name)
    {
        Modifiers = [modifier];
        Name = name;
    }

    public GetAccessor(IModifier modifier, Identifier name, IGeneralType type)
    {
        Modifiers = [modifier];
        Name = name;
        Type = type;
    }

    public IModifier[]? Modifiers { get; set; }
    public Identifier Name { get; set; }
    public IGeneralType? Type { get; set; }

    public string GetText()
    {
        return $"{string.Join("", from m in Modifiers ?? [] select $"{m.GetText()} ")}get {Name.GetText()}(){(Type is not null ? $": {Type.GetText()}" : "")}";
    }
}
