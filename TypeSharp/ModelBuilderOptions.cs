using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class ModelBuilderOptions
    {
        public static readonly ModelBuilderOptions Default = new();

        public bool Verbose { get; set; }
    }
}
