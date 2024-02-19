using Clients.Interfaces;
using Contracts.Exceptions;
using Core.DTOs;
using Core.Models;
using Core.Repositories;
using FluentAssertions;
using Moq;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new Mock<IOrderRepository>();
        private readonly Mock<IUserClient> _userClientMock = new Mock<IUserClient>();
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderService = new OrderService(_orderRepositoryMock.Object, _userClientMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_UserDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            int externalUserId = 123;
            _orderRepositoryMock.Setup(repo => repo.UserExistsAsync(externalUserId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.CreateOrderAsync(new CreateOrderDto { UserId = externalUserId }));
            exception.Message.Should().Contain($"User with ID {externalUserId} not found.");

            _orderRepositoryMock.Verify(repo => repo.UserExistsAsync(externalUserId), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            int orderId = 456;
            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetOrderByIdAsync(orderId));
            exception.Message.Should().Contain($"Order with ID {orderId} not found.");

            _orderRepositoryMock.Verify(repo => repo.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_OrderDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            int orderId = 789;
            _orderRepositoryMock.Setup(repo => repo.DeleteOrderAsync(orderId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.DeleteOrderAsync(orderId));
            exception.Message.Should().Contain($"Order with ID {orderId} could not be deleted or does not exist.");

            _orderRepositoryMock.Verify(repo => repo.DeleteOrderAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task MarkOrderAsPaidAsync_OrderDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            int orderId = 1011;
            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.MarkOrderAsPaidAsync(orderId));
            exception.Message.Should().Contain($"Order with ID {orderId} not found.");

            _orderRepositoryMock.Verify(repo => repo.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task MarkOrderAsCompletedAsync_OrderNotPaid_ThrowsPaymentRequiredException()
        {
            // Arrange
            var order = new Order { OrderId = 1213, IsPaid = false };
            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(order.OrderId))
                .ReturnsAsync(order);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PaymentRequiredException>(() => _orderService.MarkOrderAsCompletedAsync(order.OrderId));
            exception.Message.Should().Contain($"Order with ID {order.OrderId} is not paid.");

            _orderRepositoryMock.Verify(repo => repo.GetOrderByIdAsync(order.OrderId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_UserExists_CreatesOrderAndReturnsOrderDto()
        {
            // Arrange
            var userId = 123;
            var createOrderDto = new CreateOrderDto
            {
                UserId = userId,
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ItemId = 1, Quantity = 2 }
                }
            };
            var expectedOrder = new Order
            {
                OrderId = 1,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsPaid = false,
                IsCompleted = false,
                Items = createOrderDto.Items.Select(i => new OrderItem
                {
                    ItemId = i.ItemId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _orderRepositoryMock.Setup(repo => repo.UserExistsAsync(userId)).ReturnsAsync(true);
            _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync(expectedOrder.OrderId);
            _orderRepositoryMock.Setup(repo => repo.InsertOrderItemAsync(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CreateOrderAsync(createOrderDto);

            // Assert
            result.OrderId.Should().Be(expectedOrder.OrderId);
            _orderRepositoryMock.Verify(repo => repo.UserExistsAsync(userId), Times.Once);
            _orderRepositoryMock.Verify(repo => repo.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
            _orderRepositoryMock.Verify(repo => repo.InsertOrderItemAsync(It.IsAny<OrderItem>()), Times.Exactly(createOrderDto.Items.Count));
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_OrdersExist_ReturnsOrderDtos()
        {
            // Arrange
            var userId = 123;
            var orders = new List<Order>
            {
                new Order { OrderId = 1, UserId = userId, CreatedAt = DateTime.UtcNow, IsPaid = true, IsCompleted = false },
                new Order { OrderId = 2, UserId = userId, CreatedAt = DateTime.UtcNow, IsPaid = false, IsCompleted = true }
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserIdAsync(userId)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetOrdersByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.All(o => o.UserId == userId).Should().BeTrue();
            _orderRepositoryMock.Verify(repo => repo.GetOrdersByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_OrderExists_DeletesOrder()
        {
            // Arrange
            var orderId = 789;

            _orderRepositoryMock.Setup(repo => repo.DeleteOrderAsync(orderId)).ReturnsAsync(true);

            // Act
            var result = await _orderService.DeleteOrderAsync(orderId);

            // Assert
            result.Should().BeTrue();
            _orderRepositoryMock.Verify(repo => repo.DeleteOrderAsync(orderId), Times.Once);
        }
    }
}
