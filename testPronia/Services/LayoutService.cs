using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<AppUser> _userManager;

		public LayoutService(AppDbContext context, IHttpContextAccessor http,UserManager<AppUser> userManager)
        {
            _context = context;
            _http=http;
			_userManager=userManager;	
        }
        public async Task<Dictionary<string,string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        } 
        public async Task<List<BasketItemVM>> GetBasketItemAsync()
        {
			List<BasketItemVM> basketItems = new List<BasketItemVM>();
			if (_http.HttpContext.User.Identity.IsAuthenticated)
			{
				AppUser? user = await _userManager.Users
					.Include(u => u.BasketItems)
					.ThenInclude(bi => bi.Product)
					.ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
					.FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
				foreach (BasketItem item in user.BasketItems)
				{
					BasketItemVM itemVM = new BasketItemVM()
					{
						Name = item.Product.Name,
						Price = item.Price,
						Count = item.Count,
						Id = item.Id,
						Subtotal = item.Price * item.Count,
						Image = item.Product.ProductImages.FirstOrDefault()?.Url
					};
					basketItems.Add(itemVM);
				}
			}
            else
            {
				if (_http.HttpContext.Request.Cookies["Basket"] is not null)
				{
					List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);

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
				}
			}

            
			return basketItems;
		}
    }
}
