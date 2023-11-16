using Microsoft.AspNetCore.Mvc;
using testPronia.DAL;
using testPronia.Models;

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
            List<Product> products = _context.Products.Take(8).OrderBy(p=>p.Id).ToList();
            return View(products);
        }
    }
}
