using DotNetCli;
using NStandard;
using System;
using System.Reflection;

namespace TypeSharp.Cli
{
    public class Program
    {
        public static readonly string CLI_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static CmdContainer CmdContainer;

        static void Main(string[] args)
        {
            CmdContainer = new CmdContainer("ts", ProjectInfo.GetCurrent());
            CmdContainer.CacheCommands(Assembly.GetExecutingAssembly());

            PrintWelcome();

            CmdContainer.PrintProjectInfo();
            CmdContainer.Run(args);
        }

        public static void PrintWelcome()
        {
            Console.WriteLine($@"
{"ヽ(*^▽^)ノ".Center(60)}

TypeSharp .NET Command-line Tools {CLI_VERSION}");
        }

    }
}
