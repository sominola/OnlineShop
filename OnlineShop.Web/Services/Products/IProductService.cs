using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.Services.Products
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();

        Task CreateProductAsync(Product product);
        
        Task<Product> GetProductByIdAsync(Guid id);
        Task EditProduct(Product product);

        Task DeleteProductAsync(Product product);
    }
}