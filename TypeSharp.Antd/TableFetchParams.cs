using System.Collections.Generic;

namespace TypeSharp.Antd
{
    public class TableFetchParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortKey { get; set; }

        /// <summary>
        /// if 'descend' then Descend, otherwise Aescend.
        /// </summary>
        public string SortOrder { get; set; }
        public ESortOrder ESortOrder => SortOrder != "descend" ? ESortOrder.Aescend : ESortOrder.Descend;

        public Dictionary<string, string[]> FilteredValues { get; set; } = new Dictionary<string, string[]>();
    }
}
