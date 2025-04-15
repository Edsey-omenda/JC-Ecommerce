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

        public async Task<PagedResult<Order>> GetAllOrdersAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageIndex = 1, int pageSize = 10)
        {
            var query = jCEcommerceDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("CustomerName", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(o => o.User.FullName.ToLower().Contains(filterQuery.ToLower()));
                }
                else if (filterOn.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(o => o.Status.Name.ToLower().Contains(filterQuery.ToLower()));
                }
            }

            var totalItems = await jCEcommerceDbContext.Orders.CountAsync();
            var filteredItems = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "date" => isAscending ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate),
                    "total" => isAscending ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount),
                    _ => query
                };
            }

            var skip = (pageIndex - 1) * pageSize;
            var orders = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedResult<Order>
            {
                Items = orders,
                TotalItems = totalItems,
                FilteredItems = filteredItems,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(filteredItems / (double)pageSize),
                BeginIndex = skip + 1,
                EndIndex = skip + orders.Count,
                ReturnedItems = orders.Count
            };
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

        public async Task<PagedResult<Order>> GetOrdersByUserIdAsync(Guid userId, string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageIndex = 1, int pageSize = 1000)
        {
            var query = jCEcommerceDbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Status)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(o => o.Status.Name.ToLower().Contains(filterQuery.ToLower()));
                }
            }

            var totalItems = await jCEcommerceDbContext.Orders.CountAsync();
            var filteredItems = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "date" => isAscending ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate),
                    "total" => isAscending ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount),
                    _ => query
                };
            }

            var skip = (pageIndex - 1) * pageSize;
            var orders = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedResult<Order>
            {
                Items = orders,
                TotalItems = totalItems,
                FilteredItems = filteredItems,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(filteredItems / (double)pageSize),
                BeginIndex = skip + 1,
                EndIndex = skip + orders.Count,
                ReturnedItems = orders.Count
            };
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
