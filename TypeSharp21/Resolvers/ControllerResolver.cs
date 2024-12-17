using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public partial class ControllerResolver : Resolver
{
    private struct Verb(string name, string attribute)
    {
        public string Name { get; set; } = name;
        public string Attribute { get; set; } = attribute;
    }

    public string? BaseAddress { get; set; }

    private static readonly string[] _routes =
    [
        "Microsoft.AspNetCore.Mvc.RouteAttribute",
    ];
    private static readonly string _defaultRouteTemplate = "[Controller]/{Action=Index}";
    private static readonly string[] _controllers =
    [
        "Microsoft.AspNetCore.Mvc.Controller",
        "Microsoft.AspNetCore.Mvc.ControllerBase"
    ];
    private static readonly Verb[] _verbs =
    [
        new Verb("get", "Microsoft.AspNetCore.Mvc.HttpGetAttribute"),
        new Verb("post", "Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
        new Verb("put", "Microsoft.AspNetCore.Mvc.HttpPutAttribute"),
        new Verb("delete", "Microsoft.AspNetCore.Mvc.HttpDeleteAttribute"),
        new Verb("options", "Microsoft.AspNetCore.Mvc.HttpOptionsAttribute"),
        new Verb("head", "Microsoft.AspNetCore.Mvc.HttpHeadAttribute"),
        new Verb("patch", "Microsoft.AspNetCore.Mvc.HttpPatchAttribute"),
    ];
    private static Verb DefaultVerb => _verbs[0];
    private static readonly string _fromBody = "Microsoft.AspNetCore.Mvc.FromBodyAttribute";

    private static Verb[] GetMethodVerbs(MethodInfo method)
    {
        var attrs = method.GetCustomAttributes();
        Verb[] verbs =
        [
            ..
            from verb in _verbs
            where attrs.Any(x => x.GetType().FullName == verb.Attribute)
            select verb
        ];
        return verbs;
    }
    private static string[] GetRouteTemplates(MethodInfo method)
    {
        var attrs = method.GetCustomAttributes();
        string[] templates =
        [
            ..
            from attr in method.GetCustomAttributes()
            where _routes.Contains(attr.GetType().FullName)
            let template_prop = attr.GetType().GetProperty("Template")
            where template_prop is not null
            select template_prop.GetValue(attr) as string
        ];
        return templates;
    }

    [GeneratedRegex(@"[\{\[][Cc]ontroller[\}\]]|[\{\[][Cc]ontroller\s*=[^\}\]]+[\}\]]")]
    private static partial Regex GetRouteControllerRegex();

    [GeneratedRegex(@"[\{\[][Aa]ction[\}\]]|[\{\[][Aa]ction\s*=[^\}\]]+[\}\]]")]
    private static partial Regex GetRouteActionRegex();

    private string GetUri(MethodInfo method)
    {
        var templates = GetRouteTemplates(method);
        var template = templates.Length != 0 ? templates.First() : _defaultRouteTemplate;

        var controller = method.DeclaringType!.Name;
        if (controller.EndsWith("Controller")) controller = controller.Substring(0, controller.Length - 10);

        var action = method.Name;
        var uri = template;
        uri = GetRouteControllerRegex().Replace(uri, controller);
        uri = GetRouteActionRegex().Replace(uri, action);

        return BaseAddress is not null ? $"{BaseAddress}/{uri}" : uri;
    }

    private bool CanResolve(Type type)
    {
        return type.BaseType is not null && _controllers.Contains(type.BaseType.FullName);
    }

    protected override bool TryResolve(Type type, out Lazy<IStatement>? statement)
    {
        if (CanResolve(type))
        {
            statement = new Lazy<IStatement>(() =>
            {
                var declaration = new ClassDeclaration(type.Name);
                var members = new List<ClassDeclaration.IMember>();

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    var methodParams = (
                        from p in method.GetParameters()
                        let attrs = p.GetCustomAttributes()
                        select new
                        {
                            InBody = attrs.Any(x => x.GetType().FullName == _fromBody),
                            Paramter = new Parameter(p.Name!, Parser.GetOrCreateGeneralType(p.ParameterType))
                        }
                    ).ToArray();
                    var verbs = GetMethodVerbs(method);
                    var verb = verbs.Length != 0 ? verbs.First() : DefaultVerb;
                    var uri = GetUri(method);

                    var query = string.Join("&",
                        from p in methodParams
                        where !p.InBody
                        let name = p.Paramter.Name.GetText()
                        select $"{name}=${{{name}}}"
                    );
                    var body =
                    (
                        from p in methodParams
                        where p.InBody
                        let name = p.Paramter
                        select name
                    ).FirstOrDefault();

                    if (verb.Name is "get" or "post" or "put" or "patch")
                    {
                        var returnType = Parser.GetOrCreateGeneralType(method.ReturnType);
                        var rawBuilder = new StringBuilder();

                        rawBuilder.Append(
                            $"""
                            return await fetch(`{uri}{(string.IsNullOrEmpty(query) ? "" : $"?{query}")}`, {"{"}
                              method: '{verb.Name.ToUpper()}'
                            """);

                        if (body is not null)
                        {
                            rawBuilder.AppendLine(
                                $"""
                                ,
                                  body: JSON.stringify({body.Name.GetText()})
                                {"}"}).then(async response => {"{"}
                                """);
                        }
                        else
                        {
                            rawBuilder.AppendLine(
                                $"""

                                {"}"}).then(async response => {"{"}
                                """);
                        }

                        if (returnType.Kind == SyntaxKind.VoidKeyword)
                        {
                            rawBuilder.AppendLine(
                                $"""
                                  await response.json();
                                """);
                        }
                        else
                        {
                            rawBuilder.AppendLine(
                                $"""
                                  return await response.json() as {returnType.GetText()};
                                """);
                        }

                        rawBuilder.Append("}");

                        var methodDeclaration = new MethodDeclaration(
                            [AsyncKeyword.Default],
                            method.Name,
                            [.. from x in methodParams select x.Paramter],
                            returnType
                        )
                        {
                            Body = new Block()
                            {
                                Statements =
                                [
                                    new RawText(rawBuilder.ToString()),
                                ],
                            },
                        };
                        members.Add(methodDeclaration);
                    }
                    else
                    {
                        var methodDeclaration = new MethodDeclaration(
                            method.Name,
                            [.. from x in methodParams select x.Paramter]
                        )
                        {
                            Body = new Block()
                            {
                                Statements =
                                [
                                    new ReturnStatement(new StringLiteral("undefined")),
                                ],
                            },
                        };
                        members.Add(methodDeclaration);
                    }
                }
                declaration.Members = [.. members];

                return declaration;
            });
            return true;
        }

        statement = null;
        return false;
    }
}
