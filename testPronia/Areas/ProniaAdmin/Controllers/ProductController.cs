using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.Areas.ViewModels;
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


		public async Task<IActionResult> Create()
		{
			ViewBag.Categories= await _context.Category.ToListAsync();

			return View();
		}
        [HttpPost]
		public async Task<IActionResult> Create(CreateProductVM createProductVM)
		{
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Category.AnyAsync(c => c.Id == createProductVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError("CategoryId","Category doesn't exists");
            }
            Product product = new Product
            {
                Name = createProductVM.Name,
                CategoryId = createProductVM.CategoryId,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                SKU = createProductVM.SKU
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

			return View(nameof(Index));
		}
	}
}
