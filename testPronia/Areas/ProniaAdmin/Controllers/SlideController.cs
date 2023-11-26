using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using testPronia.Areas.ViewModels;
using testPronia.DAL;
using testPronia.Models;
using testPronia.Utilities.Extensions;

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
		public async Task<IActionResult> Create(CreateSlideVM slideVM)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (slideVM.Photo == null)
			{
				ModelState.AddModelError("Photo","Photo required");
				return View();
				
			}

			if (!slideVM.Photo.ValidateType("image/"))
			{
				ModelState.AddModelError("Photo","Incorrect file type");
				return View();
			}

			if(!slideVM.Photo.ValidateSize(2*1024))
			{
				ModelState.AddModelError("Photo", "Photo size should not be larger than 2 mb");
				return View();
				
			}
			if (slideVM.Order <= 0)
			{
				ModelState.AddModelError("Order", "Order cannot be less than 0");
			}
			

		     string fileName = await slideVM.Photo.CreateFile(_env.WebRootPath,"assets","images","slider");
			Slide slide = new Slide
			{
				Title = slideVM.Title,
				SubTitle = slideVM.SubTitle,
				Description = slideVM.Description,
				Order = slideVM.Order,
				SlideImageUrl=fileName
			};

			await _context.AddAsync(slide);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int id)
		{
			Slide slide= await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			return View(slide);
		}

		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0)
			{
				return BadRequest();
			}
			Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			slide.SlideImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
			_context.Slides.Remove(slide);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}
	}
}
