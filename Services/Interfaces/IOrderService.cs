using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<bool> DeleteOrderAsync(int orderId);
        Task MarkOrderAsPaidAsync(int orderId);
        Task MarkOrderAsCompletedAsync(int orderId);
        Task DeleteUnpaidOrdersAsync();
    }
}
