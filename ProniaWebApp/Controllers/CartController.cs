using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
	public class CartController : Controller
	{
		AppDbContext _context;

		public CartController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var jsonCookie = Request.Cookies["Basket"];
			List<BasketCookieItemVM> basketItems = new List<BasketCookieItemVM>();
			if (jsonCookie != null)
				if (jsonCookie != null)
				{
					var cookieItems = JsonConvert.DeserializeObject<List<CookieItemVM>>(jsonCookie);

					bool countCheck = false;
					List<CookieItemVM> deletedCookie = new List<CookieItemVM>();
					foreach (var item in cookieItems)
					{
						Product product = _context.Products.Where(p => p.IsDeleted == false).Include(p => p.ProductImages.Where(p => p.IsPrime == true)).FirstOrDefault(p => p.Id == item.Id);
						if (product == null)
						{
							deletedCookie.Add(item);
							continue;
						}
						basketItems.Add(new BasketCookieItemVM()
						{
							Id = item.Id,
							Name = product.Name,
							Price = product.Price,
							Count = item.Count,
							ImgUrl = product.ProductImages.FirstOrDefault().ImgUrl
						});
					}
					if (deletedCookie.Count > 0)
					{
						foreach (var delete in deletedCookie)
						{
							cookieItems.Remove(delete);
						}
						Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookieItems));
					}


				}
			return View(basketItems);
		}
		public IActionResult AddBasket(int id)
		{
			if (id <= 0) return BadRequest();
			Product product = _context.Products.Where(p => p.IsDeleted == false).FirstOrDefault(p => p.Id == id);
			if (product == null) return NotFound();
			List<CookieItemVM> basket;
			var json = Request.Cookies["Basket"];

			if (json != null)
			{
				basket = JsonConvert.DeserializeObject<List<CookieItemVM>>(json);
				var existProduct = basket.FirstOrDefault(p => p.Id == id);
				if (existProduct != null)
				{
					existProduct.Count += 1;
				}
				else
				{
					basket.Add(new CookieItemVM()
					{
						Id = id,
						Count = 1
					});
				}

			}

			else
			{
				basket = new List<CookieItemVM>();
				basket.Add(new CookieItemVM()
				{
					Id = id,
					Count = 1
				});
			}


			var cookieBasket = JsonConvert.SerializeObject(basket);
			Response.Cookies.Append("Basket", cookieBasket);
			return RedirectToAction(nameof(Index), "Home");
		}

		public IActionResult RemoveBasketItem(int id)
		{
			var cookieBasket = Request.Cookies["Basket"];
			if (cookieBasket != null)
			{
				List<CookieItemVM> basket = JsonConvert.DeserializeObject<List<CookieItemVM>>(cookieBasket);

				var deleteElement = basket.FirstOrDefault(p => p.Id == id);
				if (deleteElement != null)
				{
					basket.Remove(deleteElement);
				}

				Response.Cookies.Append("Basket", JsonConvert.SerializeObject(basket));
				return Ok();
			}
			return NotFound();
		}

		public IActionResult GetBasket()
		{
			var basketCookieJson = Request.Cookies["Basket"];

			return Content(basketCookieJson);
		}


	
	}
}
