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
    [Command("TSGenerator", Abbreviation = "tsg", Description = "Generate TypeScript model from CSharp class.")]
    public class TypeScriptGeneratorCommand : Command
    {
        private static readonly string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public TypeScriptGeneratorCommand(CmdContainer container, string[] args) : base(container, args) { }

        [CmdProperty("out", Abbreviation = "o", Description = "Specify the output directory path.")]
        public string OutFolder { get; set; } = ".";

        [CmdProperty("include", Abbreviation = "i", Description = "Specify the include other types, such as 'Ajax.JSend,JSend'.")]
        public string[] Includes { get; set; } = new string[0];

        [CmdProperty("relative", Abbreviation = "r", Description = "Treat a specified type as a defined type.")]
        public string[] Relatives { get; set; } = new string[0];

        [CmdProperty("names", Abbreviation = "n", Description = "Include original names of properties.")]
        public bool GenerateNames { get; set; } = false;

        public override void Run()
        {
            var targetAssemblyName = Program.ProjectInfo.AssemblyName;
            var assemblyContext = new AssemblyContext($"{TargetBinFolder}/{targetAssemblyName}.dll", DotNetFramework.Parse(Program.ProjectInfo.TargetFramework));

            string outFile;
            if (Directory.Exists(OutFolder))
            {
                outFile = Path.GetFullPath($"{OutFolder}{targetAssemblyName}.api.ts");
            }
            else if (OutFolder.Last().For(c => c == '/' || c == '\\'))
            {
                // if Directory
                if (!Directory.Exists(OutFolder)) Directory.CreateDirectory(OutFolder);
                outFile = Path.GetFullPath($"{OutFolder}{targetAssemblyName}.api.ts");
            }
            else
            {
                // if File
                var dir = Path.GetDirectoryName(OutFolder);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                outFile = Path.GetFullPath(OutFolder);
            }

            var builder = new TypeScriptModelBuilder();
            var markAttr = assemblyContext.GetType($"{nameof(TypeSharp)}.{nameof(TypeScriptModelAttribute)},{nameof(TypeSharp)}");
            var modelTypes = assemblyContext.RootAssembly.GetTypesWhichMarkedAs(markAttr);
            builder.CacheTypes(modelTypes);

            var includeTypes = Includes.Select(include => assemblyContext.GetType(include)).ToArray();
            builder.CacheTypes(includeTypes);

            foreach (var relative in Relatives)
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
                OutputNames = GenerateNames,
            });

            Console.WriteLine($"File saved: {outFile}");
        }

    }
}
