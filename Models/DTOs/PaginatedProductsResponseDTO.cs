namespace JC_Ecommerce.Models.DTOs
{
    public class PaginatedProductsResponseDTO
    {
        public List<ProductResponseDTO> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int FilteredItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)FilteredItems / PageSize);
    }
}
