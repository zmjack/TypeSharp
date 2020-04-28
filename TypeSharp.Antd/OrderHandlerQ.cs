using System;
using System.Linq;
using System.Linq.Expressions;

namespace TypeSharp.Antd
{
    public class OrderHandlerQ<TSource>
    {
        private readonly IQueryable<TSource> Source;
        private readonly ESortOrder SortOrder;

        public OrderHandlerQ(IQueryable<TSource> source, ESortOrder sortOrder)
        {
            Source = source;
            SortOrder = sortOrder;
        }

        public IOrderedQueryable<TSource> Order<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            if (SortOrder == ESortOrder.Aescend)
                return Queryable.OrderBy(Source, keySelector);
            else return Queryable.OrderByDescending(Source, keySelector);
        }

    }
}
