using Microsoft.AspNetCore.Mvc;
using testPronia.DAL;
using testPronia.Models;
using testPronia.ModelViews;

namespace testPronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;   
        }
        List<Slide> Slides = new()
        {
            new Slide() {Id=1,Title="Title1",SubTitle="Subtitle1",Description="Description1",SlideImageUrl="1-1-524x617.png"},

            new Slide() {Id=1,Title="Title2",SubTitle="Subtitle2",Description="Description2",SlideImageUrl="1-2-524x617.png"} 
        };
       
        
        public IActionResult Index()
        {
            
            List<Product> Products = _context.Products.Take(8).OrderBy(p=>p.Id).ToList();

            List<Slide> Slides = new()
        {
            new Slide() {Id=1,Title="Title1",SubTitle="Subtitle1",Description="Description1",SlideImageUrl="1-1-524x617.png"},

            new Slide() {Id=1,Title="Title2",SubTitle="Subtitle2",Description="Description2",SlideImageUrl="1-2-524x617.png"}
        };

            VM vm = new() {slides=Slides, products=Products };

            return View(vm);
        }
    }
}
