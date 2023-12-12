using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProniaWebApp.Models;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
	public class CartController : Controller
	{
		AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public CartController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async  Task<IActionResult> Index()
		{
			List<BasketCookieItemVM> basketItems = new List<BasketCookieItemVM>();
			if (User.Identity.IsAuthenticated)
			{
				
					AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
					List<BasketItem> userBaskets = await _context.BasketItems
						.Where(b => b.AppUserId == user.Id)
						.Include(b => b.Product)
						.ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrime == true))
						.Where(b => !b.Product.IsDeleted)
						.ToListAsync();
					foreach (var item in userBaskets)
					{
						basketItems.Add(new BasketCookieItemVM()
						{
							Name = item.Product.Name,
							ImgUrl = item.Product.ProductImages.FirstOrDefault().ImgUrl,
							Price = item.Price,
							Count = item.Count,
						});
					}
				}
			else
			{
				var jsonCookie = Request.Cookies["Basket"];

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
			}
			return View(basketItems);
		}
		public  async Task<IActionResult> AddBasket(int id)
		{
			if (id <= 0) return BadRequest();
			Product product = _context.Products.Where(p => p.IsDeleted == false).FirstOrDefault(p => p.Id == id);
			if (product == null) return NotFound();


			if (User.Identity.IsAuthenticated)
			{
				AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
				BasketItem oldItem = _context.BasketItems
					.FirstOrDefault(b => b.AppUserId == user.Id && b.ProductId == id);
				if (oldItem == null)
				{
					BasketItem newItem = new BasketItem()
					{
						AppUser = user,
						Product = product,
						Price = product.Price,
						Count = 1
					};
					_context.BasketItems.Add(newItem);
				}
				else
				{
					oldItem.Count += 1;
				}
				await _context.SaveChangesAsync();
			}
			else
			{

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
			}
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
