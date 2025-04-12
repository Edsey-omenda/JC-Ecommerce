namespace JC_Ecommerce.Models.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int FilteredItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int BeginIndex { get; set; }
        public int EndIndex { get; set; }
        public int ReturnedItems { get; set; }
    }
}
