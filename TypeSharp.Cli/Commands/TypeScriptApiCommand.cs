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
    [Command("TSApi", Abbreviation = "tsapi", Description = "Generate TypeScript api class from CSharp class.")]
    public class TypeScriptApiCommand : Command
    {
        private static readonly string TargetBinFolder = Path.GetFullPath($"{Program.ProjectInfo.ProjectRoot}/bin/Debug/{Program.ProjectInfo.TargetFramework}");

        public TypeScriptApiCommand(CmdContainer container, string[] args) : base(container, args) { }

        [CmdProperty("out", Abbreviation = "o", Description = "Specify the output directory path. (default: Typings)")]
        public string Out { get; set; } = ".";
        
        [CmdProperty("include", Abbreviation = "i", Description = "Specify the include other types, such as 'Ajax.JSend,JSend'.")]
        public string[] Includes { get; set; } = new string[0];

        [CmdProperty("relative", Abbreviation = "r", Description = "Treat a specified type as a defined type.")]
        public string[] Relatives { get; set; } = new string[0];

        [CmdProperty("uri", Abbreviation = "u", Description = "Specify the root uri of apis.")]
        public string Uri { get; set; } = "";

        public override void Run()
        {
            var targetAssemblyName = Program.ProjectInfo.AssemblyName;
            var assemblyContext = new AssemblyContext($"{TargetBinFolder}/{targetAssemblyName}.dll", DotNetFramework.Parse(Program.ProjectInfo.TargetFramework));

            string outFile;
            if (Directory.Exists(Out))
            {
                outFile = Path.GetFullPath($"{Out}{targetAssemblyName}.api.ts");
            }
            else if (Out.Last().For(c => c == '/' || c == '\\'))
            {
                // if Directory
                if (!Directory.Exists(Out)) Directory.CreateDirectory(Out);
                outFile = Path.GetFullPath($"{Out}{targetAssemblyName}.api.ts");
            }
            else
            {
                // if File
                var dir = Path.GetDirectoryName(Out);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                outFile = Path.GetFullPath(Out);
            }

            var builder = new TypeScriptApiBuilder(Uri);
            var markAttr = assemblyContext.GetType($"{nameof(TypeSharp)}.{nameof(TypeScriptApiAttribute)},{nameof(TypeSharp)}");
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
                else Console.Error.WriteLine("Each parameter('Relative') must contain a semicolon(;).");
            }

            builder.WriteTo(outFile);

            Console.WriteLine($"File saved: {outFile}");
        }

    }
}
