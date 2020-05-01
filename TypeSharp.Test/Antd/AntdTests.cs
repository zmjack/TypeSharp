using LinqSharp;
using System.Linq;
using Xunit;

namespace TypeSharp.Antd.Test
{
    public class AtndTests
    {
        private class Model
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void Test1()
        {
            var fetchParams = new TableFetchParams
            {
                Page = 2,
                PageSize = 2,
                FilteredValues =
                {
                    [nameof(Model.Name)] = new[] { "A" },
                },
                SortKey = nameof(Model.Age),
                SortOrder = "descend",
            };

            Model[] source =
            {
                new Model { Name = "A1", Age = 18 },
                new Model { Name = "A2", Age = 19 },
                new Model { Name = "A3", Age = 20 },
                new Model { Name = "B1", Age = 22 },
                new Model { Name = "B2", Age = 23 },
            };

            var handler = new TableFetchHandlerE<Model>(fetchParams)
            {
                Filters =
                {
                    [nameof(Model.Name)] = (e, values) => e.XWhere(h =>
                    {
                        return h.Or(values, v => x => x.Name.Contains(v));
                    }),
                },
                Orders =
                {
                    [nameof(Model.Age)] = h => h.Order(x => x.Age),
                },
            };

            var record = handler.Handle(source).First();
            Assert.Equal("A1", record.Name);
        }
    }
}
