using System.Collections.Generic;

namespace BookApi.models
{
    public class BookCriteria
    {
        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public string SearchText { get; set; }

        public IEnumerable<FilterCriteria> FiltersCriteria { get; set; }

        public SortCriteria SortCriteria { get; set; }
    }

    public class FilterCriteria
    {
        public string Column { get; set; }

        public FilterOperator Operator { get; set; }

        public object Value1 { get; set; }

        public object Value2 { get; set; }
    }

    public class SortCriteria
    {
        public string column { get; set; }

        public bool? ascending { get; set; }
    }

    public enum FilterOperator
    {
        Between,

        GreaterThan,

        LessThan,

        EqualTo
    }
}