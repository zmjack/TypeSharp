using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsField : INameable
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
