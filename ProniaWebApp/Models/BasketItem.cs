namespace ProniaWebApp.Models
{
	public class BasketItem:BaseEntity
	{
		public int ProductId { get; set; }
		public Product Product { get; set; }
		public int Count { get; set; }
		public double Price { get; set; }
		public string AppUserId { get; set; }
		public AppUser AppUser { get; set; }
		public int? OrderId { get; set; }
		public Order? Order { get; set; }
	}
}
