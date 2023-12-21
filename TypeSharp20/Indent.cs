using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public struct Indent
    {
        private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

        public static readonly Indent Zero = new(4, 0);

        public int Space { get; set; } = 4;
        public int Value { get; set; }

        public Indent(int space, int value)
        {
            Space = space;
            Value = value;
        }

        public override string ToString()
        {
            var indent = Space * Value;
            return _cache.GetOrCreate(indent, entry =>
            {
                return " ".Repeat(indent);
            });
        }

        public Indent Tab() => new(Space, Value + 1);

    }
}
