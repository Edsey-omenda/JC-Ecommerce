namespace JC_Ecommerce.Models.Domain
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public Guid StatusId { get; set; } // FK

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        //Navigation Properties
        public OrderStatus Status { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
