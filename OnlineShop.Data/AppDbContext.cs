using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data.Models;

namespace OnlineShop.Data
{
    public sealed class AppDbContext: IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> Images { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

    }

    
}