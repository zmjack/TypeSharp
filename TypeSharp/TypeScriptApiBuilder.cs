using NStandard;
using NStandard.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TypeSharp
{
    public class TypeScriptApiBuilder
    {
        private readonly TypeScriptModelBuilder ModelBuilder = new TypeScriptModelBuilder();
        private CacheSet<Type, TsType> TsTypes => ModelBuilder.TsTypes;
        private readonly string RootUri;

        public HashSet<Type> TypeList = new HashSet<Type>();

        public TypeScriptApiBuilder(string rootUri)
        {
            RootUri = rootUri;
        }

        public void WriteTo(string path) => File.WriteAllText(path, Compile());

        public string Compile()
        {
            var exports = new List<string>();
            var code = new StringBuilder();
            code.AppendLine(Declare.Info);
            code.AppendLine("import { ApiHelper } from \"type-sharp\"");

            var groups = TypeList.Select(type => TsTypes[type].Value).GroupBy(x => x.Namespace).ToArray();
            if (groups.Any()) code.AppendLine();
            foreach (var group in groups)
            {
                var ns = group.Key;
                foreach (var type in group)
                {
                    var typeName = GetTypeName(type.ClrType).ExtractFirst(new Regex(@"^(\w+?)(?:Controller)?$"), "$1Api");
                    code.AppendLine($@"export class {typeName} {{");
                    code.AppendLine($@"    constructor(public api: ApiHelper = ApiHelper.default) {{ }}");

                    var classRouteTemplate = GetRouteTemplate(type.ClrType);
                    var methods = type.ClrType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        Type returnClrType;

                        var returnAttr = method.GetCustomAttributes()
                            .FirstOrDefault(x => x.GetType().FullName == typeof(ApiReturnAttribute).FullName)?
                            .For(x =>
                            {
                                var eobj = x.ToExpandoObject() as IDictionary<string, object>;
                                return new ApiReturnAttribute(eobj[nameof(ApiReturnAttribute.ReturnType)] as Type);
                            });
                        if (returnAttr != null) returnClrType = returnAttr.ReturnType;
                        else
                        {
                            var returnType = method.ReturnType;
                            if (returnType.FullName.StartsWith("Microsoft.AspNetCore.Mvc"))
                                returnClrType = typeof(object);
                            else returnClrType = returnType;
                        }

                        var returnTsType = TsTypes[returnClrType].Value;
                        var methodRouteTemplate = GetRouteTemplate(type.ClrType);
                        var controller = type.ClrType.Name.ExtractFirst(new Regex(@"^(\w+?)(?:Controller)?$"));
                        var action = method.Name;
                        var verb = GetMethodVerbs(method) switch
                        {
                            string[] verbs when verbs.Contains("get") => "get",
                            string[] verbs when verbs.Contains("post") => "post",
                            string[] verbs when verbs.Contains("put") => "put",
                            string[] verbs when verbs.Contains("delete") => "delete",
                            string[] verbs when verbs.Contains("options") => "options",
                            string[] verbs when verbs.Contains("head") => "head",
                            string[] verbs when verbs.Contains("patch") => "patch",
                            _ => "get",
                        };

                        var route = (methodRouteTemplate ?? classRouteTemplate)?
                            .RegexReplace(new Regex(@"[\{\[]controller[\}\]]|[\{\[]controller\s*=[^\}\]]+[\}\]]"), controller)
                            .RegexReplace(new Regex(@"[\{\[]action[\}\]]|[\{\[]action\s*=[^\}\]]+[\}\]]"), action)
                            ?? $"{controller}/{action}";
                        var parameters = method.GetParameters();
                        var parametersDeclare = parameters.Select(x => $"{x.Name}: {TsTypes[x.ParameterType].Value.ReferenceName}").Join(", ");

                        var bodyParameters = parameters.Where(x => x.HasAttributeViaName("Microsoft.AspNetCore.Mvc.FromBodyAttribute"));
                        var formParameters = parameters.Where(x => x.HasAttributeViaName("Microsoft.AspNetCore.Mvc.FromFormAttribute"));
                        var queryParameters = parameters.Except(bodyParameters).Except(formParameters);
                        var queryParametersDeclare = queryParameters.Any() ? $" {queryParameters.Select(x => x.Name).Join(", ")} " : "";
                        var uri = $"{RootUri}/{route}";


                        var returnFileAttr = method.GetCustomAttributes().FirstOrDefault(x => x.GetType().FullName == typeof(ApiReturnFileAttribute).FullName);
                        if (returnFileAttr == null)
                        {
                            if (new[] { "post", "put", "patch" }.Contains(verb))
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<{returnTsType.ReferenceName}> {{" +
                                    $" return this.api.{verb}('{uri}', {bodyParameters.FirstOrDefault()?.Name ?? "{}"}, {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                            else
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<{returnTsType.ReferenceName}> {{" +
                                    $" return this.api.{verb}('{uri}', {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                        }
                        else
                        {
                            if (new[] { "post" }.Contains(verb))
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<void> {{" +
                                    $" return this.api.{verb}_save('{uri}', {bodyParameters.FirstOrDefault()?.Name ?? "{}"}, {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                            else if (new[] { "get" }.Contains(verb))
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<void> {{" +
                                    $" return this.api.{verb}_save('{uri}', {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                            else throw new NotSupportedException($"Only GET and POST is supported with {nameof(ApiReturnFileAttribute)}.");
                        }
                    }

                    code.AppendLine($@"}}");
                }
            }

            return code.ToString();
        }

        private string GetRouteTemplate(ICustomAttributeProvider provider)
        {
            var attr = provider.GetAttributeViaName("Microsoft.AspNetCore.Mvc.RouteAttribute");
            var template = attr?.GetReflector().DeclaredProperty<string>("Template")?.Value;
            return template;
        }

        private string[] GetMethodVerbs(MethodInfo provider)
        {
            var verbDefines = new[]
            {
                (Verb: "get", Attr: "Microsoft.AspNetCore.Mvc.HttpGetAttribute"),
                (Verb: "post", Attr: "Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
                (Verb: "put", Attr: "Microsoft.AspNetCore.Mvc.HttpPutAttribute"),
                (Verb: "delete", Attr: "Microsoft.AspNetCore.Mvc.HttpDeleteAttribute"),
                (Verb: "options", Attr: "Microsoft.AspNetCore.Mvc.HttpOptionsAttribute"),
                (Verb: "head", Attr: "Microsoft.AspNetCore.Mvc.HttpHeadAttribute"),
                (Verb: "patch", Attr: "Microsoft.AspNetCore.Mvc.HttpPatchAttribute"),
            };

            var attrs = provider.GetCustomAttributes();
            var verbs = verbDefines.Where(define => attrs.Any(x => x.GetType().FullName == define.Attr)).Select(x => x.Verb).ToArray();
            return verbs;
        }

        private string GetTypeName(Type type)
        {
            var attr = type.GetCustomAttribute<TypeScriptApiAttribute>();
            return attr?.TypeName ?? type.Name;
        }

        public void CacheTypes(params Type[] types) => types.Each(type => CacheType(type));
        public void CacheType<TType>() => CacheType(typeof(TType));
        public void CacheType(Type type) => TypeList.Add(type);

        public void AddDeclaredType(Type type, string typeName) => ModelBuilder.AddDeclaredType(type, typeName);

    }
}