using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int key = 1)
        {
            List<Product> products;
            switch (key)
            {
                case 1:
                    products= await _context.Products.OrderBy(p=>p.Name).Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)).Take(8).ToListAsync();
                    
                    break;
                case 2:
                    products = await _context.Products.OrderBy(p => p.Price).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).Take(8).ToListAsync();
                    break;
                case 3:
                    products = await _context.Products.OrderBy(p => p.Id).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).Take(8).ToListAsync();
                    break;
                default:
                    products = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).Take(8).ToListAsync();
                    break;



            }
            return View(products);
        }
    }
}
