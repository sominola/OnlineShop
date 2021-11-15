using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data.Models;
using OnlineShop.Services.Products;

// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace OnlineShop.Web.ViewModels.Products
{
    public class ProductPageViewModel
    {
        public FilterViewModel Filter { get; init; }
        public IEnumerable<Product> Products => _products?.ToList();
        public int TotalProducts { get; private set; }
        public int TotalPages { get; private set; }

        private IQueryable<Product> _products;
        private readonly IProductService _productService;

        public ProductPageViewModel(FilterViewModel filter)
        {
            Filter = filter;
        }

        public ProductPageViewModel(FilterViewModel filter, IProductService productService)
        {
            Filter = filter;
            _productService = productService;
            _products = _productService.GetAllProducts();
            _products = _productService.SortProductsByOrder(_products, Filter.CurrentSort);
        }


        public void SortProductsByName()
        {
            if (!string.IsNullOrEmpty(Filter.CurrentName))
            {
                _products = _productService.GetProductsByName(_products, Filter.CurrentName);
            }
        }

        public void SortProductsByBrand()
        {
            if (!string.IsNullOrEmpty(Filter.CurrentBrand) && Filter.CurrentBrand != "All")
            {
                _products = _productService.GetProductsByBrand(_products, Filter.CurrentBrand);
            }
        }

        public void GenerateBrands()
        {
            var brands = _products.Select(x => x.Brand).Distinct().ToList();
            brands.Insert(0, new Brand() {Name = "All", Id = Guid.Empty});
            Filter.Brands = new SelectList(brands, "Name", "Name", Filter.CurrentBrand);
        }

        public async Task GenerateSkipTakeAsync()
        {
            int skipProducts = (Filter.CurrentPage - 1) * Filter.TotalProductsOnPage;
            double totalProducts = await _products.CountAsync();
            int maxPages = (int) Math.Ceiling(totalProducts / Filter.TotalProductsOnPage);

            //check if take > countProducts
            int takeProducts = Filter.TotalProductsOnPage;
            int skipTake = skipProducts + takeProducts;
            if (skipTake > totalProducts)
                takeProducts -= skipTake - (int) totalProducts;


            if (totalProducts > Filter.TotalProductsOnPage)
                _products = _productService.SkipTakeProducts(_products, skipProducts, takeProducts);

            TotalPages = maxPages;
            TotalProducts = (int) totalProducts;
            Filter.TotalProductsOnPage = skipProducts + takeProducts;
        }

        public async Task<bool> ProductsAnyAsync()
        {
            return await _products.AnyAsync();
        }
    }
}