using Microsoft.AspNetCore.Mvc;

namespace ProniaWebApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        AppDbContext _context;

		public CategoryController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
        {
            List<Category> categories = _context.Categories.Include(p => p.Products).ToList();
            return View(categories);
        }
        public IActionResult Create() 
        { 
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        public IActionResult Update(int id)
        {
            return View();
        }
    }
}
