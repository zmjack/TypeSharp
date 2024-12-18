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
    private static readonly string _fileResult = "Microsoft.AspNetCore.Mvc.FileResult";
    private static readonly string[] _actionResults =
    [
        "Microsoft.AspNetCore.Mvc.IActionResult",
        "Microsoft.AspNetCore.Mvc.ActionResult",
    ];

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

    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        if (CanResolve(type))
        {
            general = new Lazy<IGeneralType>(() => new TypeReference(type.Name));
            declaration = new Lazy<IDeclaration>(() =>
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

                    if (verb.Name is "get" or "post" or "put" or "delete" or "patch")
                    {
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

                        IGeneralType returnType;
                        var returnFullName = method.ReturnType.FullName;
                        if (_actionResults.Any(name => returnFullName == name))
                        {
                            returnType = TypeReference.Promise([AnyKeyword.Default]);
                        }
                        else if (returnFullName == _fileResult)
                        {
                            returnType = TypeReference.Promise([VoidKeyword.Default]);
                            rawBuilder.AppendLine(
                                $"""
                                  var filename = tsharp_get_filename(response.headers['content-disposition']);
                                  tsharp_save_blob(await response.blob(), filename);
                                """);
                        }
                        else
                        {
                            var _returnType = TypeReference.Promise([Parser.GetOrCreateGeneralType(method.ReturnType)]);
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

        general = null;
        declaration = null;
        return false;
    }
}
