using DotNetCli;
using NStandard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TypeSharp.Cli
{
    public class Program
    {
        public static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();
        public static readonly string CLI_VERSION = ThisAssembly.GetName().Version.ToString();
        public static CmdContainer CmdContainer { get; } = new("ts", ThisAssembly, ProjectInfo.GetFromDirectory(Directory.GetCurrentDirectory()));

        static void Main(string[] args)
        {
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
