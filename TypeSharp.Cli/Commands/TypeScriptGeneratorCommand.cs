using DotNetCli;
using Ink;
using NStandard;
using NStandard.Reference;
using NStandard.Runtime;
using System;
using System.IO;
using System.Linq;

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
  {"-r|--relative",20}{"\t"}Treat a specified type as a defined type.
  {"-n|--names",20}{"\t"}Include original names of properties.
");
        }

        public void Run(string[] args)
        {
            var conArgs = new ConArgs(args, "-");
            if (conArgs["-h"].Concat(conArgs["--help"]).Any())
            {
                PrintUsage();
                return;
            }

            var outFolder = conArgs["-o"].Concat(conArgs["--out"]).FirstOrDefault() ?? ".";
            var includes = conArgs["-i"].Concat(conArgs["--include"]).ToArray();
            var relatives = conArgs["-r"].Concat(conArgs["--relative"]).ToArray();
            var outputNames = conArgs["-n"].Concat(conArgs["--names"]).Any();

            GenerateTypeScript(outFolder, includes, relatives, outputNames);
        }

        private static void GenerateTypeScript(string outFolder, string[] includes, string[] relatives, bool outputNames)
        {
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var targetAssemblyName = Program.ProjectInfo.AssemblyName;
            var assemblyContext = new AssemblyContext($"{TargetBinFolder}/{targetAssemblyName}.dll", DotNetFramework.Parse(Program.ProjectInfo.TargetFramework));

            var builder = new TypeScriptModelBuilder();
            var outFile = $"{Path.GetFullPath($"{outFolder}/{targetAssemblyName}.d.ts")}";

            var markAttr = assemblyContext.GetType($"{nameof(TypeSharp)}.{nameof(TypeScriptModelAttribute)},{nameof(TypeSharp)}");
            var modelTypes = assemblyContext.RootAssembly.GetTypesWhichMarkedAs(markAttr);
            builder.CacheTypes(modelTypes);

            var includeTypes = includes.Select(include => assemblyContext.GetType(include)).ToArray();
            builder.CacheTypes(includeTypes);

            foreach (var relative in relatives)
            {
                if (relative.Count(";") == 1)
                {
                    var pair = relative.Split(";");
                    builder.AddDeclaredType(assemblyContext.GetType(pair[0]), pair[1]);
                }
                else Console.Error.WriteLine("The 'relative' parameter must contain a semicolon(;).");
            }

            builder.WriteTo(outFile, new CompileOptions
            {
                OutputNames = outputNames,
            });

            Console.WriteLine($"File saved: {outFile}");
        }

    }
}
