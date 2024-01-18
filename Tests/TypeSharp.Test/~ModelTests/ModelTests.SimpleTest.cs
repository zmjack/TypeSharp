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
        public void SimpleTest()
        {
            var types = Assembly.GetExecutingAssembly().GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<RootClass>();
            builder.CacheType<JSend>("fe");

            var tscode = builder.Compile();
            TestUtil.Assert(tscode, nameof(ModelTests), nameof(SimpleTest));
        }
    }
}
