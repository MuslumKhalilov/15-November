using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Category.Include(c=> c.Products).ToListAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
				return View();
			}
            bool result = _context.Category.Any(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name","Category already exists");
                return View();
            }

            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
            
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (!ModelState.IsValid) {return View(); } 

            Category existed= await _context.Category.FirstOrDefaultAsync(c => c.Id == id);

            if (existed==null) return NotFound();
            
            bool result = _context.Category.Any(c => c.Name==category.Name && c.Id==id);
            if (result) { ModelState.AddModelError("Name","Category already exists"); return View();}

            existed.Name = category.Name;
            
            _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
