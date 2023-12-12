using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using testPronia.Areas.ViewModels;
using testPronia.DAL;
using testPronia.Models;
using testPronia.Utilities.Extensions;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env= env;  
        }

        public async Task<IActionResult> Index(int page)
        {
			double count = await _context.Products.CountAsync();
			if (page<0)
			{
				return BadRequest();
			}
			else if (page > Math.Ceiling(count / 3))
			{
				return NotFound();
			}

			ViewBag.PageCount = 6;
			List<Product> products= await _context.Products
				.Skip(3 * page).Take(3)
				.Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=> p.Category)
                .Include(p=> p.ProductImages
                .Where(pi=>pi.IsPrimary==true)) .ToListAsync();
			PaginateVM<Product> paginateVM = new PaginateVM<Product>()
			{
				TotalPage = Math.Ceiling(count / 3),
				CurrentPage = page + 1,
				Items = products
		};

            return View(paginateVM);
        }

		[Authorize(Roles = "Admin,Moderator")]
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

            if (!createProductVM.MainPhoto.ValidateType("image/"))
            {
				ViewBag.Categories = await _context.Category.ToListAsync();
				ViewBag.Tags = await _context.Tags.ToListAsync();
				ViewBag.Sizes = await _context.Sizes.ToListAsync();
				ViewBag.Colors = await _context.Colors.ToListAsync();
				ModelState.AddModelError("MainPhoto", "File tipi uyqun deyil");
				return View();
			}
            if (!createProductVM.MainPhoto.ValidateSize(600))
            {
				ViewBag.Categories = await _context.Category.ToListAsync();
				ViewBag.Tags = await _context.Tags.ToListAsync();
				ViewBag.Sizes = await _context.Sizes.ToListAsync();
				ViewBag.Colors = await _context.Colors.ToListAsync();
				ModelState.AddModelError("MainPhoto", "File olcusu uyqun deyil");
				return View();
			}


			if (!createProductVM.HoverPhoto.ValidateType("image/"))
			{
				ViewBag.Categories = await _context.Category.ToListAsync();
				ViewBag.Tags = await _context.Tags.ToListAsync();
				ViewBag.Sizes = await _context.Sizes.ToListAsync();
				ViewBag.Colors = await _context.Colors.ToListAsync();
				ModelState.AddModelError("HoverPhoto", "File tipi uyqun deyil");
				return View();
			}
			if (!createProductVM.HoverPhoto.ValidateSize(600))
			{
				ViewBag.Categories = await _context.Category.ToListAsync();
				ViewBag.Tags = await _context.Tags.ToListAsync();
				ViewBag.Sizes = await _context.Sizes.ToListAsync();
				ViewBag.Colors = await _context.Colors.ToListAsync();
				ModelState.AddModelError("HoverPhoto", "File olcusu uyqun deyil");
				return View();
			}

            ProductImage mainImage = new ProductImage
            {
                Alternative= createProductVM.Name,
                IsPrimary = true,
                Url = await createProductVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
            };
			ProductImage hoverImage = new ProductImage
			{
				Alternative = createProductVM.Name,
				IsPrimary = false,
				Url = await createProductVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
			};

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
                ProductColors= new List<ProductColor>(),
                ProductImages= new List<ProductImage> { mainImage,hoverImage}

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
            TempData["Message"] = "";
            foreach (IFormFile photo in createProductVM.Photos)
            {
				if (!photo.ValidateType("image/"))
				{
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uyqun deyil</p>";
                    continue;
				}
				if (!photo.ValidateSize(600))
				{
					TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uyqun deyil</p>";
					continue;
				}
                product.ProductImages.Add(new ProductImage { IsPrimary=null,Url= await photo.CreateFile(_env.WebRootPath,"assets", "images", "website-images"),Alternative=createProductVM.Name});
			}


			_context.Products.Add(product);
            await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Details(int id)
		{
			Product product = await _context.Products.Include(p=>p.ProductColors.FirstOrDefault(pc=>pc.ProductId==id))
                .Include(p=>p.ProductImages.FirstOrDefault(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p=> p.Id==id);

			return View();
		}
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.Include(p=>p.ProductImages).Include(p=>p.ProductColors).Include(p=>p.ProductSizes).Include(p=>p.ProductTags).FirstOrDefaultAsync(p=>p.Id==id);
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
                SizeIDs= product.ProductSizes.Select(ps=>ps.SizeId).ToList(),
                Colors= await _context.Colors.ToListAsync(),
                ColorIDs= product.ProductColors.Select(ps=>ps.ColorId).ToList(),
                ProductImages=product.ProductImages
                
                
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
        {
			Product existed = await _context.Products.Include(p=>p.ProductImages).Include(p => p.ProductColors).Include(p => p.ProductSizes).Include(p => p.ProductTags).FirstOrDefaultAsync(p => p.Id == id);
            productVM.ProductImages = existed.ProductImages;
			if (!ModelState.IsValid)
            {
                productVM.Categories= await _context.Category.ToListAsync();
                productVM.Tags= await _context.Tags.ToListAsync();
				productVM.Sizes= await _context.Sizes.ToListAsync();
                productVM.Colors= await _context.Colors.ToListAsync();
				return View(productVM);
            }
            
            if (existed == null)
            {
                return NotFound();
            }
            bool result = await _context.Category.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Category.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
				return View(productVM);
            }

           /* existed.ProductTags.RemoveAll(pt=> !productVM.TagIDs.Exists(tID=> tID==pt.TagId));*/ 
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

			foreach (ProductSize productSize in existed.ProductSizes)
			{
				if (!productVM.SizeIDs.Exists(sID => sID == productSize.SizeId))
				{
					_context.ProductSizes.Remove(productSize);
				}
			}
			List<int> NewSizeIDs = new List<int>();
			foreach (int SizeId in productVM.SizeIDs)
			{
				if (!existed.ProductSizes.Any(pt => pt.SizeId == SizeId))
				{

					existed.ProductSizes.Add(new ProductSize { SizeId=SizeId });
				}
			}

			foreach (ProductColor productColor in existed.ProductColors)
			{
				if (!productVM.ColorIDs.Exists(cID => cID == productColor.ColorId))
				{
					_context.ProductColors.Remove(productColor);
				}
			}
			List<int> NewColorIDs = new List<int>();
			foreach (int ColorId in productVM.ColorIDs)
			{
				if (!existed.ProductColors.Any(pt => pt.ColorId == ColorId))
				{

					existed.ProductColors.Add(new ProductColor { ColorId = ColorId });
				}
			}
            if (productVM.MainPhoto is not null) 
            {
				if (!productVM.MainPhoto.ValidateType("image/"))
				{
					productVM.Categories = await _context.Category.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File tipi uyqun deyil");
					return View(productVM);
				}
				if (!productVM.MainPhoto.ValidateSize(600))
				{
					productVM.Categories = await _context.Category.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File olcusu uyqun deyil");
					return View(productVM);
				}
			}
			if (productVM.HoverPhoto is not null)
			{
				if (!productVM.HoverPhoto.ValidateType("image/"))
				{
					productVM.Categories = await _context.Category.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File tipi uyqun deyil");
					return View(productVM);
				}
				if (!productVM.HoverPhoto.ValidateSize(600))
				{
					productVM.Categories = await _context.Category.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File olcusu uyqun deyil");
					return View(productVM);
				}
			}
			if (productVM.ProductImages is not null)
			{
				string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage mainImage= existed.ProductImages.FirstOrDefault(pi=>pi.IsPrimary==true);
				mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				_context.ProductImages.Remove(mainImage);
				existed.ProductImages.Add(new ProductImage
				{
					Alternative=productVM.Name,
					IsPrimary=true,
					Url=fileName
				});
				
			}
			if (productVM.ProductImages is not null)
			{
				string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
				hoverImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				_context.ProductImages.Remove(hoverImage);
				existed.ProductImages.Add(new ProductImage
				{
					Alternative = productVM.Name,
					IsPrimary = false,
					Url = fileName
				});

			}
			if (productVM.ProductImages == null)
			{
				productVM.ProductImages = new List<ProductImage>();
			}
			List<ProductImage> removable = existed.ProductImages.Where(pi => !productVM.ImageIDs.Exists(ImgId=>ImgId==pi.Id)&& pi.IsPrimary==null).ToList();
			foreach (ProductImage image in removable)
			{
				image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				existed?.ProductImages.Remove(image);
			}

			TempData["Message"] = "";
			foreach (IFormFile photo in productVM.Photos)
			{
				if (!photo.ValidateType("image/"))
				{
					TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uyqun deyil</p>";
					continue;
				}
				if (!photo.ValidateSize(600))
				{
					TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uyqun deyil</p>";
					continue;
				}
				existed.ProductImages.Add(new ProductImage { IsPrimary = null, Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images"), Alternative = productVM.Name });
			}

			existed.Name=productVM.Name;
            existed.Description=productVM.Description;
            existed.SKU=productVM.SKU;
            existed.Price=productVM.Price;
            existed.CategoryId=productVM.CategoryId;
            
            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
			if (existed == null) return NotFound();

			foreach (ProductImage image in existed.ProductImages)
			{
				image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
			}

            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
		

	}
}
