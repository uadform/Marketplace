using Core.Models;
using Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _dbConnection;

        public OrderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            var sqlOrderInsert = @"
            INSERT INTO orders (user_id, created_at, is_paid, is_completed)
            VALUES (@UserId, @CreatedAt, @IsPaid, @IsCompleted)
            RETURNING order_id;";

            var orderId = await _dbConnection.QuerySingleAsync<int>(sqlOrderInsert, order);
            return orderId;
        }

        public async Task InsertOrderItemAsync(OrderItem orderItem)
        {
            var sqlOrderItemInsert = @"
            INSERT INTO order_items (order_id, item_id, quantity)
            VALUES (@OrderId, @ItemId, @Quantity);";

            await _dbConnection.ExecuteAsync(sqlOrderItemInsert, orderItem);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var deleteOrderItemsSql = @"
            DELETE FROM order_items
            WHERE order_id = @OrderId;";

            await _dbConnection.ExecuteAsync(deleteOrderItemsSql, new { OrderId = orderId });

            var deleteOrderSql = @"
            DELETE FROM orders
            WHERE order_id = @OrderId;";

            var rowsAffected = await _dbConnection.ExecuteAsync(deleteOrderSql, new { OrderId = orderId });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var orderSql = @"
            SELECT order_id AS OrderId, user_id AS UserId, created_at AS CreatedAt, 
                   is_paid AS IsPaid, is_completed AS IsCompleted
            FROM orders 
            WHERE user_id = @UserId;";

            var orders = (await _dbConnection.QueryAsync<Order>(orderSql, new { UserId = userId })).ToList();

            if (!orders.Any())
            {
                return orders;
            }

            var orderIds = orders.Select(o => o.OrderId).ToList();
            var itemsSql = $@"
            SELECT order_item_id AS OrderItemId, order_id AS OrderId, item_id AS ItemId, 
                   quantity AS Quantity
            FROM order_items 
            WHERE order_id = @OrderId;";

            foreach (var order in orders)
            {
                if (order != null)
                {
                    var items = await _dbConnection.QueryAsync<OrderItem>(itemsSql, new { OrderId = order.OrderId });
                    order.Items = items.ToList();
                }
            }

            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var orderSql = @"
            SELECT order_id AS OrderId, user_id AS UserId, created_at AS CreatedAt, 
                   is_paid AS IsPaid, is_completed AS IsCompleted
            FROM orders 
            WHERE order_id = @OrderId";

            var itemSql = @"
            SELECT order_item_id AS OrderItemId, order_id AS OrderId, item_id AS ItemId, 
                   quantity AS Quantity
            FROM order_items 
            WHERE order_id = @OrderId";

            var order = await _dbConnection.QuerySingleOrDefaultAsync<Order>(orderSql, new { OrderId = orderId });
            if (order != null)
            {
                var items = await _dbConnection.QueryAsync<OrderItem>(itemSql, new { OrderId = orderId });
                order.Items = items.ToList();
            }

            return order;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var sql = @"
            UPDATE orders
            SET is_paid = @IsPaid, is_completed = @IsCompleted
            WHERE order_id = @OrderId;";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, order);
            return rowsAffected > 0;
        }

        public async Task DeleteUnpaidOrdersAsync()
        {
            var deleteItemsSql = @"
            DELETE FROM order_items
            WHERE order_id IN (
                SELECT order_id FROM orders
                WHERE is_paid = FALSE AND created_at <= (CURRENT_TIMESTAMP AT TIME ZONE 'UTC' - INTERVAL '2 hours')
            );";
            await _dbConnection.ExecuteAsync(deleteItemsSql);

            var deleteOrdersSql = @"
            DELETE FROM orders
            WHERE is_paid = FALSE AND created_at <= (CURRENT_TIMESTAMP AT TIME ZONE 'UTC' - INTERVAL '2 hours');";
            await _dbConnection.ExecuteAsync(deleteOrdersSql);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            var sql = "SELECT COUNT(1) FROM users WHERE user_id = @UserId";
            var exists = await _dbConnection.ExecuteScalarAsync<bool>(sql, new { UserId = userId });
            return exists;
        }
    }
}
