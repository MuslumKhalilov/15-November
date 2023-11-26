using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
            
        }
        public IActionResult Index()
        {
            List<Size> sizes = _context.Sizes.ToList();
            return View(sizes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Sizes.Any(s => s.Name.ToLower().Trim() == size.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Size already exists");
                return View();
            }

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        async public Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Size existed = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);

            _context.Sizes.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            return View(size);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Size size)
        {
            if (!ModelState.IsValid) { return View(); }

            Size existed = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            bool result = _context.Sizes.Any(s => s.Name == size.Name && s.Id == id);
            if (result) { ModelState.AddModelError("Name", "Size already exists"); return View(); }

            existed.Name = size.Name;

            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



        public async Task<IActionResult> Details(int id)
        {

            List<ProductSize> productSizes = await _context.ProductSizes.Include(ps => ps.Product.ProductImages).Where(s => s.SizeId == id).ToListAsync();


            return View(productSizes);
        }

    }
}
