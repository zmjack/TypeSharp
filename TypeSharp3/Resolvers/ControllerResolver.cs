using System.Collections.ObjectModel;
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
    public struct Action(string verb, string name, string[]? produces = null)
    {
        public string Verb { get; set; } = verb;
        public string Name { get; set; } = name;
        public string[]? Produces { get; set; } = produces;
    }

    public string? BaseAddress { get; set; }
    public string DefaultRoute { get; set; } = "[Controller]/[Action]";

    private static readonly string[] _controllers =
    [
        "Microsoft.AspNetCore.Mvc.Controller",
        "Microsoft.AspNetCore.Mvc.ControllerBase"
    ];
    private static readonly string[] _fileResults =
    [
        "Microsoft.AspNetCore.Mvc.FileResult",
        "Microsoft.AspNetCore.Mvc.FileContentResult"
    ];
    private static readonly string[] _actionResults =
    [
        "Microsoft.AspNetCore.Mvc.IActionResult",
        "Microsoft.AspNetCore.Mvc.ActionResult",
        "Microsoft.AspNetCore.Mvc.JsonResult",
    ];
    private static readonly string _producesAttribute = "Microsoft.AspNetCore.Mvc.ProducesAttribute";
    private static readonly string _routeAttribute = "Microsoft.AspNetCore.Mvc.RouteAttribute";
    private static readonly string _fromBodyAttribute = "Microsoft.AspNetCore.Mvc.FromBodyAttribute";
    private static readonly string _fromServicesAttribute = "Microsoft.AspNetCore.Mvc.FromServicesAttribute";
    private static readonly Verb[] _verbAttributes =
    [
        new Verb("GET", "Microsoft.AspNetCore.Mvc.HttpGetAttribute"),
        new Verb("POST", "Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
        new Verb("PUT", "Microsoft.AspNetCore.Mvc.HttpPutAttribute"),
        new Verb("DELETE", "Microsoft.AspNetCore.Mvc.HttpDeleteAttribute"),
        new Verb("OPTIONS", "Microsoft.AspNetCore.Mvc.HttpOptionsAttribute"),
        new Verb("HEAD", "Microsoft.AspNetCore.Mvc.HttpHeadAttribute"),
        new Verb("PATCH", "Microsoft.AspNetCore.Mvc.HttpPatchAttribute"),
    ];

    private static Action[] GetActions(MethodInfo method)
    {
        var attrs = method.GetCustomAttributes();

        string[]? produces = null;
        var producesAttribute = attrs.FirstOrDefault(x => x.GetType().FullName == _producesAttribute);
        if (producesAttribute is not null)
        {
            produces =
            [
                ..
                (producesAttribute.GetType().GetProperty("ContentTypes")?.GetValue(producesAttribute) as Collection<string>)!
            ];
        }

        Action[] actions =
        [
            ..
            from verb in _verbAttributes
            let attr = attrs.FirstOrDefault(x => x.GetType().FullName == verb.Attribute)
            where attr is not null
            let name = attr.GetType().GetProperty("Name")?.GetValue(attr) as string ?? method.Name
            select new Action(verb.Name, name, produces)
        ];
        if (actions.Length == 0)
        {
            actions = [new Action("GET", method.Name, produces)];
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
            where _routeAttribute == attr.GetType().FullName
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
        var template = templates.Length != 0 ? templates.First() : DefaultRoute;

        var controller = method.DeclaringType!.Name;
        if (controller.EndsWith("Controller")) controller = controller[..^10];

        var route = template;
        route = GetRouteControllerRegex().Replace(route, controller);
        route = GetRouteActionRegex().Replace(route, action.Name);

        return $"{BaseAddress}/{route}";
    }

    private static bool CanResolve(Type type)
    {
        while (type.BaseType is not null)
        {
            if (_controllers.Contains(type.BaseType.FullName))
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }

    private Parameter GetParameter(ParameterInfo parameter)
    {
        var parameterName = parameter.Name!;
        if (Generator.CamelCase)
        {
            parameterName = StringEx.CamelCase(parameterName);
        }
        var parameterType = parameter.ParameterType;

        var general = Generator.GetOrCreateGeneralType(parameterType);
        return new(parameterName, general);
    }

    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        if (!CanResolve(type))
        {
            general = null;
            declaration = null;
            return false;
        }

        var typeName = type.Name;
        general = new Lazy<IGeneralType>(() =>
        {
            IIdentifier referenceName = Generator.ModuleCode != ModuleCode.None && type.Namespace is not null
                ? new QualifiedName($"{type.Namespace}.{typeName}")
                : new Identifier(typeName);
            return new TypeReference(referenceName);
        });
        declaration = new Lazy<IDeclaration>(() =>
        {
            var declaration = new ClassDeclaration([ExportKeyword.Default], typeName);
            var members = new List<ClassDeclaration.IMember>();

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodName = method.Name;
                if (Generator.CamelCase) methodName = StringEx.CamelCase(methodName);

                var methodParams = (
                    from p in method.GetParameters()
                    let attrs = p.GetCustomAttributes()
                    let fromServices = attrs.Any(x => x.GetType().FullName == _fromServicesAttribute)
                    where !fromServices
                    let fromBody = attrs.Any(x => x.GetType().FullName == _fromBodyAttribute)
                    select new
                    {
                        FromBody = fromBody,
                        Paramter = GetParameter(p)
                    }
                ).ToArray();
                var actions = GetActions(method);
                var action = actions.First();
                var uri = GetUri(method, action);

                var query = string.Join("&",
                    from p in methodParams
                    where !p.FromBody
                    let name = p.Paramter.Name.GetText()
                    select $"{name}=${{{name}}}"
                );
                var body =
                (
                    from p in methodParams
                    where p.FromBody
                    let name = p.Paramter
                    select name
                ).FirstOrDefault();

                if (action.Verb is "GET" or "POST" or "PUT" or "DELETE" or "OPTIONS" or "HEAD" or "PATCH")
                {
                    var rawBuilder = new StringBuilder();
                    rawBuilder.Append($"""
                    return fetch(`{uri}{(string.IsNullOrEmpty(query) ? "" : $"?{query}")}`, {"{"}
                      method: '{action.Verb}',
                      headers: {"{"}
                        'Content-Type': 'application/json'
                    """);

                    if (action.Produces is not null)
                    {
                        if (action.Produces.Length > 1)
                        {
                            rawBuilder.Append($"""
                            ,
                                'Accept': __accept
                            """);
                        }
                        else if (action.Produces.Length > 0)
                        {
                            rawBuilder.Append($"""
                            ,
                                'Accept': '{string.Join(", ", action.Produces)}'
                            """);
                        }
                    }

                    rawBuilder.AppendLine();
                    rawBuilder.Append($"""
                      {"}"}
                    """);

                    if (body is not null)
                    {
                        rawBuilder.AppendLine($"""
                        ,
                          body: JSON.stringify({body.Name.GetText()})
                        """);
                    }
                    else
                    {
                        rawBuilder.AppendLine();
                    }

                    rawBuilder.AppendLine($"""
                    {"}"}).then(async response => {"{"}
                    """
                    );

                    TypeReference returnGeneralType;
                    var returnType = method.ReturnType;
                    if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        returnType = returnType.GetGenericArguments()[0];
                    }
                    else if (returnType == typeof(Task))
                    {
                        returnType = typeof(void);
                    }

                    var returnFullName = returnType.FullName;
                    if (_fileResults.Contains(returnFullName))
                    {
                        returnGeneralType = TypeReference.Promise(VoidKeyword.Default);
                        rawBuilder.AppendLine("""
                          const disposition = response.headers.get('Content-Disposition');
                          $ts_save(await response.blob(), $ts_hcd(disposition));
                        """);
                    }
                    else
                    {
                        rawBuilder.AppendLine("""                            
                          if (typeof $ts_handle_response !== 'undefined') {
                            return await $ts_handle_response(response) as any;
                          }
                        """);

                        if (_actionResults.Contains(returnFullName))
                        {
                            returnGeneralType = TypeReference.Promise(AnyKeyword.Default);
                            rawBuilder.AppendLine("""                            
                              return await response.json() as any;
                            """);
                        }
                        else
                        {
                            var awaitType = Generator.GetOrCreateGeneralType(returnType);
                            returnGeneralType = TypeReference.Promise(awaitType);
                            if (awaitType.Kind != SyntaxKind.VoidKeyword)
                            {
                                rawBuilder.AppendLine($"""
                                  return await response.json() as {awaitType.GetText()};
                                """);
                            }
                        }
                    }

                    rawBuilder.Append("""
                    }).catch(reason => {
                      if (typeof $ts_handle_error !== 'undefined') {
                        return new Promise((_, reject) => reject($ts_handle_error(reason)));
                      }
                      return new Promise((_, reject) => reject(reason));
                    });
                    """);


                    if (action.Produces is not null && action.Produces.Length > 1)
                    {
                        var methodDeclaration = new MethodDeclaration(
                            [AsyncKeyword.Default],
                            methodName,
                            [
                                ..
                                from x in methodParams select x.Paramter,

                                new Parameter(
                                    new Identifier("__accept"),
                                    new UnionType(
                                    [
                                        ..
                                        from p in action.Produces
                                        select new LiteralType(new StringLiteral(p))
                                    ]),
                                    new LiteralType(new StringLiteral(action.Produces[0]))
                                )
                            ],
                            returnGeneralType
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
                            [AsyncKeyword.Default],
                            methodName,
                            [.. from x in methodParams select x.Paramter],
                            returnGeneralType
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
}
