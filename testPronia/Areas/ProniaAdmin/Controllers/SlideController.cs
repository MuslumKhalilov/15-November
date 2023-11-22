using Microsoft.AspNetCore.Mvc;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class SlideController : Controller
	{
		private readonly AppDbContext _context;

		public SlideController(AppDbContext context)
        {
			_context = context;   
        }
        public IActionResult Index()
		{
            List<Slide> slides = _context.Slides.ToList();
			return View(slides);
		}
	}
}
