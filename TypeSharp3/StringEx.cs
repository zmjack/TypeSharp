namespace TypeSharp
{
    internal static class StringEx
    {
        private static int IndexOf(string @this, Func<char, bool> predicate)
        {
            int i = 0;
            foreach (var e in @this)
            {
                if (predicate(e))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Converts a string to CamelCase string. e.g. dotNET.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CamelCase(string source)
        {
            if (string.IsNullOrEmpty(source)) return source;

            static bool IsNotUpper(char c) => c is < 'A' or > 'Z';
            var index = IndexOf(source, IsNotUpper);

            return index switch
            {
                int when index > 1 => $"{source[..(index - 1)].ToLower()}{source[index - 1]}{source[index..]}",
                int when index == 1 => $"{char.ToLower(source[0])}{source.Substring(index)}",
                int when index == 0 => source,
                _ => source.ToLower(),
            };
        }
    }
}
