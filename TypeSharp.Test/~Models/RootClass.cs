using System;
using System.Collections.Generic;

namespace TypeSharp.Test
{
    [TypeScriptModel(Namespace = "TSNS1")]
    public class RootClass
    {
        public const string CONST_STRING = "const_string";
        public const int CONST_INTEGER = int.MaxValue;
        public EState State { get; set; }
        public SubClass Sub { get; set; }
        public SubClass[] Subs { get; set; }
        public string Str { get; set; }
        public int Int { get; set; }
        public string[] StrArray { get; set; }
        public Guid? NGuid { get; set; }
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
    }
}
