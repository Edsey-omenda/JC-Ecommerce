namespace JC_Ecommerce.Models.DTOs
{
    public class OrderResponseDTO
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItemResponseDTO> Items { get; set; }
    }

    public class OrderItemResponseDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
