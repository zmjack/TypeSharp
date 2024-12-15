using TypeSharp.Test.Models.Interfaces;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void InterfaceTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<ISimpleInterface>();

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
