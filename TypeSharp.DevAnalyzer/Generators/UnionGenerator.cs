using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace TypeSharp.DevAnalyzer.Generators;

[Generator]
public class UnionGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        Config.Debugger();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var typeWriter = new CodeWriter(context);
        var keywords = XmlUtil.GetKeywords(context);
        var unions = XmlUtil.GetUnions(context);

        var indent = new Indent();
        var codeBuilder = new StringBuilder();
        foreach (var union in unions)
        {
            codeBuilder.Clear();
            typeWriter.Write(codeBuilder, indent, union.TypeName, @params =>
            {
                codeBuilder.AppendLine(
                    $"""
                    {@params.Indent}/// <summary>
                    {@params.Indent}/// Possible classes:{string.Join($" |",
                        from type in union.PossiableTypes select $"\r\n{@params.Indent}/// <see cref=\"{type.Name}\" />")}
                    {@params.Indent}/// </summary>
                    {@params.Indent}public partial interface {@params.Name} : INode
                    {@params.Indent}{"{"}
                    {@params.Indent}{"}"}
                    """
                );
            });

            var code = codeBuilder.ToString();
            context.AddSource($"union-{union.TypeName}.g.cs", code);
        }

        var impls_by_type =
        (
            from pair in
                from impl in unions
                from typePair in impl.PossiableTypes
                select new
                {
                    Implement = impl.TypeName,
                    TypePair = typePair,
                }
            group pair by pair.TypePair
        ).ToDictionary(x => x.Key, x => x.Select(x => x.Implement).ToArray());
        foreach (var type in impls_by_type)
        {
            codeBuilder.Clear();

            var construtctType = type.Key.ConstrutctType;
            var name = type.Key.Name;

            typeWriter.Write(codeBuilder, indent, name, @params =>
            {
                codeBuilder.AppendLine(
                    $"""
                    {@params.Indent}public partial {construtctType switch
                    {
                        ConstrutctType.Interface => "interface",
                        ConstrutctType.Class => "class",
                        ConstrutctType.Strcut => "struct",
                        _ => "<error>",
                    }} {name}{(type.Value.Any() ? $" :\r\n{string.Join(",\r\n",
                        from impl in type.Value
                        let name = impl.Trim()
                        where !name.Contains(" ")
                        select $"{@params.Indent + 1}{name}"
                    )}" : "")}
                    {@params.Indent}{"{"}
                    {@params.Indent}{"}"}
                    """
                );
            });

            var code = codeBuilder.ToString();
            context.AddSource($"impl-{name}.g.cs", code);
        }
    }
}
