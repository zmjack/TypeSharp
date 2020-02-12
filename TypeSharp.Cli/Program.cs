#define DEV_OFF
using DotNetCli;
using NEcho;
using NStandard;
using System;
using System.Reflection;

namespace TypeSharp.Cli
{
    public class Program
    {
        public static readonly string CLI_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();

#if DEV
        public static CommandContainer CommandContainer = new CommandContainer(ProjectInfo.Get(@""), "ts");
#else
        public static CommandContainer CommandContainer = new CommandContainer(ProjectInfo.GetCurrent(), "ts");
#endif

        public static ProjectInfo ProjectInfo => CommandContainer.ProjectInfo;

        static void Main(string[] args)
        {
#if DEV
            args = new[] { "tsg" };
#endif

            CommandContainer.CacheCommands(Assembly.GetExecutingAssembly());

            PrintWelcome();

            var conArgs = new ConArgs(args, "-");
            CommandContainer.PrintProjectInfo();
            CommandContainer.Run(conArgs);
        }

        public static void PrintWelcome()
        {
            Console.WriteLine($@"
{"ヽ(*^▽^)ノ".Center(60)}

TypeSharp .NET Command-line Tools {CLI_VERSION}");
        }

    }
}
