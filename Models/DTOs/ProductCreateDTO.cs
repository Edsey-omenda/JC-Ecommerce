namespace JC_Ecommerce.Models.DTOs
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; }
        public List<string> Color { get; set; }
        public List<string> Size { get; set; }

    }
}
