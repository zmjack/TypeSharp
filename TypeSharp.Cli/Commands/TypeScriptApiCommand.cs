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
  {"-i|--include",20}{"\t"}Specify the include other types, such as 'Ajax.JSend,JSend.dll'.
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

        private static void GenerateTypeScript(string outFolder, string[] includes, string[] relatives, string uri)
        {
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var targetAssemblyName = Program.ProjectInfo.AssemblyName;
            var targetAssembly = Assembly.LoadFrom($"{TargetBinFolder}/{targetAssemblyName}.dll");
            AppDomain.CurrentDomain.AssemblyResolve += GAC.CreateAssemblyResolver(Program.ProjectInfo.TargetFramework, GACFolders.All);

            var typeDefs = includes
                .Select(typeString => new ClrTypeDefinition(TargetBinFolder, typeString, targetAssemblyName))
                .Concat(new[] { new ClrTypeDefinition { AssemblyName = targetAssemblyName } })
                .OrderBy(x => x.AssemblyName == targetAssemblyName ? 0 : 1);

            var relativeDefs = relatives
                .Select(relative =>
                {
                    if (relative.Count(";") == 1)
                    {
                        var pair = relative.Split(";");
                        return new
                        {
                            new ClrTypeDefinition(TargetBinFolder, pair[0], targetAssemblyName).Type,
                            TypeName = pair[1],
                        };
                    }
                    else
                    {
                        Console.Error.WriteLine("The 'relative' parameter must contain a semicolon(;).");
                        return null;
                    }
                })
                .Where(x => x != null)
                .ToArray();

            foreach (var group in typeDefs.GroupBy(x => x.AssemblyName))
            {
                var builder = new TypeScriptApiBuilder(uri);
                var outFile = $"{Path.GetFullPath($"{outFolder}/{group.Key}@Api.ts")}";

                foreach (var relative in relativeDefs)
                    builder.AddDeclaredType(relative.Type, relative.TypeName);

                if (group.Key == targetAssemblyName)
                {
                    var types = targetAssembly.GetTypesWhichMarkedAs<TypeScriptApiAttribute>();
                    builder.CacheTypes(types);
                }

                var includeTypes = group.Select(x => x.Type).Where(x => x != null).ToArray();
                builder.CacheTypes(includeTypes);

                builder.WriteTo(outFile);

                Console.WriteLine($"File saved: {outFile}");
            }
        }

    }
}
