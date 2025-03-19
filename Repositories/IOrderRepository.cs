using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;

namespace JC_Ecommerce.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Guid userId, List<OrderItemDTO> items);
        Task<List<Order>> GetAllOrdersAsync(); //Admin
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId); //Customer
        Task<Order?> GetOrderByIdAsync(Guid orderId, Guid? userId = null); //Optional check for owner
        Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId); //Admin
        Task<List<OrderStatus>> GetAllOrderStatusesAsync();

    }
}
