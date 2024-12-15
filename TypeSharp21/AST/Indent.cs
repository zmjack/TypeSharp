using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace TypeSharp21.AST;

public struct Indent
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public static readonly Indent Zero = new(0);

    public int Space { get; set; } = 4;
    public int Value { get; set; }

    public Indent(int value)
    {
        Value = value;
    }

    public override string ToString()
    {
        var indent = Space * Value;
        return _cache.GetOrCreate(indent, entry =>
        {
            var sb = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        })!;
    }

    public Indent Tab() => new(Value + 1)
    {
        Space = Space,
    };

}
