using DotNetCli;
using Ink;
using NStandard;
using NStandard.Reference;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TypeSharp.Cli
{
    [Command("TSApi", "tsapi", Description = "Generate TypeScript api class from CSharp class.")]
    public class TypeScriptApiCommand : ICommand
    {
        private static readonly string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet ts (tsapi) [Options]

Options:
  {"-o|--out",20}{"\t"}Specify the output directory path. (default: Typings)
  {"-i|--include",20}{"\t"}Specify to include other built-in models, such as 'JSend'.
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
            AppDomain.CurrentDomain.AssemblyResolve += GAC.CreateAssemblyResolver(Program.ProjectInfo.TargetFramework, GACFolders.All);

            var _includes = includes.Select(include =>
            {
                if (include.Count(",") == 1)
                {
                    var parts = include.Split(",");
                    return new
                    {
                        String = include,
                        TypeName = parts[0],
                        AssemblyName = parts[1],
                    };
                }
                else
                {
                    return new
                    {
                        String = include,
                        TypeName = include,
                        AssemblyName = assemblyName,
                    };
                }
            });

            foreach (var group in _includes.GroupBy(x => x.AssemblyName))
            {
                var builder = new TypeScriptApiBuilder();
                var dll = $"{TargetBinFolder}/{group.Key}.dll";
                var outFile = $"{Path.GetFullPath($"{outFolder}/{group.Key}.ts")}";
                var assembly = Assembly.LoadFrom(dll);

                var includeTypes = group.Select(include =>
                {
                    var type = assembly.GetType(include.TypeName);
                    if (type == null) Console.Error.WriteLine($"Can not resolve: {include.String}");

                    return type;
                }).ToArray();

                builder.CacheTypes(includeTypes);

                if (group.Key == assemblyName)
                {
                    var types = assembly.GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
                    builder.CacheTypes(types);
                }
                builder.WriteTo(outFile);

                Console.WriteLine($"File saved: {outFile}");
            }
        }

    }
}
