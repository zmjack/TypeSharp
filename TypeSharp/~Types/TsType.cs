﻿using NStandard;
using System;
using System.Linq;
using System.Reflection;

namespace TypeSharp
{
    public class TsType
    {
        public TsType(CacheContainer<Type, TsType> tsTypes, Type clrType, bool cacheProperties)
        {
            ClrType = clrType;

            TsProperties = new Cache<TsProperty[]>
            {
                CacheMethod = () =>
                {
                    if (cacheProperties)
                    {
                        var allProps = clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        var declaredProps = allProps.Where(x => x.DeclaringType == clrType);
                        var duplicateNames = allProps.Select(x => x.Name).Intersect(declaredProps.Select(x => x.Name)).ToArray();
                        var props = allProps.Where(x => !duplicateNames.Contains(x.Name)).Concat(declaredProps);

                        if (clrType.IsGenericType)
                        {
                            if (clrType.IsGenericTypeDefinition)
                            {
                                return props.Select(prop =>
                                {
                                    var propType = prop.PropertyType;
                                    if (propType.IsGenericParameter)
                                    {
                                        return new TsProperty
                                        {
                                            PropertyName = StringEx.CamelCase(prop.Name),
                                            PropertyTypeDefinition = propType.Name,
                                        };
                                    }
                                    else
                                    {
                                        return new TsProperty
                                        {
                                            PropertyName = StringEx.CamelCase(prop.Name),
                                            PropertyType = tsTypes[propType].Value,
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
                                return new TsProperty
                                {
                                    PropertyName = StringEx.CamelCase(prop.Name),
                                    PropertyType = tsTypes[propType].Value,
                                };
                            }).ToArray();
                        }
                    }
                    else return new TsProperty[0];
                },
            };
        }

        /// <summary>
        /// Hint: If Namespace is null, the properties should be null.
        /// </summary>
        public string Namespace { get; set; }

        public string TypeName { get; set; }

        public Type ClrType { get; private set; }

        public TsTypeClass TypeClass => ClrType.IsEnum ? TsTypeClass.Enum : TsTypeClass.Interface;

        public Cache<TsProperty[]> TsProperties { get; private set; }

        public TsEnumValue[] TsEnumValues { get; set; }

        public bool Declare { get; set; }

        public string ReferenceName => Namespace is null ? TypeName : $"{Namespace}.{TypeName}";
    }
}