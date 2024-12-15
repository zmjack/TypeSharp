using System;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void GenericTest2()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(Type.GetType("TypeSharp.Test.Models.GenericClass`1"));
            var tscode = builder.Compile(new BuildOptions
            {
                OutputNames = true,
            });
            Util.Assert(tscode);
        }
    }
}
