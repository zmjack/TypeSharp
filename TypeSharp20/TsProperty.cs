using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsProperty : INameable
    {
        public string Name { get; set; }
        public TsType Type { get; set; }
    }
}
