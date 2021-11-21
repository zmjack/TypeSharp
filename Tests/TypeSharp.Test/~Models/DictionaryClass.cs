using System.Collections.Generic;

namespace TypeSharp.Test
{
    [TypeScriptModel]
    public class DictionaryClass
    {
        public Dictionary<string, int> Dict1 { get; set; }
        public Dictionary<string, Dictionary<string, int>> Dict2 { get; set; }
    }
}
