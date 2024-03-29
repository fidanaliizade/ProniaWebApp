﻿using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
    public class ShopController:Controller
    {
        AppDbContext _db;

        public ShopController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Detail(int? id)
        {
            //string cookie = Request.Cookies[".AspNetCore.Session"];
            //string session = HttpContext.Session.GetString("Name");

            //if(session == null) return NotFound();




			Product product = _db.Products
                .Where(p=>p.IsDeleted == false)
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .FirstOrDefault(product=>product.Id==id);
            if(product==null)
            {
                return NotFound();  
            }
            DetailVM detailVM = new DetailVM()
            {
                Product = product,
                Products=_db.Products.Include(p=>p.ProductImages).Include(p=>p.Category).Where(p=>p.CategoryId==product.CategoryId&&p.Id!=product.Id).ToList()
            };

            return View(detailVM);
        }
    }
}
