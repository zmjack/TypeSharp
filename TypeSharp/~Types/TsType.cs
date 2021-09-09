using NStandard;
using NStandard.Caching;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TypeSharp
{
    public class TsType
    {
        /// <summary>
        /// Hint: If Namespace is null, the properties should be null.
        /// </summary>
        public string Namespace { get; set; }

        public string TypeName { get; set; }

        public string PureName => TypeName.ExtractFirst(new Regex(@"^([^<]+)"));

        public Type ClrType { get; private set; }

        public TsTypeClass TypeClass => ClrType.IsEnum ? TsTypeClass.Enum : TsTypeClass.Interface;

        public Cache<TsProperty[]> TsProperties { get; private set; }

        public TsEnumValue[] TsEnumValues { get; set; }

        public bool Declare { get; set; }

        public string ReferenceName => Namespace is null ? TypeName : $"{Namespace}.{TypeName}";

        public TsType(TypeScriptModelBuilder builder, Type clrType, bool cacheProperties)
        {
            var tsTypes = builder.TsTypes;
            ClrType = clrType;

            TsProperties = new Cache<TsProperty[]>
            {
                CacheMethod = () =>
                {
                    if (cacheProperties)
                    {
                        var chain = clrType.GetExtendChain();
                        var props = clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(x => !x.GetCustomAttributes().Any(attr => attr.GetType().FullName == $"{nameof(TypeSharp)}.{nameof(TypeScriptIgnoreAttribute)}"))
                            .GroupBy(x => x.Name)
                            .Select(g =>
                            {
                                if (g.Skip(1).Any()) return g.OrderBy(x => chain.IndexOf(x.DeclaringType)).First();
                                else return g.First();
                            }).ToArray();

                        if (clrType.IsGenericType)
                        {
                            if (clrType.IsGenericTypeDefinition)
                            {
                                return props.Select(prop =>
                                {
                                    var propType = prop.PropertyType;
                                    var required = prop.HasAttributeViaName("System.ComponentModel.DataAnnotations.RequiredAttribute");
                                    if (propType.IsGenericParameter)
                                    {
                                        return new TsProperty
                                        {
                                            ClrName = prop.Name,
                                            PropertyName = StringEx.CamelCase(prop.Name),
                                            PropertyTypeDefinition = propType.Name,
                                            Required = required,
                                        };
                                    }
                                    else
                                    {
                                        return new TsProperty
                                        {
                                            ClrName = prop.Name,
                                            PropertyName = StringEx.CamelCase(prop.Name),
                                            PropertyType = tsTypes[propType].Value,
                                            Required = required,
                                        };
                                    }
                                }).ToArray();
                            }
                            else return new TsProperty[0];
                        }
                        else
                        {
                            return props.Select(prop =>
                            {
                                var propType = prop.PropertyType;
                                var required = prop.HasAttributeViaName("System.ComponentModel.DataAnnotations.RequiredAttribute");

                                return new TsProperty
                                {
                                    ClrName = prop.Name,
                                    PropertyName = StringEx.CamelCase(prop.Name),
                                    PropertyType = tsTypes[propType].Value,
                                    Required = required,
                                };
                            }).ToArray();
                        }
                    }
                    else return new TsProperty[0];
                },
            };
        }

    }
}
