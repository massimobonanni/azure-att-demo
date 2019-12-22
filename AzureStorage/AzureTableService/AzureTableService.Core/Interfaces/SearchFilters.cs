namespace AzureTableService.Core.Interfaces
{
    public abstract class SearchFilters
    {
        public enum SortDirection
        {
            Ascending,
            Descending
        }

        public int? PageNumber { get; set; }
        public int? NumberOfItems { get; set; }

        public string OrderBy { get; set; }
        public SortDirection OrderDirection { get; set; }
    }
}