using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        //public IActionResult Index()
        //{
        //    return View();
        //}
        public ProductController(AppDbContext context)
        {
            _context=context;
        }

        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product product=_context.Products.Include(p => p.Category).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

    }
}
