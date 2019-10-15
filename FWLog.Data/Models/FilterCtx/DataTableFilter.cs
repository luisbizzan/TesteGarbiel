using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Models.FilterCtx
{
    public class DataTableFilter
    {
        // properties are not capital due to json mapping
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<Column> Columns { get; set; }
        public Search Search { get; set; }
        public List<Order> Order { get; set; }

        /// <summary>
        /// Returns the first ordered column name.
        /// </summary>
        public string OrderByColumn
        {
            get
            {
                if (Columns == null || !Columns.Any() || Order == null || !Order.Any())
                {
                    return null;
                }

                return Columns[Order[0].Column].Data;
            }
        }

        /// <summary>
        /// Returns the order direction (ASC or DESC) for the first ordered column.
        /// </summary>
        public string OrderByDirection
        {
            get
            {
                if (Order == null || !Order.Any())
                {
                    return null;
                }

                bool dirBool = string.Equals("asc", Order[0].Dir, System.StringComparison.InvariantCultureIgnoreCase);
                return dirBool ? "ASC" : "DESC";
            }
        }
    }

    public class DataTableFilter<T> : DataTableFilter
    {
        public T CustomFilter { get; set; }
    }

    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; }
    }

    public class Search
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    public class Order
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }
}
