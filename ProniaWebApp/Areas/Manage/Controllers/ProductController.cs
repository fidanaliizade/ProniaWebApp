using Microsoft.AspNetCore.Mvc;
using ProniaWebApp.Areas.Manage.ViewModels.Product;

namespace ProniaWebApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {

        AppDbContext _context { get; set; }

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> product = await _context.Products.Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag).ToListAsync();
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
            



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        public async Task<IActionResult> Update(int id)
        {
            Product product = await _context.Products.Include(p=>p.Category)
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
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
                TagIds = new List<int>()
            };

            foreach (var item in product.ProductTags)
            {
                updateProductVM.TagIds.Add(item.TagId);
;            }

			return View(updateProductVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM updateProductVM)
        {
			ViewBag.Categories = await _context.Categories.ToListAsync();
			ViewBag.Tags = await _context.Tags.ToListAsync(); 
            if(!ModelState.IsValid)
            {
                return View();
            }
			Product existProduct = await _context.Products.Where(p => p.Id == updateProductVM.Id).FirstOrDefaultAsync();
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
            existProduct.CategoryId= updateProductVM.CategoryId;


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
                if(existProduct!=null)
                {
                    createTags= updateProductVM.TagIds.Where(ti=>!existProduct.ProductTags.Exists(pt => pt.TagId == ti)).ToList();
                }
                else
                {
                    createTags=updateProductVM.TagIds.ToList();
                }

                foreach (var tagid in createTags)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagid,
                        ProductId = existProduct.Id
                    };
                    //existProduct.ProductTags.Add(productTag);

                    await  _context.ProductTags.AddAsync(productTag);
                }0

                List<ProductTag> removeTags=existProduct.ProductTags.Where(pt=>updateProductVM.TagIds.Contains(pt.TagId)).ToList();
                 
                _context.ProductTags.RemoveRange(removeTags);

            }
            else
            {
                var productTagList = _context.ProductTags.Where(pt=>pt.ProductId == existProduct.Id).ToList();
                _context.ProductTags.RemoveRange(productTagList);
            }



            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

		}
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if(product is null) 
            {
                return View("Error");
            };
            _context.Products.Remove(product);
            _context.SaveChanges();
           
            return RedirectToAction(nameof(Index));
        }
    }
}
