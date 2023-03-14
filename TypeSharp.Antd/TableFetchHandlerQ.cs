using LinqSharp;
using LinqSharp.Pages;
using NStandard;
using System.Collections.Generic;
using System.Linq;

namespace TypeSharp.Antd
{
    public class TableFetchHandlerQ<TSource>
    {
        public readonly TableFetchParams FetchParams;
        public Dictionary<string, OrderDelegateQ<TSource>> Orders = new Dictionary<string, OrderDelegateQ<TSource>>();
        public Dictionary<string, FilterDelegateQ<TSource>> Filters = new Dictionary<string, FilterDelegateQ<TSource>>();

        public TableFetchHandlerQ(TableFetchParams fetchParams)
        {
            FetchParams = fetchParams;
        }

        public PagedQueryable<TSource> Handle(IQueryable<TSource> source)
        {
            if (Filters != null)
            {
                foreach (var filteredValue in FetchParams.FilteredValues)
                {
                    if (Filters.ContainsKey(filteredValue.Key))
                        source = Filters[filteredValue.Key](source, filteredValue.Value);
                }
            }

            if (Orders != null && FetchParams.SortKey.IsNullOrWhiteSpace())
            {
                if (Orders.ContainsKey(FetchParams.SortKey))
                    source = Orders[FetchParams.SortKey](new OrderHandlerQ<TSource>(source, FetchParams.ESortOrder));
            }

            return source.SelectPage(FetchParams.Page.For(x => x < 1 ? 1 : x), FetchParams.PageSize.For(x => x < 1 ? 20 : x));
        }
    }

}
