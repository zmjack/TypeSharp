using NStandard;
using NStandard.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        public string[] PossibleValues { get; set; }

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
                        var props = clrType
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(x => !x.GetCustomAttributes().Any(attr => attr.GetType().FullName == $"{nameof(TypeSharp)}.{nameof(TypeScriptIgnoreAttribute)}"))
                            .GroupBy(x => x.Name)
                            .Select(g =>
                            {
                                if (g.Skip(1).Any()) return g.OrderBy(x => chain.IndexOf(x.DeclaringType)).First();
                                else return g.First();
                            }).ToArray();
                        var methods = clrType
                            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                            .Where(x => !x.GetCustomAttributes().Any(attr => attr.GetType().FullName == $"{nameof(TypeSharp)}.{nameof(TypeScriptIgnoreAttribute)}"))
                            .GroupBy(x => x.Name)
                            .Select(g =>
                            {
                                if (g.Skip(1).Any()) return g.OrderBy(x => chain.IndexOf(x.DeclaringType)).First();
                                else return g.First();
                            }).ToArray();

                        static bool IsRequired(PropertyInfo property)
                        {
                            var attributes = property.GetCustomAttributes(true);
                            var required = (
                                from x in attributes
                                let attributeType = x.GetType()
                                where attributeType.FullName == "System.ComponentModel.DataAnnotations.RequiredAttribute"
                                    || attributeType == typeof(TypeScriptRequiredAttribute)
                                select x
                            ).Any();
                            return required;
                        }

                        //TODO: not supported.
                        static string[] GetPossibleValues(PropertyInfo property)
                        {
                            var attributes = property.GetCustomAttributes(true);
                            var possibleValuesList = (
                                from x in attributes
                                let attributeType = x.GetType()
                                where attributeType == typeof(TypeScriptPossibleValuesAttribute)
                                select (x as TypeScriptPossibleValuesAttribute).PossibleValues
                            ).ToArray();

                            var list = new List<string>();
                            foreach (var possibleValue in Any.Flat<string>(possibleValuesList))
                            {
                                list.Add(possibleValue);
                            }

                            return list.ToArray();
                        }

                        var list = new List<TsProperty>();
                        if (clrType.IsGenericType)
                        {
                            if (clrType.IsGenericTypeDefinition)
                            {
                                foreach (var prop in props)
                                {
                                    var propType = prop.PropertyType;
                                    var required = IsRequired(prop);

                                    if (propType.IsGenericParameter)
                                    {
                                        list.Add(new TsProperty
                                        {
                                            ClrName = prop.Name,
                                            Property = StringEx.CamelCase(prop.Name),
                                            PropertyTypeDefinition = propType.Name,
                                            Required = required,
                                        });
                                    }
                                    else
                                    {
                                        list.Add(new TsProperty
                                        {
                                            ClrName = prop.Name,
                                            Property = StringEx.CamelCase(prop.Name),
                                            PropertyType = tsTypes[propType].Value,
                                            Required = required,
                                        });
                                    }
                                }
                            }
                            else return Array.Empty<TsProperty>();
                        }
                        else
                        {
                            foreach (var prop in props)
                            {
                                var propType = prop.PropertyType;
                                var required = IsRequired(prop);

                                list.Add(new TsProperty
                                {
                                    ClrName = prop.Name,
                                    Property = StringEx.CamelCase(prop.Name),
                                    PropertyType = tsTypes[propType].Value,
                                    Required = required,
                                });
                            }
                        }

                        return list.ToArray();
                    }
                    else return Array.Empty<TsProperty>();
                },
            };
        }

    }
}
