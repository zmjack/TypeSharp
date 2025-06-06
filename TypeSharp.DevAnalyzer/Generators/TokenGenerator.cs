using Microsoft.CodeAnalysis;
using System.Text;

namespace TypeSharp.DevAnalyzer.Generators;

[Generator]
public class TokenGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        Config.Debugger();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var typeWriter = new CodeWriter(context);
        var tokens = XmlUtil.GetTokens(context);

        foreach (var token in tokens)
        {
            var codeBuilder = new StringBuilder();
            typeWriter.WriteNamespaces(codeBuilder, new Indent(0, 4), Config.RootNamespace.Split('.'), indent =>
            {
                codeBuilder.AppendLine(
                    $"""
                    {indent}public partial class {token.ClassName} : INode, IToken<{token.ClassName}>
                    {indent}{"{"}
                    {indent}    public static {token.ClassName} Default {"{"} get; {"}"} = new {token.ClassName}();
                    {indent}    
                    {indent}    protected {token.ClassName}() {"{"} {"}"}
                    {indent}    
                    {indent}    public SyntaxKind Kind => SyntaxKind.{token.ClassName};
                    {indent}    
                    {indent}    public string GetText(Indent indent = default)
                    {indent}    {"{"}
                    {indent}        return "{token.Mark}";
                    {indent}    {"}"}
                    {indent}{"}"}
                    """
                );
            });
            var code = codeBuilder.ToString();
            context.AddSource($"{token.ClassName}.g.cs", code);
        }
    }
}
