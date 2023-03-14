using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public struct Indent
    {
        public static readonly Indent Zero = new();

        public static int Unit { get; set; } = 4;

        private static readonly MemoryCache cache = new(new MemoryCacheOptions());

        public int Value { get; set; }

        public Indent(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            var value = Value;
            return cache.GetOrCreate(value, entry =>
            {
                return " ".Repeat(value);
            });
        }

        public Indent Tab() => new(Value + Unit);

    }
}
