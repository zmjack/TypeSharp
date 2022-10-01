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
        public void SuperTest()
        {
            var types = Assembly.GetExecutingAssembly().GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<SuperClass>();
            builder.CacheType<ChildClass>();

            var tscode = builder.Compile();
            TestUtil.Assert(tscode, nameof(ModelTests), nameof(SuperTest));
        }
    }
}
