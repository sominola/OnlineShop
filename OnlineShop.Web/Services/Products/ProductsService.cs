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
            var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
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