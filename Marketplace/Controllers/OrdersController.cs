using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace Marketplace.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
        {
            var orderDto = await _orderService.CreateOrderAsync(createOrderDto);
            return Created();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var orderDto = await _orderService.GetOrderByIdAsync(id);
            return Ok(orderDto);
        }


        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetOrdersByUser(int id)
        {
            var ordersDto = await _orderService.GetOrdersByUserIdAsync(id);
            return Ok(ordersDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> MarkOrderAsPaid(int id)
        {
            await _orderService.MarkOrderAsPaidAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> MarkOrderAsCompleted(int id)
        {
            await _orderService.MarkOrderAsCompletedAsync(id);
            return NoContent();
        }
    }
}