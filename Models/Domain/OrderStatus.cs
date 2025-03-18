namespace JC_Ecommerce.Models.Domain
{
    public class OrderStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }  //"Pending", "Shipped"
        public string Color { get; set; }

        //Navigation Properties
        public ICollection<Order> Orders { get; set; }
    }
}
