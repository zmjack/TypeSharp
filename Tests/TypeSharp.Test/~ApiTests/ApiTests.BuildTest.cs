using System.IO;
using TypeSharp.Test.Controllers;
using Xunit;

namespace TypeSharp.Test
{
    public partial class ApiTests
    {
        [Fact]
        public void BuildTest()
        {
            var builder = new TypeScriptApiBuilder();
            builder.CacheType<SimpleController>();

            var tscode = builder.Compile();
            TestUtil.Assert(tscode, nameof(ApiTests), nameof(BuildTest));
        }

    }
}
