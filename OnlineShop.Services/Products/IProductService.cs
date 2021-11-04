using System;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.Products
{
    public interface IProductService
    {
        IQueryable<Product> GetAllProducts();

        IQueryable<Product> GetProductsByName(IQueryable<Product> products, string name);
        
        IQueryable<Product> GetProductsByBrand(IQueryable<Product> products, string brand);
        IQueryable<Product> SortProductsByOrder(IQueryable<Product> products, OrderBy order);

        IQueryable<Product> SkipTakeProducts(IQueryable<Product> products, int skip, int take);

        Task CreateProductAsync(Product product);

        Task<Product> GetProductByIdAsync(Guid id);
        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(Guid id);
    }
}