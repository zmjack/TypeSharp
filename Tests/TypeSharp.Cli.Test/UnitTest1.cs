using DotNetCli;
using System.IO;
using Xunit;

namespace TypeSharp.Cli.Test
{
    public class UnitTest1
    {
        private static CmdContainer _container { get; } = new("ts", Program.ThisAssembly, Project.GetFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "../../..")));

        [Fact]
        public void Test1()
        {
            _container.Run(["tsg", "-r", "System.Lazy`1[System.Int32];number", "-r", "System.Lazy`1;any"]);
            var content = File.ReadAllText("TypeSharp.Cli.Test.ts");
            Assert.Equal($@"{TestUtil.DeclareContent}

declare namespace TypeSharp.Test {{
    interface SimpleModel {{
        value?: number;
        lazyInt32?: number;
        lazyInt64?: any;
        dateTimeValue?: Date;
    }}
}}
", content);
        }

        [Fact]
        public void Test2()
        {
            _container.Run(["tsg", "-r", "System.Lazy`1[System.Int32];number", "-r", "System.Lazy`1;any"]);
            var content = File.ReadAllText("TypeSharp.Cli.Test.ts");
            Assert.Equal($@"{TestUtil.DeclareContent}

declare namespace TypeSharp.Test {{
    interface SimpleModel {{
        value?: number;
        lazyInt32?: number;
        lazyInt64?: any;
        dateTimeValue?: Date;
    }}
}}
", content);
        }

    }
}
