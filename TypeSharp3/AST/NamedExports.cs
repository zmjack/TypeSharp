namespace TypeSharp.AST;

public class NamedExports : INode
{
    public SyntaxKind Kind => SyntaxKind.NamedExports;

    public required ExportSpecifier[] Elements { get; set; }

    public string GetText(Indent indent = default)
    {
        var generics = Elements.Length != 0
            ? $"<{string.Join(", ", from e in Elements select e.GetText())}>"
            : "";

        return
            Elements.Length > 0
                ? $"\r\n{indent + 1}{string.Join($",\r\n{indent + 1}", from e in Elements select e.GetText())}"
                : "";
    }
}
