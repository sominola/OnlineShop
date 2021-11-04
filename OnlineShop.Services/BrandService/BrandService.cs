using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.BrandService
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _db;

        public BrandService(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<Brand> GetAllBrands()
        {
            return _db.Brands;
        }

        public IQueryable<Brand> FindBrandById(Guid id)
        {
            return _db.Brands.Where(b => b.Id == id);
        }

        public async Task<Brand> FindBrandsByNameAsync(string name)
        {
            return await _db.Brands.FirstOrDefaultAsync(b => b.Name == name);
        }

        public async Task CreateBrand(Brand brand)
        {
            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
        }
    }
}