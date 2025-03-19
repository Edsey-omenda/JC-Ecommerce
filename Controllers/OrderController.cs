using JC_Ecommerce.Models.DTOs;
using JC_Ecommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JC_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        //Get all orders
        //GET: api/Orders/
        [HttpGet("all-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await orderRepository.GetAllOrdersAsync();

            var response = orders.Select(order => new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status?.Name ?? "Pending",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        //Get logged in user orders -my
        //GET: api/Orders/my
        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var orders = await orderRepository.GetOrdersByUserIdAsync(Guid.Parse(userIdClaim));

            var response = orders.Select(order => new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status?.Name ?? "Pending",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        //Get a single order by id
        //GET: api/Orders/{id:Guid}
        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var isAdmin = User.IsInRole("Admin");

            var order = await orderRepository.GetOrderByIdAsync(id, isAdmin ? null : Guid.Parse(userIdClaim));

            if (order == null)
            {
                return NotFound();
            }

            var response = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status?.Name ?? "Unknown",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return Ok(response);
        }


        //Add/Create a new order
        //POST: api/Orders
        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> PlaceOrder([FromBody] List<OrderItemDTO> items)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID missing or invalid in token");
            }

            var order = await orderRepository.CreateOrderAsync(userId, items);

            var response = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status?.Name ?? "Pending",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return Ok(response);
        }


        //Update Order Status
        //PUT: api/Orders/{orderId}/status/{statusId}
        [HttpPut("{orderId}/status/{statusId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid orderId, Guid statusId)
        {
            var success = await orderRepository.UpdateOrderStatusAsync(orderId, statusId);

            if (!success)
            {
                return NotFound("Order not found");
            }

            return Ok("Order status updated");
        }

    }
}
