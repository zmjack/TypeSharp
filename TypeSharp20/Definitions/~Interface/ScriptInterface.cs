using Microsoft.Extensions.Caching.Memory;
using NStandard;
using System;
using System.Text;
using System.Threading.Tasks;
using TypeSharp.Build;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial record ScriptInterface : IComment, IGenericGenerable, IEncodable, INameable
    {
        public static readonly ScriptInterface Generator = new("Generator", new[]
        {
            new ScriptGeneric("T", ScriptType.Unknown),
            new ScriptGeneric("TReturn", ScriptType.Any),
            new ScriptGeneric("TNext", ScriptType.Unknown),
        });

        public string Namespace { get; }
        public string Name { get; }
        public QualifiedName FullName => QualifiedName.Combine(Namespace, $"{Name}{(GenericArguments is not null ? $"<{GenericArguments.Select(x => x.Name).Join(", ")}>" : string.Empty)}");
        public ScriptGeneric[] Generics { get; set; }
        public ScriptType[] GenericArguments { get; set; }

        public ScriptInterface MakeGenericInterface(params ScriptType[] arguments)
        {
            if (Generics is null) throw new InvalidOperationException("No generic arguments definition.");

            var least = Generics.TakeWhile(x => x.Default is null).Count();
            var most = Generics.Length;
            var inputLength = arguments.Length;

            if (inputLength < least) throw new ArgumentException($"Too less arguments. (At least {least} arguments are required.)", nameof(arguments));
            if (inputLength > Generics.Length) throw new ArgumentException($"Too more arguments specified. (At most {most} arguments are required.)", nameof(arguments));

            var types = Generics.Select(x => x.Default).ToArray();
            foreach (var (index, type) in arguments.AsIndexValuePairs())
            {
                types[index] = type;
            }

            return this with
            {
                GenericArguments = types,
            };
        }

        public string Comment { get; set; }
        public ScriptType[] ExtendedInterfaces { get; set; }

        public Field[] Fields { get; set; } = Array.Empty<Field>();
        public Property[] Properties { get; set; } = Array.Empty<Property>();

        public ScriptInterface(string name)
        {
            Name = name;
        }
        public ScriptInterface(string name, params ScriptGeneric[] generics)
        {
            Name = name;
            Generics = generics;
        }

        public ScriptInterface Extends(params ScriptType[] extends)
        {
            ExtendedInterfaces = extends ?? Array.Empty<ScriptType>();
            return this;
        }

        public string Encode(Indent indent, string ownerPrefix)
        {
            if (Name.IsNullOrWhiteSpace()) throw new InvalidOperationException($"{nameof(Name)} is required.");

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}interface {Name}{(Generics is not null ? $"<{Generics.Select(x => x.Name).Join(", ")}>" : string.Empty)}");
            sb.AppendLine($"{indent}{{");

            if (Fields is not null)
            {
                foreach (var property in Fields)
                {
                    var propertyType = property.Type.FullName.GetSimplifiedName(ownerPrefix);
                    sb.AppendLine($"{indent.Tab()}{property.Name}: {propertyType};");
                }
            }

            sb.AppendLine($"{indent}}}");

            return sb.ToString();
        }
    }
}