
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _db;
        public HomeController(AppDbContext db)
        {
                _db = db;
        }
        public async  Task<IActionResult> Index()
        {

            HomeVM vm = new HomeVM()
            {
                Products = await _db.Products.Include(p => p.ProductImages).ToListAsync(),
                Sliders= await _db.Sliders.ToListAsync(),
            };
            return View(vm);
        }
    }
}
