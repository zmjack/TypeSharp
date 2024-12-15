using System.Diagnostics;

namespace TypeSharp.DevAnalyzer;

[DebuggerDisplay("{Value} * {Space}")]
public struct Indent
{
    public int Space { get; set; } = 4;
    public int Value;

    public Indent(int value)
    {
        Value = value;
    }

    private static int Clamp(int value)
    {
        if (value < 0) return 0;
        else return value;
    }

    public static Indent operator +(Indent @this, int value)
    {
        return new(Clamp(@this.Value + value))
        {
            Space = @this.Space,
        };
    }
    public static Indent operator -(Indent @this, int value)
    {
        return new(Clamp(@this.Value - value))
        {
            Space = @this.Space,
        };
    }

    public static Indent operator ++(Indent @this) => @this + 1;
    public static Indent operator --(Indent @this) => @this - 1;

    private static string GetText(int value)
    {
        return value switch
        {
            <= 0 => "",
            1 => "    ",
            2 => "        ",
            3 => "            ",
            4 => "                ",
            _ => $"{GetText(4)}{GetText(value - 4)}",
        };
    }

    public override string ToString()
    {
        return GetText(Value);
    }
}
