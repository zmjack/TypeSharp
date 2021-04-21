using System.IO;
using Xunit;

namespace TypeSharp.Test
{
    public class TypeScriptApiBuilderTests
    {
        [Fact]
        public void ApiBuildTest()
        {
            var builder = new TypeScriptApiBuilder("");
            builder.CacheType<SimpleController>();

            var tscode = builder.Compile();
            var expectedCode = $"{TestUtil.DeclareContent}\r\n{File.ReadAllText($"{nameof(ApiBuildTest)}.ts")}";
            Assert.Equal(expectedCode, tscode);
        }

    }
}
