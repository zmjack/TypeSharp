namespace TypeSharp;

internal class NamespaceComparer : IComparer<string[]>
{
    public int Compare(string[] x, string[] y)
    {
        if (x.Length == y.Length)
        {
            foreach (var (left, right) in x.Zip(y))
            {
                var ret = StringComparer.Ordinal.Compare(left, right);
                if (ret != 0) return ret;
            }
            return 0;
        }
        else
        {
            return x.Length < y.Length ? -1 : 1;
        }
    }
}
