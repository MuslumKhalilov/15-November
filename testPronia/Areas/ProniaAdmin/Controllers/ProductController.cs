using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
             List<Product> products= await _context.Products.Include(p=> p.Category).Include(p=> p.ProductImages.Where(pi=>pi.IsPrimary==true)) .ToListAsync();

            return View(products);
        }
    }
}
