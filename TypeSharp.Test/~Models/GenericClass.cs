using System;
using System.Collections.Generic;

namespace TypeSharp.Test
{
    [TypeScriptModel(Namespace = "TSNS3")]
    public class GenericClass<T>
    {
        public T Value { get; set; }
        public GenericClass<int> IntValue { get; set; }
    }
}
