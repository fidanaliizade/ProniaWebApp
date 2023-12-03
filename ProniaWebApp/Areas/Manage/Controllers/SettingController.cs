using Microsoft.AspNetCore.Mvc;
using ProniaWebApp.Models;

namespace ProniaWebApp.Areas.Manage.Controllers
{
	[Area("Manage")]
	public class SettingController : Controller
    {


        AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Setting> settings =await _context.Settings.ToListAsync();
            return View(settings);
        }

		[HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Setting setting)
        {
			if (!ModelState.IsValid)
			{
				return View();
			}
			_context.Settings.Add(setting);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int id)
		{
			Setting setting = _context.Settings.Find(id);
			_context.Settings.Remove(setting);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Update(int id)
		{
			Setting setting = _context.Settings.Find(id);
			return View(setting);
		}
		[HttpPost]
		public IActionResult Update(Setting newSetting)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			Setting oldSetting = _context.Settings.Find(newSetting.Id);
			oldSetting.Key=newSetting.Key;
			oldSetting.Value=newSetting.Value;
			
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
