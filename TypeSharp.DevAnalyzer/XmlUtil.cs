using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeSharp.DevAnalyzer.XmlParsers;

namespace TypeSharp.DevAnalyzer;

internal static class XmlUtil
{
    private static readonly Dictionary<string, string> contents = [];
    private static readonly Dictionary<string, object> cache = [];

    internal static IEnumerable<Keyword> GetKeywords(GeneratorExecutionContext context)
    {
        return GetOrCreate(context, "keywords.xml", KeywordsXml.Parse) ?? [];
    }
    internal static IEnumerable<Union> GetUnions(GeneratorExecutionContext context)
    {
        return GetOrCreate(context, "unions.xml", UnionXml.Parse) ?? [];
    }

    private static T? GetOrCreate<T>(GeneratorExecutionContext context, string name, Func<string, T> factory) where T : class
    {
        var file = context.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == name);
        if (file is not null)
        {
            var fileContent = file.GetText()!.ToString();
            if (contents.TryGetValue(name, out var content))
            {
                if (!System.Diagnostics.Debugger.IsAttached && fileContent == content)
                {
                    return cache[name] as T;
                }
            }

            var target = factory(fileContent);
            contents[name] = fileContent;
            cache[name] = target;
            return target;
        }
        else return null;
    }
}
