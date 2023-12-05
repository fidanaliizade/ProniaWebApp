using Microsoft.AspNetCore.Mvc;
using ProniaWebApp.Areas.Manage.ViewModels.Product;
using ProniaWebApp.Helpers;
using ProniaWebApp.Models;

namespace ProniaWebApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {

        AppDbContext _context { get; set; }
        IWebHostEnvironment _env;

		public ProductController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		public async Task<IActionResult> Index()
        {
            List<Product> product = await _context.Products.Where(p=>p.IsDeleted==false).Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag).Include(p=>p.ProductImages).ToListAsync();
            return View(product);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM createProductVM)
        {
			ViewBag.Categories = await _context.Categories.ToListAsync();
			ViewBag.Tags = await _context.Tags.ToListAsync();
            if(!ModelState.IsValid)
            {
                return View("Error");
            }
            bool resultCategory = await _context.Categories.AnyAsync(c=>c.Id== createProductVM.CategoryId);
            if(!resultCategory)
            {
                ModelState.AddModelError("CatgoryId", "No such category exists.");
                return View();
            }


            Product product = new Product()
            {
                Name = createProductVM.Name,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                ProductCode = createProductVM.ProductCode,
                CategoryId = createProductVM.CategoryId,
                ProductImages= new List<ProductImages>()
             
            };
            if (createProductVM.TagIds != null)
            {
                foreach (int tagId in createProductVM.TagIds)
                {
                    bool resultTag = await _context.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", "No such tag exists.");
                        return View();
                    }
                    ProductTag productTag = new ProductTag()
                    {
                        Product = product,
                        TagId = tagId
                    };



                    _context.ProductTags.Add(productTag);





                }
            }


            if (!createProductVM.MainPhoto.CheckType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "Enter right image format.");
                return View();
            }
			if (!createProductVM.MainPhoto.CheckLength(3000))
			{
				ModelState.AddModelError("MainPhoto", "Enter max 3mb image.");
                return View();
			}

			if (!createProductVM.HoverPhoto.CheckType("image/"))
			{
				ModelState.AddModelError("HoverPhoto", "Enter right image format.");
                return View();
			}
			if (!createProductVM.HoverPhoto.CheckLength(3000))
			{
				ModelState.AddModelError("HoverPhoto", "Enter max 3mb image.");
                return View();
			}

            ProductImages mainImage = new ProductImages()
            {
                IsPrime = true,
                ImgUrl = createProductVM.MainPhoto.Upload(_env.WebRootPath, @"\Upload\Product\"),
                Product = product,
            };

			ProductImages hoverImage = new ProductImages()
			{
				IsPrime = false,
				ImgUrl = createProductVM.HoverPhoto.Upload(_env.WebRootPath, @"\Upload\Product\"),
				Product = product,
			};
            

            product.ProductImages.Add(mainImage);
            product.ProductImages.Add(hoverImage);

            if (createProductVM.Photos != null)
            {
                foreach (var item in createProductVM.Photos)
                {
					if (!item.CheckType("image/"))
					{
                        TempData["Error"] += $"{item.FileName} Type is not correct. \t ";

						continue;
					}
					if (!item.CheckLength(3000))
					{
						TempData["Error"] += $"{item.FileName} Image is more than 3mb. \t";
						continue;
					}
					ProductImages newPhoto = new ProductImages()
					{
						IsPrime = null,
						ImgUrl = item.Upload(_env.WebRootPath, @"\Upload\Product\"),
						Product = product,
					};
                    product.ProductImages.Add(newPhoto);
				}
            }







			await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        public async Task<IActionResult> Update(int id)
        {
            Product product = await _context.Products.Where(p=>p.IsDeleted==false).Include(p=>p.Category)
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductImages)
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            if(product is null )
            {
                return View("Error");
            }
			ViewBag.Categories = await _context.Categories.ToListAsync();
			ViewBag.Tags = await _context.Tags.ToListAsync();
            UpdateProductVM updateProductVM = new UpdateProductVM()
            {
                Id = id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ProductCode = product.ProductCode,
                CategoryId = product.CategoryId,
                TagIds = new List<int>(),
                productImages=new List<ProductImagesVM>()
            };

            foreach (var item in product.ProductTags)
            {
                updateProductVM.TagIds.Add(item.TagId);
;           }
            foreach (var item in product.ProductImages)
            {
                ProductImagesVM productImagesVM = new ProductImagesVM()
                {
                    IsPrime = item.IsPrime,
                    ImgUrl=item.ImgUrl,
                    Id= item.Id
                };
                updateProductVM.productImages.Add(productImagesVM);
            }

			return View(updateProductVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM updateProductVM)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            Product existProduct = await _context.Products.Include(p=>p.ProductImages).Include(p => p.ProductTags).Where(p => p.Id == updateProductVM.Id).FirstOrDefaultAsync();
            if (existProduct is null)
            {
                return View("Error");
            }
            bool resultCategory = await _context.Categories.AnyAsync(c => c.Id == updateProductVM.CategoryId);
            if (!resultCategory)
            {
                ModelState.AddModelError("CatgoryId", "No such category exists.");
                return View();
            }
            existProduct.Name = updateProductVM.Name;
            existProduct.Price = updateProductVM.Price;
            existProduct.ProductCode = updateProductVM.ProductCode;
            existProduct.Description = updateProductVM.Description;
            existProduct.CategoryId = updateProductVM.CategoryId;


            if (updateProductVM.TagIds != null)
            {
                foreach (int tagId in updateProductVM.TagIds)
                {
                    bool resultTag = await _context.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", "No such tag exists.");
                        return View();
                    }

                }


                List<int> createTags;
                if (existProduct.ProductTags != null)
                {
                    createTags = updateProductVM.TagIds.Where(ti => !existProduct.ProductTags.Exists(pt => pt.TagId == ti)).ToList();
                }
                else
                {
                    createTags = updateProductVM.TagIds.ToList();
                }

                foreach (var tagid in createTags)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagid,
                        ProductId = existProduct.Id
                    };
                    //existProduct.ProductTags.Add(productTag);



                    await _context.ProductTags.AddAsync(productTag);
                };

                List<ProductTag> removeTags = existProduct.ProductTags.Where(pt => !updateProductVM.TagIds.Contains(pt.TagId)).ToList();

                _context.ProductTags.RemoveRange(removeTags);

            }
            else
            {
                var productTagList = _context.ProductTags.Where(pt => pt.ProductId == existProduct.Id).ToList();
                _context.ProductTags.RemoveRange(productTagList);
            }

            TempData["Error"] = "";
            if (updateProductVM.MainPhoto != null)
            {


                if (!updateProductVM.MainPhoto.CheckType("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "Enter right image format.");
                    return View();
                }
                if (!updateProductVM.MainPhoto.CheckLength(3000))
                {
                    ModelState.AddModelError("MainPhoto", "Enter max 3mb image.");
                    return View();
                }

                ProductImages newMainImages = new ProductImages()
                {
                    IsPrime = true,
                    ProductId = existProduct.Id,
                    ImgUrl = updateProductVM.MainPhoto.Upload(_env.WebRootPath, @"\Upload\Product\")
                };
                var oldMainPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == true);
                existProduct.ProductImages?.Remove(oldMainPhoto);
                existProduct.ProductImages.Add(newMainImages);
            }
			if (updateProductVM.HoverPhoto != null)
			{


				if (!updateProductVM.HoverPhoto.CheckType("image/"))
				{
					ModelState.AddModelError("HoverPhoto", "Enter right image format.");
					return View();
				}
				if (!updateProductVM.HoverPhoto.CheckLength(3000))
				{
					ModelState.AddModelError("HoverPhoto", "Enter max 3mb image.");
					return View();
				}

				ProductImages newHoverImages = new ProductImages()
				{
					IsPrime = false,
					ProductId = existProduct.Id,
					ImgUrl = updateProductVM.HoverPhoto.Upload(_env.WebRootPath, @"\Upload\Product\")
				};
				var oldHoverPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == false);
				existProduct.ProductImages?.Remove(oldHoverPhoto);
				existProduct.ProductImages.Add(newHoverImages);
			}
            if(updateProductVM.Photos != null)

            {
				foreach (var item in updateProductVM.Photos)
				{
					if (!item.CheckType("image/"))
					{
						TempData["Error"] += $"{item.FileName} Type is not correct. \t ";

						continue;
					}
					if (!item.CheckLength(3000))
					{
						TempData["Error"] += $"{item.FileName} Image is more than 3mb. \t";
						continue;
					}
					ProductImages newPhoto = new ProductImages()
					{
						IsPrime = null,
						ImgUrl = item.Upload(_env.WebRootPath, @"\Upload\Product\"),
						Product = existProduct,
					};
					existProduct.ProductImages.Add(newPhoto);
				}
			}
            if (updateProductVM.ImageIds != null)
            {

                var removeLitImage = existProduct.ProductImages?.Where(p => !updateProductVM.ImageIds.Contains(p.Id) && p.IsPrime == null).ToList();
                foreach (var item in removeLitImage)
                {
                    existProduct.ProductImages.Remove(item);
                    item.ImgUrl.DeleteFile(_env.WebRootPath, @"\Upload\Product\");
                }
            }
            else
            {
                existProduct.ProductImages.RemoveAll(p=>p.IsPrime==null);
            }





			await _context.SaveChangesAsync();
            return RedirectToAction("Index");

		}
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Where(p=>p.IsDeleted==false).FirstOrDefault(p => p.Id == id);
            if(product is null) 
            {
                return View("Error");
            };
            product.IsDeleted=true;
            _context.SaveChanges();
           
            return Ok();
        }
    }
}
