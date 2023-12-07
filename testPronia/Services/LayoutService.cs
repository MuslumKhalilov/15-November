using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using testPronia.DAL;
using testPronia.Models;
using testPronia.ModelViews;

namespace testPronia.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
		private readonly IHttpContextAccessor _http;

		public LayoutService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http=http;
        }
        public async Task<Dictionary<string,string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        } 
        public async Task<List<BasketItemVM>> GetBasketItemAsync()
        {
			List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);
			List<BasketItemVM> basketItems = new List<BasketItemVM>();
			foreach (BasketCookieItemVM item in basket)
			{
				Product product = _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefault(p => p.Id == item.Id);
				if (product != null)
				{
					BasketItemVM itemVM = new BasketItemVM()
					{
						Id = item.Id,
						Count = item.Count,
						Price = product.Price,
						Image = product.ProductImages.FirstOrDefault().Url,
						Name = product.Name,
						Subtotal = item.Count * product.Price

					};
					basketItems.Add(itemVM);
				}
			}
			return basketItems;
		}
    }
}
