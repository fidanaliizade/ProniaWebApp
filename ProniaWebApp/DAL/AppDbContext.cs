using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ProniaWebApp.DAL
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)

        {
                
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
		public DbSet<Slider> Sliders { get; set; }
        public DbSet<Setting> Settings { get; set; } 
	}
}
