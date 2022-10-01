using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsEnum : INameable
    {
        public string Name { get; set; }
        public TsField[] Fields { get; set; }
    }
}
