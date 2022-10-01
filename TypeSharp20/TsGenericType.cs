using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsGenericType : INameable
    {
        public string Name { get; set; }
        public TsType Extends { get; set; }
    }
}
