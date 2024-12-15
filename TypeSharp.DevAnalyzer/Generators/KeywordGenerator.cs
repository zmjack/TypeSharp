using Microsoft.CodeAnalysis;
using System.Text;

namespace TypeSharp.DevAnalyzer.Keywords;

[Generator]
public class KeywordGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        Config.Debugger();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var typeWriter = new CodeWriter(context);
        var keywords = XmlUtil.GetKeywords(context);

        foreach (var keyword in keywords)
        {
            var codeBuilder = new StringBuilder();
            typeWriter.WriteNamespaces(codeBuilder, new Indent(), Config.RootNamespace.Split('.'), indent =>
            {
                codeBuilder.AppendLine(
                    $"""
                    {indent}public partial class {keyword.ClassName} : INode, IKeyword<{keyword.ClassName}>
                    {indent}{"{"}
                    {indent}    public static {keyword.ClassName} Default {"{"} get; {"}"} = new {keyword.ClassName}();
                    {indent}
                    {indent}    public SyntaxKind Kind => SyntaxKind.{keyword.ClassName};
                    {indent}
                    {indent}    public string GetText()
                    {indent}    {"{"}
                    {indent}        return "{keyword.Name}";
                    {indent}    {"}"}
                    {indent}{"}"}
                    """
                );
            });
            var code = codeBuilder.ToString();
            context.AddSource($"{keyword.ClassName}.g.cs", code);
        }
    }
}
