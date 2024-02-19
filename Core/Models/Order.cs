namespace Core.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCompleted { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
