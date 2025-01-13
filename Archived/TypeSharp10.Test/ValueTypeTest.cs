using TypeSharp.Test.Models;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void ValueTypeTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<ValueTypeClass>();

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
