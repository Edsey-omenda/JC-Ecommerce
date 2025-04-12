namespace JC_Ecommerce.Models.Domain
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        // New properties
        public List<string> Color { get; set; } = new List<string>();
        public List<string> Size { get; set; } = new List<string>();

        //Navigation Properties
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
