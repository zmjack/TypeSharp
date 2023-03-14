using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public struct QualifiedName
    {
        public string Value { get; internal set; }

        public static QualifiedName Combine(params string[] parts)
        {
            var sb = new StringBuilder();
            var enumerator = parts.Where(x => x is not null).GetEnumerator();

            while (enumerator.MoveNext())
            {
                sb.Append(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    sb.Append('.');
                    sb.Append(enumerator.Current);
                }
            }

            var value = sb.ToString();
            return new QualifiedName { Value = value };
        }

        public string GetSimplifiedName(string ownerPrefix)
        {
            var fullName = Value;
            if (ownerPrefix is null) return fullName;

            var common = StringEx.CommonStarts(fullName, ownerPrefix);
            if (common.Length != 0)
            {
                var lastDot = common.LastIndexOf('.');
                if (lastDot == -1) lastDot = common.Length;
                return fullName.Substring(lastDot + 1);
            }
            else return fullName;
        }

        public static implicit operator QualifiedName(string value) => new QualifiedName { Value = value };
    }
}
