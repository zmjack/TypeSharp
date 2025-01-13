using System.Collections.Generic;

namespace TypeSharp.Test.Models
{
    [TypeScriptModel(Namespace = "TSNS2")]
    public class SubClass
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ICollection<string> Members { get; set; }
    }

}
