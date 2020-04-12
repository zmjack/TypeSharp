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
        private static readonly string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet ts (tsg|tsgenerator) [Options]

Options:
  {"-o|--out",20}{"\t"}Specify the output directory path. (default: Typings)
  {"-i|--include",20}{"\t"}Specify the include other types, such as 'Ajax.JSend,JSend.dll'.
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

            var outFolder = conArgs["-o"] ?? conArgs["--out"] ?? ".";
            var includes = conArgs["-i"]?.Split(";") ?? conArgs["--include"]?.Split(";") ?? new string[0];

            GenerateTypeScript(outFolder, includes);
        }

        private static void GenerateTypeScript(string outFolder, string[] includes)
        {
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var assemblyName = Program.ProjectInfo.AssemblyName;
            Assembly.LoadFrom($"{TargetBinFolder}/{assemblyName}.dll");
            AppDomain.CurrentDomain.AssemblyResolve += GAC.CreateAssemblyResolver(Program.ProjectInfo.TargetFramework, GACFolders.All);

            var _includes = includes.Select(include =>
            {
                if (include.Count(",") == 1)
                {
                    var parts = include.Split(",");
                    return new
                    {
                        TypeString = include,
                        TypeName = parts[0],
                        AssemblyName = parts[1],
                    };
                }
                else
                {
                    return new
                    {
                        TypeString = include,
                        TypeName = include,
                        AssemblyName = assemblyName,
                    };
                }
            }).Concat(new[]
            {
                new
                {
                    TypeString = "",
                    TypeName = (string)null,
                    AssemblyName = assemblyName,
                }
            }).OrderBy(x => x.AssemblyName == assemblyName ? 0 : 1);

            foreach (var group in _includes.GroupBy(x => x.AssemblyName))
            {
                var builder = new TypeScriptModelBuilder();
                var dll = $"{TargetBinFolder}/{group.Key}.dll";
                var outFile = $"{Path.GetFullPath($"{outFolder}/{group.Key}.d.ts")}";
                var assembly = Assembly.LoadFrom(dll);

                if (group.Key == assemblyName)
                {
                    var types = assembly.GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
                    builder.CacheTypes(types);
                }

                var includeTypes = group.Select(include =>
                {
                    if (!include.TypeString.IsNullOrWhiteSpace())
                    {
                        var type = assembly.GetType(include.TypeName);
                        if (type == null) Console.Error.WriteLine($"Can not resolve: {include.TypeString}");
                        return type;
                    }
                    else return null;
                }).Where(x => x != null).ToArray();
                builder.CacheTypes(includeTypes);

                builder.WriteTo(outFile);

                Console.WriteLine($"File saved: {outFile}");
            }
        }

    }
}
