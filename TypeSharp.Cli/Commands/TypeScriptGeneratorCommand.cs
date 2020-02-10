using Dawnx;
using DotNetCli;
using Frontend;
using NEcho;
using NStandard;
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
        private static string ProgramFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private static string UserProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static string GetTargetDllFile(string assembly) => $"{TargetBinFolder}/{assembly}.dll";
        private static string GetDllFile(string assembly, string version)
        {
            var possibleFiles = new List<string>();
            var nugetVersion = version.For(ver =>
            {
                if (ver.EndsWith(".0"))
                    return ver.Substring(0, ver.Length - 2);
                else return ver;
            });

            possibleFiles.Add(GetTargetDllFile(assembly));
            switch (Program.ProjectInfo.TargetFramework)
            {
                case string framework when framework == "netcoreapp3.0":
                    possibleFiles.Add($"{ProgramFilesFolder}/dotnet/packs/NETStandard.Library.Ref/2.1.0/ref/netstandard2.1/{assembly}.dll");
                    goto default;

                case string framework when framework == "netcoreapp3.1":
                    possibleFiles.Add($"{ProgramFilesFolder}/dotnet/packs/NETStandard.Library.Ref/2.1.0/ref/netcoreapp3.1/{assembly}.dll");
                    possibleFiles.Add($"{ProgramFilesFolder}/dotnet/packs/Microsoft.NETCore.App.Ref/3.1.0/ref/netcoreapp3.1/{assembly}.dll");
                    possibleFiles.Add($"{ProgramFilesFolder}/dotnet/packs/Microsoft.AspNetCore.App.Ref/3.1.0/ref/netcoreapp3.1/{assembly}.dll");
                    possibleFiles.Add($"{ProgramFilesFolder}/dotnet/packs/Microsoft.WindowsDesktop.App.Ref/3.1.0/ref/netcoreapp3.1/{assembly}.dll");
                    goto default;

                default:
                    possibleFiles.AddRange(new[]
                    {
                        $"{ProgramFilesFolder}/dotnet/sdk/NuGetFallbackFolder/{assembly}/{nugetVersion}/lib/netstandard2.0/{assembly}.dll",
                        $"{UserProfileFolder}/.nuget/packages/{assembly}/{nugetVersion}/lib/netstandard2.0/{assembly}.dll",
                    });
                    break;
            }

            foreach (var file in possibleFiles)
                if (File.Exists(file)) return file;
            return null;
        }

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

            var dllPath = GetTargetDllFile(Program.ProjectInfo.AssemblyName);
            var assembly = Assembly.LoadFrom(dllPath);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

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

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var regex = new Regex("([^,]+), Version=([^,]+), Culture=[^,]+, PublicKeyToken=.+");
            var match = regex.Match(args.Name);

            var assembly = match.Groups[1].Value;
            var version = match.Groups[2].Value;

            var dll = GetDllFile(assembly, version);
            if (dll != null)
            {
                Console.WriteLine($"Loaded assembly {assembly} {version}.");
                return Assembly.LoadFile(dll);
            }
            else
            {
                Console.WriteLine($"! Can not find assembly {assembly} {version}.");
                return null;
            }
        }

    }

}
