using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TypeSharp.DevAnalyzer.XmlParsers;

internal static class UnionXml
{
    internal static IEnumerable<Union> Parse(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        List<Union> unionList = [];
        Dictionary<string, TypePair[]> refDict = [];

        var root = doc.DocumentElement;
        if (root.Name == "unions")
        {
            foreach (var node in
                from x in root.ChildNodes.OfType<XmlNode>()
                where x.NodeType == XmlNodeType.Element
                select x
            )
            {
                if (node.Name is not "union" and not "typedef") continue;

                var union = ParseNode(node, refDict);
                if (union is not null)
                {
                    if (node.Name == "union")
                    {
                        unionList.Add(union);
                    }
                    else if (node.Name == "typedef")
                    {
                        refDict.Add(union.TypeName, union.PossiableTypes);
                    }
                }
            }
        }

        return unionList;
    }

    internal static Union? ParseNode(XmlNode node, Dictionary<string, TypePair[]> refDict)
    {
        string? typeName = node.Name switch
        {
            "union" => node.Attributes["type"]?.Value!,
            "typedef" => node.Attributes["name"]?.Value!,
            _ => null,
        };

        if (string.IsNullOrWhiteSpace(typeName)) return null;

        var types = new List<TypePair>();
        foreach (var node1 in
            from x in node.ChildNodes.OfType<XmlNode>()
            where x.NodeType == XmlNodeType.Element
            select x
        )
        {
            if (node1.Name == "keyword")
            {
                var name = node1.Attributes["name"]?.Value!;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    types.Add(new()
                    {
                        ConstrutctType = ConstrutctType.Class,
                        Name = Keyword.GetClassName(name)
                    });
                }
            }
            else if (node1.Name == "class")
            {
                var name = node1.Attributes["name"]?.Value!;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    types.Add(new()
                    {
                        ConstrutctType = ConstrutctType.Class,
                        Name = name,
                    });
                }
            }
            else if (node1.Name == "interface")
            {
                var name = node1.Attributes["name"]?.Value!;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    types.Add(new()
                    {
                        ConstrutctType = ConstrutctType.Interface,
                        Name = name,
                    });
                }
            }
            else if (node.Name == "union" && node1.Name == "type")
            {
                var name = node1.Attributes["name"]?.Value!;
                if (refDict.TryGetValue(name, out var _types))
                {
                    types.AddRange(_types);
                }
            }
        }

        return new()
        {
            TypeName = typeName!,
            PossiableTypes = [.. types],
        };
    }
}
