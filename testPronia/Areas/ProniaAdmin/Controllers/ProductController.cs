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
            ViewBag.Tags= await _context.Tags.ToListAsync();

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
		public async Task<IActionResult> Details(int id)
		{
			Product product = await _context.Products.Include(p=>p.ProductColors.FirstOrDefault(pc=>pc.ProductId==id))
                .Include(p=>p.ProductImages.FirstOrDefault(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p=> p.Id==id);

			return View();
		}
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);
            if (product == null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Categories= await _context.Category.ToListAsync()
                
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories= await _context.Category.ToListAsync();
                return View(productVM);
            }
            Product existed = await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);
            if (existed == null)
            {
                return NotFound();
            }
            bool result = await _context.Category.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Category.ToListAsync();
                return View();
            }
            existed.Name=productVM.Name;
            existed.Description=productVM.Description;
            existed.SKU=productVM.SKU;
            existed.Price=productVM.Price;
            existed.CategoryId=productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

	}
}
