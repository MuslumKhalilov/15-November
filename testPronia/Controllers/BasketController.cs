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

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();

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


            return Ok();
        }
        public IActionResult Info()
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}
