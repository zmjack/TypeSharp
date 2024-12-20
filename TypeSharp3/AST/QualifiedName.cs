namespace TypeSharp.AST;

public partial class QualifiedName : INode
{
    public SyntaxKind Kind => SyntaxKind.QualifiedName;

    public QualifiedName(string fullname)
    {
        var lastDot = fullname.LastIndexOf('.');
        if (lastDot == -1) throw new ArgumentException($"Not a qualified name. (FullName: {fullname})");

        var left = fullname[..lastDot];
        var right = fullname[(lastDot + 1)..];

        Left = left.Contains('.') ? new QualifiedName(left) : new Identifier(left);
        Right = new Identifier(right);
    }
    public QualifiedName(IIdentifier left, Identifier right)
    {
        Left = left;
        Right = right;
    }

    public IIdentifier Left { get; set; }
    public Identifier Right { get; set; }

    public string GetText(Indent indent = default)
    {
        return $"{Left.GetText()}.{Right.GetText()}";
    }
}
