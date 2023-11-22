using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class TagController : Controller
	{
		private readonly AppDbContext _context;

		public TagController(AppDbContext context)
        {
			_context = context;
        }
        public async Task<IActionResult> Index()
		{
			List<Tag> tags = await _context.Tags.Include(t => t.ProductTags).ToListAsync();
			return View(tags);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Tag tag)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			
			bool result = _context.Tags.Any(c => c.Name.ToLower().Trim() == tag.Name.ToLower().Trim());
			if (result)
			{
				ModelState.AddModelError("Name", "Tag already exists");
				return View();
			}

			await _context.Tags.AddAsync(tag);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

	}
}
