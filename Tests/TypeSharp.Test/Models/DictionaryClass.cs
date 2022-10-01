using System.Collections.Generic;
using TypeSharp.Test.Models;

namespace TypeSharp.Test.Models
{
    [TypeScriptModel]
    public class DictionaryClass
    {
        public Dictionary<string, int> Dict1 { get; set; }
        public Dictionary<string, Dictionary<string, int>> Dict2 { get; set; }
        public Dictionary<string, SuperClass> Dict3 { get; set; }
        public Dictionary<string, SpecialNsClass> Dict4 { get; set; }
    }
}
