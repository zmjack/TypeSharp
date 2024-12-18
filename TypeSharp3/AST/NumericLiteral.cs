namespace TypeSharp.AST;

public partial class NumericLiteral : INode
{
    public SyntaxKind Kind => SyntaxKind.NumericLiteral;

    public NumericLiteral(byte value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(short value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(ushort value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(int value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(uint value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(long value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(ulong value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(float value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(double value)
    {
        Text = value.ToString();
    }
    public NumericLiteral(decimal value)
    {
        Text = value.ToString();
    }

    public string Text { get; set; }

    public string GetText(Indent indent = default)
    {
        return Text;
    }
}
