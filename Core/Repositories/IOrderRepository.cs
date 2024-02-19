using Core.Models;

namespace Core.Repositories
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderAsync(Order order);
        Task InsertOrderItemAsync(OrderItem orderItem);
        Task DeleteUnpaidOrdersAsync();
        Task<bool> UserExistsAsync(int userId);
    }
}
