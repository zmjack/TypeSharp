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
    [Command("TSApi", "tsapi", Description = "Generate TypeScript api class from CSharp class.")]
    public class TypeScriptApiCommand : ICommand
    {
        private static readonly string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet ts (tsapi) [Options]

Options:
  {"-o|--out",20}{"\t"}Specify the output directory or file path.
  {"-i|--include",20}{"\t"}Specify the include other types, such as 'Ajax.JSend,JSend.dll'.
  {"-r|--relative",20}{"\t"}Treat a specified type as a defined type.
  {"-u|--uri",20}{"\t"}Specify the root uri of apis.
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
            var uri = conArgs["-u"].Concat(conArgs["--uri"]).FirstOrDefault() ?? "";

            GenerateTypeScript(outFolder, includes, relatives, uri);
        }

        private static void GenerateTypeScript(string outParam, string[] includes, string[] relatives, string uri)
        {
            var targetAssemblyName = Program.ProjectInfo.AssemblyName;
            var assemblyContext = new AssemblyContext($"{TargetBinFolder}/{targetAssemblyName}.dll", DotNetFramework.Parse(Program.ProjectInfo.TargetFramework));

            string outFile;
            if (outParam.Last().For(c => c == '/' || c == '\\'))
            {
                // if Directory
                if (!Directory.Exists(outParam)) Directory.CreateDirectory(outParam);
                outFile = Path.GetFullPath($"{outParam}{targetAssemblyName}.api.ts");
            }
            else
            {
                // if File
                var dir = Path.GetDirectoryName(outParam);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                outFile = Path.GetFullPath(outParam);
            }

            var builder = new TypeScriptApiBuilder(uri);
            var markAttr = assemblyContext.GetType($"{nameof(TypeSharp)}.{nameof(TypeScriptApiAttribute)},{nameof(TypeSharp)}");
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

            builder.WriteTo(outFile);

            Console.WriteLine($"File saved: {outFile}");
        }

    }
}
