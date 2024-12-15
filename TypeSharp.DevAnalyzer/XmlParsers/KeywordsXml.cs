using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TypeSharp.DevAnalyzer.XmlParsers;

internal static class KeywordsXml
{
    internal static IEnumerable<Keyword> Parse(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        List<Keyword> list = [];

        var root = doc.DocumentElement;
        if (root.Name == "keywords")
        {
            foreach (var node in
                from x in root.ChildNodes.OfType<XmlNode>()
                where x.NodeType == XmlNodeType.Element
                select x
            )
            {
                if (node.Name != "keyword") continue;

                var name = node.Attributes["name"]?.Value!;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list.Add(new()
                    {
                        Name = name,
                        ClassName = Keyword.GetClassName(name),
                    });
                }
            }
        }

        return list;
    }
}
