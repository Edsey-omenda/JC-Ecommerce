using JC_Ecommerce.Models.DTOs;
using JC_Ecommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JC_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusesController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;

        public OrderStatusesController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        //Get all status types
        //GET: api/OrderStatuses/order-statuses
        [HttpGet]
        //[Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var statuses = await orderRepository.GetAllOrderStatusesAsync();

            var result = statuses.Select(s => new OrderStatusDTO
            {
                Id = s.Id,
                Name = s.Name,
                Color = s.Color
            }).ToList();

            return Ok(result);
        }
    }
}
