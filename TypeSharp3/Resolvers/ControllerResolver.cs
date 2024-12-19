using System.Diagnostics;
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
    [DebuggerDisplay("{Verb}: {Name}")]
    public struct Action(string verb, string name)
    {
        public string Verb { get; set; } = verb;
        public string Name { get; set; } = name;
    }

    public string? BaseAddress { get; set; }

    private static readonly string[] _routes =
    [
        "Microsoft.AspNetCore.Mvc.RouteAttribute",
    ];
    private static readonly string _defaultRouteTemplate = "[Controller]/{Action}";
    private static readonly string[] _controllers =
    [
        "Microsoft.AspNetCore.Mvc.Controller",
        "Microsoft.AspNetCore.Mvc.ControllerBase"
    ];
    private static readonly Verb[] _verbs =
    [
        new Verb("GET", "Microsoft.AspNetCore.Mvc.HttpGetAttribute"),
        new Verb("POST", "Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
        new Verb("PUT", "Microsoft.AspNetCore.Mvc.HttpPutAttribute"),
        new Verb("DELETE", "Microsoft.AspNetCore.Mvc.HttpDeleteAttribute"),
        new Verb("OPTIONS", "Microsoft.AspNetCore.Mvc.HttpOptionsAttribute"),
        new Verb("HEAD", "Microsoft.AspNetCore.Mvc.HttpHeadAttribute"),
        new Verb("PATCH", "Microsoft.AspNetCore.Mvc.HttpPatchAttribute"),
    ];
    private static readonly string _fromBody = "Microsoft.AspNetCore.Mvc.FromBodyAttribute";
    private static readonly string _fileResult = "Microsoft.AspNetCore.Mvc.FileResult";
    private static readonly string[] _actionResults =
    [
        "Microsoft.AspNetCore.Mvc.IActionResult",
        "Microsoft.AspNetCore.Mvc.ActionResult",
    ];

    private static Action[] GetActions(MethodInfo method)
    {
        var attrs = method.GetCustomAttributes();
        Action[] actions =
        [
            ..
            from verb in _verbs
            let attr = attrs.FirstOrDefault(x => x.GetType().FullName == verb.Attribute)
            where attr is not null
            let name = attr.GetType().GetProperty("Name")?.GetValue(attr) as string ?? method.Name
            select new Action(verb.Name, name)
        ];
        if (actions.Length == 0)
        {
            actions = [new Action("GET", method.Name)];
        }
        return actions;
    }
    private static string[] GetRouteTemplates(ICustomAttributeProvider method)
    {
        var attrs = method.GetCustomAttributes(false);
        string[] templates =
        [
            ..
            from attr in method.GetCustomAttributes(false)
            where _routes.Contains(attr.GetType().FullName)
            let template_prop = attr.GetType().GetProperty("Template")
            where template_prop is not null
            select template_prop.GetValue(attr) as string
        ];
        return templates;
    }

    [GeneratedRegex(@"[\{\[][Cc]ontroller(?:\s*=[^\}\]]+)?[\}\]]")]
    private static partial Regex GetRouteControllerRegex();

    [GeneratedRegex(@"[\{\[][Aa]ction(?:\s*=[^\}\]]+)?[\}\]]")]
    private static partial Regex GetRouteActionRegex();

    private string GetUri(MethodInfo method, Action action)
    {
        var templates = GetRouteTemplates(method);
        if (templates.Length == 0)
        {
            templates = GetRouteTemplates(method.DeclaringType!);
        }
        var template = templates.Length != 0
            ? templates.First()
            : _defaultRouteTemplate;

        var controller = method.DeclaringType!.Name;
        if (controller.EndsWith("Controller")) controller = controller[..^10];

        var uri = template;
        uri = GetRouteControllerRegex().Replace(uri, controller);
        uri = GetRouteActionRegex().Replace(uri, action.Name);

        return BaseAddress is not null ? $"{BaseAddress}/{uri}" : uri;
    }

    private bool CanResolve(Type type)
    {
        return type.BaseType is not null && _controllers.Contains(type.BaseType.FullName);
    }

    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        if (CanResolve(type))
        {
            general = new Lazy<IGeneralType>(() => new TypeReference(type.Name));
            declaration = new Lazy<IDeclaration>(() =>
            {
                var declaration = new ClassDeclaration([ExportKeyword.Default], type.Name);
                var members = new List<ClassDeclaration.IMember>();

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    var methodName = method.Name;
                    if (Parser.CamelCase) methodName = StringEx.CamelCase(methodName);

                    var methodParams = (
                        from p in method.GetParameters()
                        let attrs = p.GetCustomAttributes()
                        select new
                        {
                            InBody = attrs.Any(x => x.GetType().FullName == _fromBody),
                            Paramter = new Parameter(p.Name!, Parser.GetOrCreateGeneralType(p.ParameterType))
                        }
                    ).ToArray();
                    var actions = GetActions(method);
                    var action = actions.First();
                    var uri = GetUri(method, action);

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

                    if (action.Verb is "GET" or "POST" or "PUT" or "DELETE" or "OPTIONS" or "HEAD" or "PATCH")
                    {
                        var rawBuilder = new StringBuilder();
                        rawBuilder.Append(
                            $"""
                            return await fetch(`{uri}{(string.IsNullOrEmpty(query) ? "" : $"?{query}")}`, {"{"}
                              method: '{action.Verb}'
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

                        IGeneralType returnType;
                        var returnFullName = method.ReturnType.FullName;
                        if (_actionResults.Any(name => returnFullName == name))
                        {
                            returnType = TypeReference.Promise(AnyKeyword.Default);
                        }
                        else if (returnFullName == _fileResult)
                        {
                            returnType = TypeReference.Promise(VoidKeyword.Default);
                            rawBuilder.AppendLine(
                                $"""
                                  $ts_save(await response.blob(), $ts_hcd(response.headers['Content-Disposition']) ?? 'file');
                                """);
                        }
                        else
                        {
                            var _returnType = TypeReference.Promise(Parser.GetOrCreateGeneralType(method.ReturnType));
                            if (_returnType.TypeArguments[0].Kind == SyntaxKind.VoidKeyword)
                            {
                                rawBuilder.AppendLine(
                                    $"""
                                      // do nothing
                                    """);
                            }
                            else
                            {
                                rawBuilder.AppendLine(
                                    $"""
                                      return await response.json() as {_returnType.GetText()};
                                    """);
                            }
                            returnType = _returnType;
                        }

                        rawBuilder.Append("});");

                        var methodDeclaration = new MethodDeclaration(
                            [AsyncKeyword.Default],
                            methodName,
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
                            methodName,
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

        general = null;
        declaration = null;
        return false;
    }
}
