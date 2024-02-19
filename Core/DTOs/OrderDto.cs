using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCompleted { get; set; }
        public List<OrderItemDto>? Items { get; set; }
    }
}
