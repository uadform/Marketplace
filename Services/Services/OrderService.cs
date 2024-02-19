using Clients.Interfaces;
using Contracts.Exceptions;
using Core.DTOs;
using Core.Models;
using Core.Repositories;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserClient _userClient;

        public OrderService(IOrderRepository orderRepository, IUserClient userClient)
        {
            _orderRepository = orderRepository;
            _userClient = userClient;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var userExists = await _orderRepository.UserExistsAsync(createOrderDto.UserId);
            if (!userExists)
            {
                throw new NotFoundException($"User with ID {createOrderDto.UserId} not found.");
            }

            var order = new Order
            {
                UserId = createOrderDto.UserId,
                CreatedAt = DateTime.UtcNow,
                IsPaid = false,
                IsCompleted = false
            };

            order.OrderId = await _orderRepository.CreateOrderAsync(order);

            foreach (var item in createOrderDto.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                };
                await _orderRepository.InsertOrderItemAsync(orderItem);
            }

            return MapToOrderDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
            {
                throw new NotFoundException($"No orders found for user ID {userId}.");
            }

            return orders.Select(order => MapToOrderDto(order));
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            return MapToOrderDto(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var deleted = await _orderRepository.DeleteOrderAsync(orderId);
            if (!deleted)
            {
                throw new NotFoundException($"Order with ID {orderId} could not be deleted or does not exist.");
            }
            return deleted;
        }

        public async Task MarkOrderAsPaidAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            order.IsPaid = true;
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task MarkOrderAsCompletedAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            if (!order.IsPaid)
            {
                throw new PaymentRequiredException($"Order with ID {orderId} is not paid.");
            }

            order.IsCompleted = true;
            var updated = await _orderRepository.UpdateOrderAsync(order);
            if (!updated)
            {
                throw new UnprocessableEntityException($"Failed to mark order {orderId} as completed.");
            }
        }

        public async Task DeleteUnpaidOrdersAsync()
        {
            await _orderRepository.DeleteUnpaidOrdersAsync();
        }

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                IsPaid = order.IsPaid,
                IsCompleted = order.IsCompleted,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}
