using Ajax;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void JSendTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(typeof(JSend));

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
