using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeSharp.Antd
{
    public class OrderHandlerE<TSource>
    {
        private readonly IEnumerable<TSource> Source;
        private readonly ESortOrder SortOrder;

        public OrderHandlerE(IEnumerable<TSource> source, ESortOrder sortOrder)
        {
            Source = source;
            SortOrder = sortOrder;
        }

        public IOrderedEnumerable<TSource> Order<TKey>(Func<TSource, TKey> keySelector)
        {
            if (SortOrder == ESortOrder.Aescend)
                return Enumerable.OrderBy(Source, keySelector);
            else return Enumerable.OrderByDescending(Source, keySelector);
        }

    }
}
