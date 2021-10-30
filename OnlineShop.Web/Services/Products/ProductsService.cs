using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.Services.Products
{
    public class ProductsService: IProductService
    {
        private readonly AppDbContext _db;

        public ProductsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _db.Products.Include(i => i.Images).ToListAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
        }
        
        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task EditProduct(Product product)
        {
            var dbProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (dbProduct != null)
            {
                dbProduct = product;
                _db.Products.Update(dbProduct);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException(nameof(dbProduct));
            }
        }

        public async Task DeleteProductAsync(Product product)
        {
            var dbProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (dbProduct != null)
            {
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