using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TypeScriptBuilder
    {
        public MemoryCache Cache = new(new MemoryCacheOptions());

    }
}
