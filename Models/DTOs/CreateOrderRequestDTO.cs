namespace JC_Ecommerce.Models.DTOs
{
    public class CreateOrderRequestDTO
    {
        public List<OrderItemDTO> Items { get; set; }
    }

    public class OrderItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
