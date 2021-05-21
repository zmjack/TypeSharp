using System;

namespace TypeSharp.Test
{
    [TypeScriptModel]
    public class SimpleModel
    {
        public int Value { get; set; }

        [TypeScriptIgnore]
        public int Hidden { get; set; }

        public Lazy<int> LazyInt32 { get; set; }
        public Lazy<long> LazyInt64 { get; set; }
    }
}
