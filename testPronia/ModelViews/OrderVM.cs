using testPronia.Models;

namespace testPronia.ModelViews
{
	public class OrderVM
	{
		public string Address { get; set; }
		public List<BasketItem>? BasketItems { get; set; }
	}
}
