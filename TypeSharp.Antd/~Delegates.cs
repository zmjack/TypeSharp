using System.Collections.Generic;
using System.Linq;

namespace TypeSharp.Antd
{
    public delegate IOrderedEnumerable<TSource> OrderDelegateE<TSource>(OrderHandlerE<TSource> handler);
    public delegate IEnumerable<TSource> FilterDelegateE<TSource>(IEnumerable<TSource> source, string[] values);

    public delegate IOrderedQueryable<TSource> OrderDelegateQ<TSource>(OrderHandlerQ<TSource> handler);
    public delegate IQueryable<TSource> FilterDelegateQ<TSource>(IQueryable<TSource> source, string[] values);

}
