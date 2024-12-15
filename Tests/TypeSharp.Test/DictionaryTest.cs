using TypeSharp.Test.Models;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void DictionaryTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<DictionaryClass>();

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
