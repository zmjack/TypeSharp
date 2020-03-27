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

            var outFolder = conArgs["-o"] ?? conArgs["-out"] ?? "";
            var includes = conArgs["-i"]?.Split(";") ?? conArgs["--include"]?.Split(";") ?? new string[0];

            GenerateTypeScript(outFolder, includes);
        }

        private static void GenerateTypeScript(string outFolder, string[] includes)
        {
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
            foreach (var include in includes)
            {
                var builder = new TypeScriptModelBuilder();
                var fileName = $"{Path.GetFullPath($"{outFolder}/{include}.ts")}";
                Type type = null;

                if (!include.Contains(","))
                {
                    type = assembly.GetType(include);
                }
                else if (include.Count(",") == 1)
                {
                    var parts = include.Split(",");
                    var refDllPath = $"{TargetBinFolder}/{parts[1]}.dll";
                    var refAssembly = Assembly.LoadFrom(refDllPath);
                    type = refAssembly.GetType(parts[0]);
                }

                if (type != null)
                {
                    builder.CacheType(type);
                    builder.WriteTo(fileName);
                    Console.WriteLine($"File saved: {fileName}");
                }
                else Console.Error.WriteLine($"Can not resolve: {include}");
            }
            #endregion
        }

    }
}
