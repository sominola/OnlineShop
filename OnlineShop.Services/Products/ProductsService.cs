using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.Products
{
    public class ProductsService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IMemoryCache _cache;

        public ProductsService(AppDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _db.Products.Include(i => i.Images).AsNoTracking();
        }

        public IQueryable<Product> GetProductsByName(IQueryable<Product> products, string name)
        {
            products = products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            return products;
        }

        public IQueryable<Product> GetProductsByBrand(IQueryable<Product> products, string brand)
        {
            return products.Where(p => p.Brand.Name == brand);
        }

        public IQueryable<Product> SkipTakeProducts(IQueryable<Product> products, int skip, int take)
        {
            return products.Skip(skip).Take(take);
        }

        public IQueryable<Product> SortProductsByOrder(IQueryable<Product> products, OrderBy order)
        {
            return order switch
            {
                OrderBy.DateAsc => products.OrderBy(p => p.DateCreated),

                OrderBy.DateDesc => products.OrderByDescending(p => p.DateCreated),

                OrderBy.NameAsc => products.OrderBy(p => p.Name),

                OrderBy.NameDesc => products.OrderByDescending(p => p.Name),

                OrderBy.PriceAsc => products.OrderBy(p => p.Price),

                OrderBy.PriceDesc => products.OrderByDescending(p => p.Price),

                _ => throw new ArgumentException(null, nameof(order))
            };
        }

        public async Task CreateProductAsync(Product product)
        {
            await _db.Products.AddAsync(product);
            var n = await _db.SaveChangesAsync();
            if (n > 0)
            {
                _cache.Set(product.Id, product, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _cache.GetOrCreateAsync(id, async _ =>
            {
                return await _db.Products.Include(p => p.Images)
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == id);
            });
        }

        public async Task UpdateProductAsync(Product product)
        {
            var dbProduct = await GetProductByIdAsync(product.Id);
            if (dbProduct != null)
            {
                dbProduct.Name = product.Name;
                dbProduct.Price = product.Price;
                dbProduct.SizeProduct = product.SizeProduct;
                dbProduct.ColorProduct = product.ColorProduct;
                dbProduct.Description = product.Description;
                dbProduct.Brand = product.Brand;

                _db.Images.RemoveRange(dbProduct.Images);
                dbProduct.Images.AddRange(product.Images);

                _db.Products.Update(dbProduct);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException(nameof(dbProduct));
            }
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var dbProduct = await GetProductByIdAsync(id);
            if (dbProduct != null)
            {
                _cache.TryGetValue(id, out Product product);
                if (product != null)
                {
                    _cache.Remove(id);
                }

                _db.Images.RemoveRange(dbProduct.Images);
                _db.Products.Remove(dbProduct);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException(nameof(dbProduct));
            }
        }
    }
}