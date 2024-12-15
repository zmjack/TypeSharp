using Ajax;
using NStandard;
using System.Reflection;
using TypeSharp.Test.Models;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void SimpleTest()
        {
            var types = Assembly.GetExecutingAssembly().GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<RootClass>();
            builder.CacheType<JSend>("fe");

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
