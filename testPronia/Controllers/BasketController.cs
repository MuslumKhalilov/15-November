using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using testPronia.DAL;
using testPronia.Models;
using testPronia.ModelViews;

namespace testPronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public BasketController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
			List<BasketItemVM> basketItems = new List<BasketItemVM>();
			
			if (User.Identity.IsAuthenticated)
			{
				AppUser? user = await  _userManager.Users
					.Include(u=>u.BasketItems)
					.ThenInclude(bi=>bi.Product)
					.ThenInclude(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
					.FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in user.BasketItems)
                {
					BasketItemVM itemVM = new BasketItemVM()
					{
						Name= item.Product.Name,
						Price= item.Price,
						Count = item.Count,
						Id=item.Id,
						Subtotal = item.Price * item.Count,
						Image = item.Product.ProductImages.FirstOrDefault()?.Url
					};
					basketItems.Add(itemVM);
                }


            }
			else
			{
				if (Request.Cookies["Basket"] is not null)
				{
					List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

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

            return View(basketItems);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
				AppUser user = await _userManager.Users.Include(u=>u.BasketItems).FirstOrDefaultAsync(u=>u.Id==User.FindFirst(ClaimTypes.NameIdentifier).Value);
				if (user is null) { return NotFound(); }
				BasketItem basket= user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
				if (basket is null)
				{
					basket = new BasketItem()
					{
						AppUserId = user.Id,
						ProductId = product.Id,
						Count = 1,
						Price = product.Price
					};
					_context.BasketItems.AddAsync(basket);
				}
				else
				{
					basket.Count++;
				}
			
				
				await _context.SaveChangesAsync();
            }
            else
            {
				List<BasketCookieItemVM> basket;
				if (Request.Cookies["Basket"] != null)
				{
					basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
					BasketCookieItemVM cookieItemVM = basket.FirstOrDefault(c => c.Id == id);
					if (cookieItemVM == null)
					{
						BasketCookieItemVM basketCookie = new BasketCookieItemVM()
						{
							Id = id,
							Count = 1

						};
						basket.Add(basketCookie);
					}
					else
					{
						cookieItemVM.Count++;
					}
				}
				else
				{
					basket = new List<BasketCookieItemVM>();
					BasketCookieItemVM basketCookieItem = new BasketCookieItemVM()
					{
						Id = id,
						Count = 1
					};
					basket.Add(basketCookieItem);


				}
				string json = JsonConvert.SerializeObject(basket);
				Response.Cookies.Append("Basket", json);
			}
           


            return Ok();
        }
        public IActionResult Info()
        {
            return Content(Request.Cookies["Basket"]);
        }

        public IActionResult PlusCount(int id)
        {
            List<BasketCookieItemVM> basketCookieItemVMs = new List<BasketCookieItemVM>();
            BasketCookieItemVM basketCookieItemVM = basketCookieItemVMs.FirstOrDefault(b => b.Id == id);
            basketCookieItemVM.Count++;
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            List<BasketCookieItemVM> basketCookieItems =  JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM itemVM = basketCookieItems.FirstOrDefault(b => b.Id == id);
            basketCookieItems.Remove(itemVM);
            string json = JsonConvert.SerializeObject(basketCookieItems);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction("Index","Basket");
        }
		public async Task<IActionResult> DeleteFromDb(int id)
		{			
			BasketItem item = await _context.BasketItems.FirstOrDefaultAsync(i => i.Id == id);
			if (item == null) return NotFound();
			_context.BasketItems.Remove(item);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index","Basket");
			
			
		}
		public IActionResult Checkout()
		{
			return View();
		}
    }
}
