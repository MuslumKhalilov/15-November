using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class ColorController : Controller
	{
		private readonly AppDbContext _context;

		public ColorController(AppDbContext context)
        {
            _context= context;
        }
        public IActionResult Index()
		{
			List<Models.Color> colors = _context.Colors.ToList();

			return View(colors);
		}
		[Authorize(Roles = "Admin,Moderator")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Models.Color color)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			bool result = _context.Colors.Any(c => c.Name.ToLower().Trim() == color.Name.ToLower().Trim());
			if (result)
			{
				ModelState.AddModelError("Name", "Color already exists");
				return View();
			}

			await _context.Colors.AddAsync(color);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));

		}
		[Authorize(Roles = "Admin")]
		async public Task<IActionResult> Delete(int id)
		{
			if (id <= 0) return BadRequest();
			Models.Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

			_context.Colors.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			Models.Color colors = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
			return View(colors);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int id, Models.Color color)
		{
			if (!ModelState.IsValid) { return View(); }

			Models.Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

			if (existed == null) return NotFound();

			bool result = _context.Colors.Any(c => c.Name == color.Name && c.Id == id);
			if (result) { ModelState.AddModelError("Name", "Color already exists"); return View(); }

			existed.Name = color.Name;

			_context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}


		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Details(int id)
        {

            List<ProductColor> productColors = await _context.ProductColors.Include(pc => pc.Product.ProductImages).Where(c => c.ColorId == id).ToListAsync();


            return View(productColors);
        }
    }
}
