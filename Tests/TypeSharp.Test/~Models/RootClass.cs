using System;
using System.ComponentModel.DataAnnotations;

namespace TypeSharp.Test
{
    [TypeScriptModel(Namespace = "TSNS1")]
    public class RootClass
    {
        public const string CONST_STRING = "const_string";
        public const int CONST_INTEGER = int.MaxValue;
        public EState State { get; set; }
        public SubClass SubClass { get; set; }
        public SubClass[] SubClasses { get; set; }
        public SubStruct SubStruct { get; set; }
        public SubStruct? NullableSubStruct { get; set; }
        public Array Array { get; set; }

        [Required]
        public string Str { get; set; }

        [Required]
        public int Int { get; set; }
        public string[] StrArray { get; set; }
        public Guid? NGuid { get; set; }
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
    }

}
