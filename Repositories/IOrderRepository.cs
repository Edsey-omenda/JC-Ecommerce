using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;

namespace JC_Ecommerce.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Guid userId, List<OrderItemDTO> items);
        Task<PagedResult<Order>> GetAllOrdersAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageIndex = 1, int pageSize = 1000); //Admin
        Task<PagedResult<Order>> GetOrdersByUserIdAsync(Guid userId, string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageIndex = 1, int pageSize = 1000); //Customer
        Task<Order?> GetOrderByIdAsync(Guid orderId, Guid? userId = null); //Optional check for owner
        Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId); //Admin
        Task<List<OrderStatus>> GetAllOrderStatusesAsync();

    }
}
