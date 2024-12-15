using NStandard;
using System.Reflection;
using TypeSharp.Test.Models;
using Xunit;

namespace TypeSharp.Test
{
    public partial class CommonTests
    {
        [Fact]
        public void SuperTest()
        {
            var types = Assembly.GetExecutingAssembly().GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<SuperClass>();
            builder.CacheType<ChildClass>();

            var tscode = builder.Compile();
            Util.Assert(tscode);
        }
    }
}
