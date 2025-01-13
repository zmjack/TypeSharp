using TypeSharp.Test.Controllers;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void BuildTest()
        {
            var builder = new TypeScriptApiBuilder();
            builder.CacheType<SimpleController>();

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }

    }
}
