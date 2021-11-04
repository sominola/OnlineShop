using System;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.BrandService
{
    public interface IBrandService
    {
        public IQueryable<Brand> GetAllBrands();

        public IQueryable<Brand> FindBrandById(Guid id);


        Task<Brand> FindBrandsByNameAsync(string name);

        Task CreateBrand(Brand brand);
    }
}