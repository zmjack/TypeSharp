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
        public void ValueTypeTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<ValueTypeClass>();

            var tscode = builder.Compile();
            TestUtil.Assert(tscode, nameof(ModelTests), nameof(ValueTypeTest));
        }
    }
}
