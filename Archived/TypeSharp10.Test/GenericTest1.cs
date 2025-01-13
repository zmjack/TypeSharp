using TypeSharp.Test.Models;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void GenericTest1()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(typeof(GenericClass<int>));
            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
