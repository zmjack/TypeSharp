using DotNetCli;
using NStandard;
using NStandard.Runtime;
using System;
using System.IO;
using System.Linq;

namespace TypeSharp.Cli
{
    [Command("TSApi", Abbreviation = "tsapi", Description = "Generate TypeScript api class from CSharp class.")]
    public class TypeScriptApiCommand : Command
    {
        private static ProjectInfo Project => Program.CmdContainer.ProjectInfo;
        private static readonly string TargetBinFolder = Path.GetFullPath($"{Project.ProjectRoot}/bin/Debug/{Project.TargetFramework}");

        public TypeScriptApiCommand(CmdContainer container, string[] args) : base(container, args) { }

        [CmdProperty("out", Abbreviation = "o", Description = "Specify the output directory path. (default: Typings)")]
        public string Out { get; set; } = ".";

        [CmdProperty("include", Abbreviation = "i", Description = "Specify the include other types, such as 'Ajax.JSend,JSend'.")]
        public string[] Includes { get; set; } = new string[0];

        [CmdProperty("relative", Abbreviation = "r", Description = "Treat a specified type as a defined type.")]
        public string[] Relatives { get; set; } = new string[0];

        [CmdProperty("uri", Abbreviation = "u", Description = "Specify the root uri of apis.")]
        public string Uri { get; set; } = "";

        [CmdProperty("package", Abbreviation = "p", Description = "Specify the PackageName for code.")]
        public string PackageName { get; set; } = "type-sharp";

        public override void Run()
        {
            var targetAssemblyName = Project.AssemblyName;
            var assemblyContext = new AssemblyContext(DotNetFramework.Parse(Project.TargetFramework), Project.Sdk);
            assemblyContext.LoadMain($"{TargetBinFolder}/{targetAssemblyName}.dll");

            string outFile;
            // if Directory
            if (Directory.Exists(Out) || Path.GetExtension(Out) == "" || Out.Last().For(c => c == '/' || c == '\\'))
            {
                if (!Directory.Exists(Out)) Directory.CreateDirectory(Out);
                outFile = Path.GetFullPath(Path.Combine(Out, $"{targetAssemblyName}.api.ts"));
            }
            // if File
            else
            {
                var dir = Path.GetDirectoryName(Out);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                outFile = Path.GetFullPath(Out);
            }

            var builder = new TypeScriptApiBuilder(Uri);
            var markAttr = assemblyContext.GetType($"{nameof(TypeSharp)}.{nameof(TypeScriptApiAttribute)},{nameof(TypeSharp)}");
            var modelTypes = assemblyContext.MainAssembly.GetTypesWhichMarkedAs(markAttr);
            builder.CacheTypes(modelTypes);

            foreach (var include in Includes)
            {
                var type = assemblyContext.GetType(include);
                if (type == null) throw new ArgumentException($"Can not find type({type.FullName}).");
                builder.CacheType(type);
            }

            foreach (var relative in Relatives)
            {
                if (relative.Count(";") == 1)
                {
                    var pair = relative.Split(";");
                    builder.AddDeclaredType(assemblyContext.GetType(pair[0]), pair[1]);
                }
                else throw new ArgumentException("Each parameter('Relative') must contain a semicolon(;).");
            }

            builder.WriteTo(outFile, PackageName);

            Console.WriteLine($"File saved: {outFile}");
        }

    }
}
