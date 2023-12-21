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
        private readonly Regex ControllerRegex = new(@"^(\w+?)(?:Controller)?$", RegexOptions.Singleline);
        private readonly TypeScriptModelBuilder ModelBuilder = new();
        private CacheSet<Type, TsType> TsTypes => ModelBuilder.TsTypes;
        public HashSet<Type> TypeList = new();

        private readonly ApiBuilderOptions _options;

        public TypeScriptApiBuilder(ApiBuilderOptions options = null)
        {
            _options = options ??= ApiBuilderOptions.Default;
        }

        private class Verb
        {
            public string Name { get; set; }
            public string Attribute { get; set; }

            public Verb(string name, string attribute)
            {
                Name = name;
                Attribute = attribute;
            }
        }

        private static readonly Verb[] Verbs =
        [
            new Verb("get", "Microsoft.AspNetCore.Mvc.HttpGetAttribute"),
            new Verb("post", "Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
            new Verb("put", "Microsoft.AspNetCore.Mvc.HttpPutAttribute"),
            new Verb("delete", "Microsoft.AspNetCore.Mvc.HttpDeleteAttribute"),
            new Verb("options", "Microsoft.AspNetCore.Mvc.HttpOptionsAttribute"),
            new Verb("head", "Microsoft.AspNetCore.Mvc.HttpHeadAttribute"),
            new Verb("patch", "Microsoft.AspNetCore.Mvc.HttpPatchAttribute"),
        ];
        private static Verb DefaultVerb => Verbs[0];

        public void WriteTo(string path, string tsPackageName = "type-sharp") => File.WriteAllText(path, Compile(tsPackageName));

        private string GetTypeScriptTypeName(Type clrType)
        {
            if (clrType is null) return "null";
            else return TsTypes[clrType].Value.ReferenceName;
        }

        public string Compile(string tsPackageName = "type-sharp")
        {
            var exports = new List<string>();
            var code = new StringBuilder();
            code.AppendLine(BuildOptions.VersionDeclaring);
            code.AppendLine();
            code.AppendLine($"import {{ ApiHelper }} from \"{tsPackageName}\";");

            var groups = TypeList.Select(type => TsTypes[type].Value).GroupBy(x => x.Namespace).ToArray();
            if (groups.Any()) code.AppendLine();
            foreach (var group in groups)
            {
                var ns = group.Key;
                foreach (var type in group)
                {
                    var typeName = GetTypeName(type.ClrType).ExtractFirst(ControllerRegex, "$1Api");
                    code.AppendLine($@"export class {typeName} {{");
                    code.AppendLine($@"    constructor(public api: ApiHelper = ApiHelper.default) {{ }}");

                    var classRouteTemplate = GetRouteTemplate(type.ClrType);
                    var methods = type.ClrType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        Type[] clrTypes;

                        var apiReturnAttr = method.GetCustomAttributes()
                            .FirstOrDefault(x => x.GetType().FullName == typeof(ApiReturnAttribute).FullName)?
                            .Pipe(x =>
                            {
                                var eobj = x.ToExpandoObject() as IDictionary<string, object>;
                                return new ApiReturnAttribute(eobj[nameof(ApiReturnAttribute.PossibleTypes)] as Type[]);
                            });

                        if (apiReturnAttr is not null) clrTypes = apiReturnAttr.PossibleTypes;
                        else
                        {
                            var returnType = method.ReturnType;
                            if (returnType.FullName.StartsWith("Microsoft.AspNetCore.Mvc"))
                                clrTypes = [typeof(object)];
                            else clrTypes = [returnType];
                        }

                        var tsGenericTypeBuilder = new StringBuilder();
                        var clrTypesEnumerator = clrTypes.AsEnumerable().GetEnumerator();
                        if (clrTypesEnumerator.MoveNext())
                        {
                            tsGenericTypeBuilder.Append(GetTypeScriptTypeName(clrTypesEnumerator.Current));
                            while (clrTypesEnumerator.MoveNext())
                            {
                                tsGenericTypeBuilder.Append($" | {GetTypeScriptTypeName(clrTypesEnumerator.Current)}");
                            }
                        }
                        var returnTypeString = tsGenericTypeBuilder.ToString();

                        var methodRouteTemplate = GetRouteTemplate(type.ClrType);
                        var controller = type.ClrType.Name.ExtractFirst(ControllerRegex);
                        var action = method.Name;
                        var verbs = GetMethodVerbs(method);
                        var verb = GetMethodVerbs(method).FirstOrDefault() ?? DefaultVerb;

                        var routePattern = methodRouteTemplate ?? classRouteTemplate ?? _options.DefaultPattern;
                        var parameters = method.GetParameters();
                        var parametersDeclare = parameters.Select(x => $"{x.Name}: {TsTypes[x.ParameterType].Value.ReferenceName}").Join(", ");

                        var bodyParameters = parameters.Where(x => x.HasAttributeViaName("Microsoft.AspNetCore.Mvc.FromBodyAttribute"));
                        var formParameters = parameters.Where(x => x.HasAttributeViaName("Microsoft.AspNetCore.Mvc.FromFormAttribute"));
                        var queryParameters = parameters.Except(bodyParameters).Except(formParameters);
                        var queryParametersDeclare = queryParameters.Any() ? $" {queryParameters.Select(x => x.Name).Join(", ")} " : "";
                        var uri = routePattern?
                            .RegexReplace(new Regex(@"[\{\[]controller[\}\]]|[\{\[]controller\s*=[^\}\]]+[\}\]]"), controller)
                            .RegexReplace(new Regex(@"[\{\[]action[\}\]]|[\{\[]action\s*=[^\}\]]+[\}\]]"), action)
                            ?? $"{controller}/{action}";

                        var returnFileAttr = method.GetCustomAttributes().FirstOrDefault(x => x.GetType().FullName == typeof(ApiReturnFileAttribute).FullName);
                        var verbName = verb.Name;
                        if (returnFileAttr == null)
                        {
                            if (verbName is "post" or "put" or "patch")
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<{returnTypeString}> {{" +
                                    $" return this.api.{verbName}('{uri}', {bodyParameters.FirstOrDefault()?.Name ?? "{}"}, {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                            else
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<{returnTypeString}> {{" +
                                    $" return this.api.{verbName}('{uri}', {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                        }
                        else
                        {
                            if (verbName is "post")
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<void> {{" +
                                    $" return this.api.{verbName}_save('{uri}', {bodyParameters.FirstOrDefault()?.Name ?? "{}"}, {{{queryParametersDeclare}}});" +
                                    $" }}");
                            }
                            else if (verbName is "get")
                            {
                                code.AppendLine($"{" ".Repeat(4)}{StringEx.CamelCase(method.Name)}({parametersDeclare}): Promise<void> {{" +
                                    $" return this.api.{verbName}_save('{uri}', {{{queryParametersDeclare}}});" +
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

        private static string GetRouteTemplate(ICustomAttributeProvider provider)
        {
            var attr = provider.GetAttributeViaName("Microsoft.AspNetCore.Mvc.RouteAttribute");
            var template = attr?.GetReflector().DeclaredProperty<string>("Template")?.Value;
            return template;
        }

        private static Verb[] GetMethodVerbs(MethodInfo provider)
        {
            var attrs = provider.GetCustomAttributes();
            var verbs = Verbs.Where(verb => attrs.Any(x => x.GetType().FullName == verb.Attribute)).ToArray();
            return verbs;
        }

        private static string GetTypeName(Type type)
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