using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
      
       
        
        public IActionResult Index()
        {
            
            List<Product> Products = _context.Products.Include(p=> p.ProductImages).Take(8).OrderBy(p=>p.Id).ToList();

            List<Slide> Slides = _context.Slides.Take(2).OrderBy(s => s.Order).ToList();
        

            VM vm = new() {slides=Slides, products=Products };

            return View(vm);
        }
        public IActionResult ErrorPage(string error)
        {

            return View(model: error);
        }
    }
}
