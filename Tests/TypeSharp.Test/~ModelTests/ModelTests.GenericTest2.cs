using Ajax;
using NStandard;
using System;
using System.IO;
using System.Reflection;
using TypeSharp.Test.Models;
using TypeSharp.Test.Models.Interfaces;
using Xunit;

namespace TypeSharp.Test
{
    public partial class ModelTests
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
            TestUtil.Assert(tscode, nameof(ModelTests), nameof(GenericTest2));
        }
    }
}
