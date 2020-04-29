using LinqSharp;
using NStandard;
using System.Collections.Generic;

namespace TypeSharp.Antd
{
    public class TableFetchHandlerE<TSource>
    {
        public readonly TableFetchParams FetchParams;
        public Dictionary<string, OrderDelegateE<TSource>> Orders = new Dictionary<string, OrderDelegateE<TSource>>();
        public Dictionary<string, FilterDelegateE<TSource>> Filters = new Dictionary<string, FilterDelegateE<TSource>>();

        public TableFetchHandlerE(TableFetchParams fetchParams)
        {
            FetchParams = fetchParams;
        }

        public PagedEnumerable<TSource> Handle(IEnumerable<TSource> source)
        {
            if (Filters != null)
            {
                foreach (var filteredValue in FetchParams.FilteredValues)
                {
                    if (Filters.ContainsKey(filteredValue.Key))
                        source = Filters[filteredValue.Key](source, filteredValue.Value);
                }
            }

            if (Orders != null && !FetchParams.SortKey.IsNullOrWhiteSpace())
            {
                if (Orders.ContainsKey(FetchParams.SortKey))
                    source = Orders[FetchParams.SortKey](new OrderHandlerE<TSource>(source, FetchParams.ESortOrder));
            }

            return source.SelectPage(FetchParams.Page.For(x => x < 1 ? 1 : x), FetchParams.PageSize.For(x => x < 1 ? 20 : x));
        }
    }

}
