using Dawnx;
using DotNetCli;
using Ajax;
using NEcho;
using NStandard;
using NStandard.Reference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TypeSharp.Cli
{
    [Command("TSGenerator", "tsg", Description = "Generate TypeScript model from CSharp class.")]
    public class TypeScriptGeneratorCommand : ICommand
    {
        private static string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet nx (tsg|tsgenerator) [Options]

Options:
  {"-o|--out".PadRight(20)}{"\t"}Specify the output directory path. (default: Typings)
  {"-i|--include".PadRight(20)}{"\t"}Specify to include other built-in models, such as 'JSend'.
");
        }

        public void Run(ConArgs cargs)
        {
            if (cargs.Properties.For(x => x.ContainsKey("-h") || x.ContainsKey("--help")))
            {
                PrintUsage();
                return;
            }

            var outFolder = cargs["-o"] ?? cargs["-out"] ?? "Typings";
            var includes = cargs["-i"]?.Split(",") ?? cargs["--include"]?.Split(",") ?? new string[0];

            GenerateTypeScript(outFolder, includes);
        }

        private static void GenerateTypeScript(string outFolder, string[] includes)
        {
            includes = includes?.Select(x => x.ToLower()).ToArray() ?? new string[0];

            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var dllPath = $"{TargetBinFolder}/{Program.ProjectInfo.AssemblyName}.dll";
            var assembly = Assembly.LoadFrom(dllPath);
            AppDomain.CurrentDomain.AssemblyResolve += GAC.CreateAssemblyResolver(Program.ProjectInfo.TargetFramework, GACFolders.All);

            #region Assembly Types
            {
                var builder = new TypeScriptModelBuilder();
                var fileName = $"{Path.GetFullPath($"{outFolder}/{Program.ProjectInfo.AssemblyName}.ts")}";
                var modelTypes = assembly.GetTypesWhichMarkedAs<TypeScriptModelAttribute>();

                builder.CacheTypes(modelTypes);
                builder.WriteTo(fileName);

                Console.WriteLine($"File saved: {fileName}");
            }
            #endregion

            #region JSend Types
            if (includes.Contains("jsend"))
            {
                var builder = new TypeScriptModelBuilder();
                var fileName = $"{Path.GetFullPath($"{outFolder}/JSend.ts")}";

                builder.CacheType<JSend>();
                builder.WriteTo(fileName);

                Console.WriteLine($"File saved: {fileName}");
            };
            #endregion
        }

    }
}
