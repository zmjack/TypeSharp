using DotNetCli;
using NStandard;
using NStandard.Runtime;
using System;
using System.IO;
using System.Linq;

namespace TypeSharp.Cli
{
    [Command("TSGenerator", Abbreviation = "tsg", Description = "Generate TypeScript model from CSharp class.")]
    public class TypeScriptGeneratorCommand : Command
    {
        public TypeScriptGeneratorCommand(CmdContainer container, string[] args) : base(container, args) { }

        [CmdProperty("out", Abbreviation = "o", Description = "Specify the output directory path.")]
        public string Out { get; set; } = ".";

        [CmdProperty("include", Abbreviation = "i", Description = "Specify the include other types, such as 'Ajax.JSend,JSend'.")]
        public string[] Includes { get; set; } = Array.Empty<string>();

        [CmdProperty("relative", Abbreviation = "r", Description = "Treat a specified type as a defined type.")]
        public string[] Relatives { get; set; } = Array.Empty<string>();

        [CmdProperty("names", Abbreviation = "n", Description = "Include original names of properties.")]
        public bool GenerateNames { get; set; } = false;

        [CmdProperty("verbose", Abbreviation = "verb", Description = "Output more information for building.")]
        public bool Verbose { get; set; } = false;

        public override void Run()
        {
            if (Container.ProjectInfo is null) throw new InvalidOperationException("No project information.");

            var project = Container.ProjectInfo.Value;
            var targetBinFolder = Path.GetFullPath($"{project.ProjectRoot}/bin/Debug/{project.TargetFramework}");
            var targetAssemblyName = project.AssemblyName;
            var assemblyContext = new AssemblyContext(DotNetFramework.Parse(project.TargetFramework), project.Sdk);
            assemblyContext.LoadMain($"{targetBinFolder}/{targetAssemblyName}.dll");

            string outFile;
            // if Directory
            if (Directory.Exists(Out) || Path.GetExtension(Out) == "" || Out.Last().For(c => c == '/' || c == '\\'))
            {
                if (!Directory.Exists(Out)) Directory.CreateDirectory(Out);
                outFile = Path.GetFullPath(Path.Combine(Out, $"{targetAssemblyName}.ts"));
            }
            // if File
            else
            {
                var dir = Path.GetDirectoryName(Out);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                outFile = Path.GetFullPath(Out);
            }

            var builder = new TypeScriptModelBuilder(new ModelBuilderOptions
            {
                Verbose = Verbose,
            });
            var markAttr = assemblyContext.GetType($"{nameof(TypeSharp)}.{nameof(TypeScriptModelAttribute)},{nameof(TypeSharp)}");
            var modelTypes = assemblyContext.MainAssembly.GetTypesWhichMarkedAs(markAttr);
            builder.CacheTypes(modelTypes);

            foreach (var include in Includes)
            {
                var type = assemblyContext.GetType(include);
                if (type == null) throw new ArgumentException($"Can not find type({include}).");
                builder.CacheType(type);
            }

            foreach (var relative in Relatives)
            {
                if (relative.Count(";") == 1)
                {
                    var pair = relative.Split(";");
                    var type = assemblyContext.GetType(pair[0]) ?? Type.GetType(pair[0]);
                    var typescriptName = pair[1];

                    if (type == null) throw new ArgumentException($"Can not find type for the specified string({relative}).");
                    builder.AddDeclaredType(type, typescriptName);
                }
                else throw new ArgumentException("Each parameter('Relative') must contain a semicolon(;).");
            }

            builder.WriteTo(outFile, new BuildOptions
            {
                OutputNames = GenerateNames,
            });

            Console.WriteLine($"File saved: {outFile}");
        }

    }
}
