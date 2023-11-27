using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.Areas.ViewModels;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
             List<Product> products= await _context.Products
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=> p.Category)
                .Include(p=> p.ProductImages
                .Where(pi=>pi.IsPrimary==true)) .ToListAsync();

            return View(products);
        }


		public async Task<IActionResult> Create()
		{
			ViewBag.Categories= await _context.Category.ToListAsync();
            ViewBag.Tags= await _context.Tags.ToListAsync();
            ViewBag.Sizes= await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();

			return View();
		}
        [HttpPost]
		public async Task<IActionResult> Create(CreateProductVM createProductVM)
		{
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Category.AnyAsync(c => c.Id == createProductVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError("CategoryId","Category doesn't exists");
            }
            foreach (int TagId in createProductVM.TagIDs)
            {
               bool tagResult= await _context.Tags.AnyAsync(t=> t.Id==TagId);
                if (!tagResult)
                {
					ViewBag.Categories = await _context.Category.ToListAsync();
					ViewBag.Tags = await _context.Tags.ToListAsync();
					ViewBag.Sizes = await _context.Sizes.ToListAsync();
					ViewBag.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("TagIds","Yanlis tag melumatlari gonderilib");
					return View();
				}
            }

            foreach (int id in createProductVM.SizeIDs)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(s=>s.Id==id);
                if (!sizeResult)
                {
					ViewBag.Categories = await _context.Category.ToListAsync();
					ViewBag.Tags = await _context.Tags.ToListAsync();
					ViewBag.Sizes = await _context.Sizes.ToListAsync();
					ViewBag.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("SizeIds", "Yanlis size melumatlari gonderilib");
					return View();
				}
            }
			foreach (int id in createProductVM.ColorIDs)
			{
				bool colorResult = await _context.Colors.AnyAsync(c => c.Id == id);
				if (!colorResult)
				{
					ViewBag.Categories = await _context.Category.ToListAsync();
					ViewBag.Tags = await _context.Tags.ToListAsync();
					ViewBag.Sizes = await _context.Sizes.ToListAsync();
					ViewBag.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("SizeIds", "Yanlis color melumatlari gonderilib");
					return View();
				}
			}

			Product product = new Product
            {
                Name = createProductVM.Name,
                CategoryId = createProductVM.CategoryId,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                SKU = createProductVM.SKU,
                ProductTags = new List<ProductTag>(),
                ProductSizes= new List<ProductSize>(),
                ProductColors= new List<ProductColor>()

			};

            foreach (int id in createProductVM.SizeIDs)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = id
                };
                product.ProductSizes.Add(productSize);
            }

			
            foreach (int tagID in createProductVM.TagIDs)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagID,
                    
                    
                };
                product .ProductTags.Add(productTag);
            }
			foreach (int colorID in createProductVM.ColorIDs)
			{
				ProductColor productColor = new ProductColor
				{
					ColorId = colorID


				};
				product.ProductColors.Add(productColor);
			}


			_context.Products.Add(product);
            await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Details(int id)
		{
			Product product = await _context.Products.Include(p=>p.ProductColors.FirstOrDefault(pc=>pc.ProductId==id))
                .Include(p=>p.ProductImages.FirstOrDefault(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p=> p.Id==id);

			return View();
		}
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.Include(p=>p.ProductSizes).Include(p=>p.ProductTags).FirstOrDefaultAsync(p=>p.Id==id);
            if (product == null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Categories= await _context.Category.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                TagIDs= product.ProductTags.Select(pt=>pt.TagId).ToList(),
                Sizes = await _context.Sizes.ToListAsync(),
                SizeIDs= product.ProductSizes.Select(ps=>ps.SizeId).ToList()
                
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories= await _context.Category.ToListAsync();
                productVM.Tags= await _context.Tags.ToListAsync();
                return View(productVM);
            }
            Product existed = await _context.Products.Include(p=> p.ProductTags).FirstOrDefaultAsync(p=>p.Id==id);
            if (existed == null)
            {
                return NotFound();
            }
            bool result = await _context.Category.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Category.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				return View();
            }
           
            foreach (ProductTag productTag in existed.ProductTags)
            {
                if (!productVM.TagIDs.Exists(tID=>tID==productTag.TagId))
                {
                    _context.ProductTags.Remove(productTag);
                }
            }
            List<int> NewTagIDs= new List<int>();
            foreach (int TagId in productVM.TagIDs)
            {
                if (!existed.ProductTags.Any(pt=>pt.TagId==TagId))
                {

                    existed.ProductTags.Add(new ProductTag { TagId=TagId});
                }
            }

            existed.Name=productVM.Name;
            existed.Description=productVM.Description;
            existed.SKU=productVM.SKU;
            existed.Price=productVM.Price;
            existed.CategoryId=productVM.CategoryId;
            
            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

	}
}
