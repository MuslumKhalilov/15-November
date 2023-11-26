using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class SlideController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		

		public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
			_context = context;
			_env = env;
        }
		
        public IActionResult Index()
		{
            List<Slide> slides = _context.Slides.ToList();
			return View(slides);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Slide slide)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (slide.Photo == null)
			{
				ModelState.AddModelError("Photo","Photo required");
				return View();
				
			}

			if (slide.Photo.ContentType.Contains("image/"))
			{
				ModelState.AddModelError("Photo","Incorrect file type");
			}

			if(slide.Photo.Length> 2 * 1024 * 1024)
			{
				ModelState.AddModelError("Photo", "Photo size should not be larger than 2 mb");
				
			}
			if (slide.Order <= 0)
			{
				ModelState.AddModelError("Order", "Order cannot be less than 0");
			}

			string fileName= Guid.NewGuid().ToString()+ slide.Photo.FileName;
			string path = Path.Combine(_env.WebRootPath, @"\assets\images\slider", fileName);
			FileStream fileStream = new FileStream(path, FileMode.Create);

			
			await slide.Photo.CopyToAsync(fileStream);
			fileStream.Close();
			slide.SlideImageUrl = fileName;

			await _context.AddAsync(slide);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int id)
		{
			Slide slide= await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			return View(slide);
		}
	}
}
