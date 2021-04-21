using System.IO;
using Xunit;

namespace TypeSharp.Cli.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var container = TestUtil.DefaultCmdContainer;
            container.Run(new[] { "tsg" });
            var content = File.ReadAllText("TypeSharp.Cli.Test.ts");
            Assert.Equal($@"{TestUtil.DeclareContent}

declare namespace TypeSharp.Test {{
    interface SimpleModel {{
        value?: number;
    }}
}}
", content);
        }
    }
}
