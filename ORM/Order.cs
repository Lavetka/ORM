namespace ORM
{
	public class Order
	{
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int ProductId { get; set; }
        public string Status { get; set; }

        public Product Product { get; set; }
    }
}

public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered,
    Cancelled
}