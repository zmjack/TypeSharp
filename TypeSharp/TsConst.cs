using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsConst
    {
        public string OuterNamespace { get; set; }
        public string InnerNamespace { get; set; }

        public string ConstName { get; set; }
        public TsType ConstType { get; set; }
        public string ConstValue { get; set; }

        public string ReferenceName => $"{OuterNamespace}.{InnerNamespace}.{ConstName}";
    }
}
