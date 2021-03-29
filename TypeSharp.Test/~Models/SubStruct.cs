using System.Collections.Generic;

namespace TypeSharp.Test
{
    [TypeScriptModel(Namespace = "TSNS2")]
    public struct SubStruct
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ICollection<string> Members { get; set; }
    }

}
