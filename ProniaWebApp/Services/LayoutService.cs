using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Services
{
	public class LayoutService
	{
		AppDbContext _context;
		IHttpContextAccessor _http;
		private readonly UserManager<AppUser> _userManager;

		public LayoutService(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager)
		{
			_context = context;

			_http = http;
			_userManager = userManager;
		}

		public async Task<Dictionary<string, string>> GetSetting()
		{
			Dictionary<string, string> setting = _context.Settings.ToDictionary(s => s.Key, s => s.Value);
			return setting;
		}


		public async Task<List<BasketCookieItemVM>> GetBasket()
		{
			List<BasketCookieItemVM> basketItems = new List<BasketCookieItemVM>();
			if (_http.HttpContext.User.Identity.IsAuthenticated)
			{

				AppUser user = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
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

				var jsonCookie = _http.HttpContext.Request.Cookies["Basket"];
				if (jsonCookie != null)
				{
					var cookieItems = JsonConvert.DeserializeObject<List<CookieItemVM>>(jsonCookie);
					bool countCheck = false;
					List<CookieItemVM> deletedCookie = new List<CookieItemVM>();

					foreach (var item in cookieItems)
					{
						Product product = await _context.Products.Where(p => p.IsDeleted == false).Include(p => p.ProductImages.Where(p => p.IsPrime == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
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
						_http.HttpContext.Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookieItems));
					}
				}

			}
			return basketItems;

		
			
		}
	}
}
