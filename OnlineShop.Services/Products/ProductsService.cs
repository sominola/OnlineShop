using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.Products
{
    public class ProductsService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductsService(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _db.Products.Include(i => i.Images).AsNoTracking();
        }

        public IQueryable<Product> GetProductsByName(IQueryable<Product> products, string name)
        {
            products = products.Where(p => p.Name.Contains(name));
            products = products.Where(p => p.Name.Contains(name.ToUpper()));
            products = products.Where(p => p.Name.Contains(name.ToLower()));
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
            await _db.SaveChangesAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await _db.Products.Include(p => p.Images).Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);
            return product;
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