using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TypeSharp.DevAnalyzer.XmlParsers;

internal class Token
{
    public string? ClassName { get; set; }
    public string? Name { get; set; }
    public string? Mark { get; set; }

    public static string GetClassName(string name)
    {
        if (name.Length == 0) return "<invalid name>";

        var chars = name.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return $"{new string(chars)}Token";
    }
}

internal static class TokensXml
{
    internal static IEnumerable<Token> Parse(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        List<Token> list = [];

        var root = doc.DocumentElement;
        if (root.Name == "tokens")
        {
            foreach (var node in
                from x in root.ChildNodes.OfType<XmlNode>()
                where x.NodeType == XmlNodeType.Element
                select x
            )
            {
                if (node.Name != "token") continue;

                var name = node.Attributes["name"]?.Value!;
                var mark = node.Attributes["mark"]?.Value!;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list.Add(new()
                    {
                        ClassName = Token.GetClassName(name),
                        Name = name,
                        Mark = mark,
                    });
                }
            }
        }

        return list;
    }
}
