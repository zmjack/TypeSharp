﻿using DotNetCli;
using Ink;
using NStandard;
using NStandard.Reference;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TypeSharp.Cli
{
    [Command("TSGenerator", "tsg", Description = "Generate TypeScript model from CSharp class.")]
    public class TypeScriptGeneratorCommand : ICommand
    {
        private static string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet ts (tsg|tsgenerator) [Options]

Options:
  {"-o|--out".PadRight(20)}{"\t"}Specify the output directory path. (default: Typings)
  {"-i|--include".PadRight(20)}{"\t"}Specify to include other built-in models, such as 'JSend'.
");
        }

        public void Run(string[] args)
        {
            var conArgs = new ConArgs(args, "-");
            if (conArgs.Properties.For(x => x.ContainsKey("-h") || x.ContainsKey("--help")))
            {
                PrintUsage();
                return;
            }

            var outFolder = conArgs["-o"] ?? conArgs["-out"] ?? ".";
            var includes = conArgs["-i"]?.Split(";") ?? conArgs["--include"]?.Split(";") ?? new string[0];

            GenerateTypeScript(outFolder, includes);
        }

        private static void GenerateTypeScript(string outFolder, string[] includes)
        {
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var assemblyName = Program.ProjectInfo.AssemblyName;
            var dllPath = $"{TargetBinFolder}/{assemblyName}.dll";
            var assembly = Assembly.LoadFrom(dllPath);
            AppDomain.CurrentDomain.AssemblyResolve += GAC.CreateAssemblyResolver(Program.ProjectInfo.TargetFramework, GACFolders.All);

            #region Assembly Types
            {
                var builder = new TypeScriptModelBuilder();
                var fileName = $"{Path.GetFullPath($"{outFolder}/{assemblyName}.ts")}";
                var modelTypes = assembly.GetTypesWhichMarkedAs<TypeScriptModelAttribute>();

                builder.CacheTypes(modelTypes);
                foreach (var include in includes.Where(x => !x.Contains(",")))
                {
                    var type = assembly.GetType(include);
                    if (type != null)
                        builder.CacheTypes(type);
                    else Console.Error.WriteLine($"Can not resolve: {include}");
                }
                builder.WriteTo(fileName);

                Console.WriteLine($"File saved: {fileName}");
            }
            #endregion

            var _includes = includes.Where(x => x.Count(",") == 1).Select(include =>
            {
                var parts = include.Split(",");
                return new
                {
                    String = include,
                    TypeName = parts[0],
                    FileName = parts[1],
                };
            });

            foreach (var group in _includes.GroupBy(x => x.FileName))
            {
                var refDllPath = $"{TargetBinFolder}/{group.Key}.dll";
                var refAssembly = Assembly.LoadFrom(refDllPath);
                var fileName = $"{Path.GetFullPath($"{outFolder}/{group.Key}.ts")}";

                var builder = new TypeScriptModelBuilder();
                foreach (var item in group)
                {
                    var type = refAssembly.GetType(item.TypeName);
                    if (type != null)
                        builder.CacheType(type);
                    else Console.Error.WriteLine($"Can not resolve: {item.String}");
                }

                builder.WriteTo(fileName);
                Console.WriteLine($"File saved: {fileName}");
            }
        }

    }
}
