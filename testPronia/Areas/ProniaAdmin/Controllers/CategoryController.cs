using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using testPronia.Areas.ViewModels;
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

        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Category.CountAsync();
			if (page < 0)
			{
				return BadRequest();
			}
			else if (page > Math.Ceiling(count / 3))
			{
				return NotFound();
			}
			List<Category> categories = await _context.Category.Skip(page*3).Take(3).Include(c=> c.Products).ToListAsync();
            PaginateVM<Category> paginateVM = new PaginateVM<Category>()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = categories
            };
            return View(paginateVM);
        }
        [Authorize(Roles ="Admin,Moderator")]
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
            
            return RedirectToAction(nameof(Index));
            
        }
		[Authorize(Roles = "Admin,Moderator")]
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
            return RedirectToAction(nameof(Index));

        }
		[Authorize(Roles = "Admin")]
		async public Task<IActionResult> Delete(int id)
        {
            if (id <=0) return BadRequest();
            Category existed = await _context.Category.FirstOrDefaultAsync(c => c.Id==id);

            _context.Category.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
            


        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Details(int id)
        {
            Category category = await _context.Category.FirstOrDefaultAsync(c => c.Id==id);
            List<Product> products = await _context.Products.Include(p=>p.ProductImages).Where(p=> p.CategoryId==id).ToListAsync();
            return View(products);
        }
    }
}
