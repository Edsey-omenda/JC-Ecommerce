using JC_Ecommerce.Data;
using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace JC_Ecommerce.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly JCEcommerceDbContext jCEcommerceDbContext;

        public OrderRepository(JCEcommerceDbContext jCEcommerceDbContext)
        {
            this.jCEcommerceDbContext = jCEcommerceDbContext;
        }

        public async Task<Order> CreateOrderAsync(Guid userId, List<OrderItemDTO> items)
        {
            var productIds = items.Select(items => items.ProductId).ToList();

            var products = await jCEcommerceDbContext.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var product = products.FirstOrDefault(product => product.ProductId == item.ProductId);

                if(product == null) 
                {
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                }

                if(product.Stock < item.Quantity)
                {
                    throw new Exception($"Not enough stock for product: {product.Name}");
                }

                //Deduct Stock
                product.Stock -= item.Quantity;

                var orderItem = new OrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price * item.Quantity
                };

                totalAmount += orderItem.Price;
                orderItems.Add(orderItem);
            }

            // Create order
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                StatusId = (await jCEcommerceDbContext.OrderStatuses.FirstAsync(s => s.Name == "Pending")).Id,
                OrderItems = orderItems
            };

            await jCEcommerceDbContext.Orders.AddAsync(order);
            await jCEcommerceDbContext.SaveChangesAsync();

            return order;

        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await jCEcommerceDbContext.Orders.Include(o => o.User).Include(o => o.Status)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToListAsync();

            return orders;
        }

        public async Task<List<OrderStatus>> GetAllOrderStatusesAsync()
        {
            var statuses = await jCEcommerceDbContext.OrderStatuses.ToListAsync();

            return statuses;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, Guid? userId = null)
        {
            var order = await jCEcommerceDbContext.Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product).Include(o => o.Status).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return null;

            if (userId != null && order.UserId != userId) return null;

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders =  await jCEcommerceDbContext.Orders.Where(o => o.UserId == userId).Include(o => o.Status)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToListAsync();

            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId)
        {
            var order = await jCEcommerceDbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return false;
            }

            order.StatusId = statusId;
            await jCEcommerceDbContext.SaveChangesAsync();

            return true;
        }
    }
}
