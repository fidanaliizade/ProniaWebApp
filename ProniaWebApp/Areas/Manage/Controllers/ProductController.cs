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
                return View();
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
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
			return View(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if(product is null) 
            {
                return View("Error");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
