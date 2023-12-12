using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;
using testPronia.ModelViews;
using testPronia.Utilities.Exceptions;

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
            if (id <= 0) throw new WrongRequestException("Gonderilen sorqunuz yanlishdir");
            
            Product product=_context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pt => pt.Color)
                .Include(p => p.ProductSizes).ThenInclude(pt => pt.Size)
                .FirstOrDefault(p => p.Id == id);
            if (product == null) throw new NotFoundException("Bele bir mehsul tapilmaadi");
            List<Product> ReleatedProducts=_context.Products.Include(p=> p.ProductImages).Where(p => p.CategoryId==product.CategoryId && p.Id != product.Id).ToList();
            DetailVM DetailVm = new() {Product=product,Products=ReleatedProducts };
            return View(DetailVm);
        }

    }
}
