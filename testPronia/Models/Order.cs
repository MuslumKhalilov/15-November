namespace testPronia.Models
{
	public class Order
	{
		public int Id { get; set; }
		public decimal TotalPrice { get; set; }
		public bool? Status { get; set; }
		public string Address { get; set; }
		public DateTime PurchaseDate { get; set; }
		public string AppUserId { get; set; }
		public AppUser AppUser { get; set; }
		public List<BasketItem> BasketItems { get; set; }
	}
}
